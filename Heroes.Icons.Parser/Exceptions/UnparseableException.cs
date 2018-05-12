using System;

namespace Heroes.Icons.Parser.Exceptions
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
