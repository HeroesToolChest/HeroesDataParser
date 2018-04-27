using System;

namespace Heroes.Icons.Parser
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
