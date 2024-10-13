using Dost_Library.Entities;

namespace Dost_Library.Application
{
    internal class Simulation
    {
        private readonly int _maxSeats;
        private readonly int _booksAvailableForReading;
        private readonly List<Seat> _seats;
        private readonly List<Task> _tasks = [];
        private readonly object _lock = new();

        public Simulation(int maxSeats, int books)
        {
            _maxSeats = maxSeats;
            _booksAvailableForReading = books;
            _seats = Enumerable.Range(1, _maxSeats).Select(i => new Seat(i)).ToList();
        }

        public void StartSimulation(IEnumerable<Student> students)
        {
            foreach (var student in students)
            {
                // Create a task to seat each student and add it to the task list
                var task = Task.Run(() => SeatStudent(student)); 
                _tasks.Add(task);
            }
        }

        public void WaitForAll()
        {
            Task.WhenAll(_tasks).Wait();
        }

        private Task SeatStudent(Student student)
        {
            Seat assignedSeat;
            // Lock the seat selection and assignment to ensure that only one student can occupy a seat at a time
            lock (_lock)
            {
                // Check if there's a seat already occupied by a student reading the same book
                // Authors Note:
                // Why did I do this? Although this is simulated, it would be resonable to think that we can cache the book
                // That this book can be cached once for multiple continious readers rather than diposed of and brought back from database continously.
                // Though this does create a bottleneck if the seat number is low, I think it is worth it when it comes to saving database costs (especially if we are using one hosted in AWS)

                if(_booksAvailableForReading <= _seats.Count) //There is (more than) enough seats for each book.
                    assignedSeat = _seats.FirstOrDefault(seat => seat.IsOccupied && seat.CurrentBookId == student.BookId);
                else 
                    assignedSeat = _seats.FirstOrDefault(seat => seat.IsOccupied);

                if (assignedSeat != null)
                {
                    Console.WriteLine($"{student.Name} is queued for Seat {assignedSeat.SeatNumber} to read {student.Book.Name}");
                    assignedSeat.WaitingQueue.Enqueue(student);
                    return Task.CompletedTask; // Exit once queued, no further task to track here
                }

                // Find an available seat if none are reading the same book
                assignedSeat = _seats.FirstOrDefault(seat => !seat.IsOccupied);

                if (assignedSeat != null)
                {
                    assignedSeat.IsOccupied = true;
                    assignedSeat.CurrentBookId = student.BookId;
                    Console.WriteLine($"{student.Name} is seated at Seat {assignedSeat.SeatNumber} to read {student.Book.Name}");
                }
            }

            if (assignedSeat != null)
            {
                return ReadBookInSeat(assignedSeat, student); // Return the task from reading the book
            }
            else
            {
                Console.WriteLine($"{student.Name} is waiting for an available seat.");
                return Task.Delay(1000).ContinueWith(t => SeatStudent(student)); // Reattempt seating after delay
            }
        }

        private Task ReadBookInSeat(Seat seat, Student student)
        {
            Console.WriteLine($"{student.Name} is reading {student.Book.Name} at Seat {seat.SeatNumber} for {student.TimeItTakesToRead} ms");

            // Use Task.Delay to simulate the reading process and chain the continuation properly
            return Task.Delay(student.TimeItTakesToRead).ContinueWith(_ =>
            {
                Console.WriteLine($"{student.Name} has finished reading {student.Book.Name} at Seat {seat.SeatNumber}");

                lock (_lock)
                {
                    // Check if there are students waiting for this seat
                    if (seat.WaitingQueue.TryDequeue(out var nextStudent))
                    {
                        Console.WriteLine($"{nextStudent.Name} is now seated at Seat {seat.SeatNumber} to read {nextStudent.Book.Name}");
                        return ReadBookInSeat(seat, nextStudent);
                    }
                    else
                    {
                        // Free the seat if no one is waiting
                        Console.WriteLine($"Seat {seat.SeatNumber} is now free.");
                        seat.IsOccupied = false;
                        seat.CurrentBookId = 0;
                        return Task.CompletedTask; // No more tasks to run
                    }
                }
            }).Unwrap();
        }
    }
}