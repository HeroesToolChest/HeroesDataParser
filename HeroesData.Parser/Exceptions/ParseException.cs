using System;

namespace HeroesData.Parser.Exceptions
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        {
        }

        public ParseException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
