using System;

namespace HeroesData.Parser
{
    [Serializable]
    public class UnknownXmlElementException : Exception
    {
        public UnknownXmlElementException()
        {
        }

        public UnknownXmlElementException(string message)
            : base(message)
        {
        }

        public UnknownXmlElementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
