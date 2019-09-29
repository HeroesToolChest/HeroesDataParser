using System;

namespace HeroesData.Parser.Exceptions
{
    [Serializable]
    internal class XmlGameDataParseException : Exception
    {
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
