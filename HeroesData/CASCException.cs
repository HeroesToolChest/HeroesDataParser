using System;

namespace HeroesData
{
    internal class CASCException : Exception
    {
        public CASCException(string message)
            : base(message)
        {
        }

        public CASCException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
