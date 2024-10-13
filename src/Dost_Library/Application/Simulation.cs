using Dost_Library.Entities;

namespace Dost_Library.Application
{
    internal class Simulation
    {
        private readonly int _maxSeats;
        private readonly List<Seat> _seats;
        private readonly List<Task> _tasks = [];

        public Simulation(int maxSeats)
        {
            _maxSeats = maxSeats;
            _seats = Enumerable.Range(1, _maxSeats).Select(i => new Seat(i)).ToList();
        }

        public void StartSimulation(IEnumerable<Student> students)
        {
            foreach (var student in students)
            {
                var task = Task.Run(() => SeatStudent(student));
                _tasks.Add(task);
            }
        }

        private void SeatStudent(Student student)
        {
            var assignedSeat = _seats.FirstOrDefault(seat => seat.IsOccupied && seat.CurrentBookId == student.BookId);

            if (assignedSeat != null)
            {
                Console.WriteLine($"{student.Name} is queued for Seat {assignedSeat.SeatNumber} to read {student.Book.Name}");
                assignedSeat.WaitingQueue.Enqueue(student);
            }
            else
            {
                // Find an available seat if none are reading the same book
                var availableSeat = _seats.FirstOrDefault(seat => !seat.IsOccupied);

                if (availableSeat != null)
                {
                    // Occupy the seat with the student
                    availableSeat.IsOccupied = true;
                    availableSeat.CurrentBookId = student.BookId;
                    Console.WriteLine($"{student.Name} is seated at Seat {availableSeat.SeatNumber} to read {student.Book.Name}");
                    ReadBookInSeat(availableSeat, student);
                }
                else
                {
                    Console.WriteLine($"{student.Name} is waiting for an available seat.");
                    Thread.Sleep(1000); // Simulate waiting
                    SeatStudent(student); 
                }
            }
        }

        private void ReadBookInSeat(Seat seat, Student student)
        {
            Console.WriteLine($"{student.Name} is reading {student.Book.Name} at Seat {seat.SeatNumber} for {student.TimeItTakesToRead} ms");
            Thread.Sleep(student.TimeItTakesToRead); // Simulate the time it takes to read the book
            Console.WriteLine($"{student.Name} has finished reading {student.Book.Name} at Seat {seat.SeatNumber}");

            lock (_seats)
            {
                // Check if there are stude   nts waiting for this seat
                if (seat.WaitingQueue.TryDequeue(out var nextStudent))
                {
                    Console.WriteLine($"{nextStudent.Name} is now seated at Seat {seat.SeatNumber} to read {nextStudent.Book.Name}");
                    ReadBookInSeat(seat, nextStudent); // Let the next student read
                }
                else
                {
                    // Free the seat if no one is waiting
                    Console.WriteLine($"Seat {seat.SeatNumber} is now free.");
                    seat.IsOccupied = false;
                    seat.CurrentBookId = 0;
                }
            }
        }

        public void WaitForAll() 
        {
            Task.WaitAll([.. _tasks]);
        }
    } 
}