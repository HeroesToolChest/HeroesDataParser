using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Helpers
{
    public class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[]? x, string[]? y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(string[] obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            int value = 0;
            foreach (var item in obj)
            {
                value += item.GetHashCode(System.StringComparison.Ordinal) * 17;
            }

            return value;
        }
    }
}
