namespace Dost_Library.Helper
{
    public static class RandomGenerator
    {
        private static Random _random = new();   
        public static int GenerateRandomInt(int begin, int end)
        {
            return _random.Next(begin, end);
        }
    }
}