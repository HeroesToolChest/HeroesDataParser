using System;

namespace HeroesData.Parser.Exceptions
{
    [Serializable]
    public class GameStringParseException : Exception
    {
        public GameStringParseException()
        {
        }

        public GameStringParseException(string message)
            : base(message)
        {
        }

        public GameStringParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
