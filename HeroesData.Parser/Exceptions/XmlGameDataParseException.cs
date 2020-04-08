using System;

namespace HeroesData.Parser.Exceptions
{
    public class XmlGameDataParseException : Exception
    {
        public XmlGameDataParseException()
        {
        }

        public XmlGameDataParseException(string? message)
            : base(message)
        {
        }

        public XmlGameDataParseException(string? message, Exception? ex)
            : base(message, ex)
        {
        }
    }
}
