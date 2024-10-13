using System.Collections.Concurrent;

namespace Dost_Library.Entities
{
    internal sealed class Seat
    {
        public int SeatNumber 
        { 
            get 
            { 
                return _seatNumber;
            } 
            set 
            {
                _seatNumber = value;
            } 
        }
        private int _seatNumber;
        public int CurrentBookId { get; set; }
        public bool IsOccupied { get; set; } = false;
        public ConcurrentQueue<Student> WaitingQueue { get; set; } = new();

        public Seat(int num)
        {
            SeatNumber = num;
        }
    }
}
