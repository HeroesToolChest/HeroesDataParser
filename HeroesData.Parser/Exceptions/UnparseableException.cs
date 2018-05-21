using System;

namespace HeroesData.Parser.Exceptions
{
    [Serializable]
    public class UnparseableException : Exception
    {
        public UnparseableException(string message)
            : base(message)
        {
        }
    }
}
