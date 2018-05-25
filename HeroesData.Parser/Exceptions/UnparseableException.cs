using System;

namespace HeroesData.Parser.Exceptions
{
    [Serializable]
    internal class UnparseableException : Exception
    {
        public UnparseableException(string message)
            : base(message)
        {
        }
    }
}
