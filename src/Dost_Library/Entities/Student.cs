using Dost_Library.Helper;

namespace Dost_Library.Entities
{
    public sealed class Student : BaseEntity
    {
        public string Name { get; set; }
        private int? _timeItTakesToRead;

        public int TimeItTakesToRead
        {
            get
            {
                if (!_timeItTakesToRead.HasValue)
                {
                    //Generate a random interval for student to read. 
                    _timeItTakesToRead = RandomGenerator.GenerateRandomInt(5, 61) * 1000;
                }
                return _timeItTakesToRead.Value;
            }
            set
            {
                _timeItTakesToRead = value;
            }
        }

        public Book Book { get; set; }
        public int BookId { get; set; }
    }
}