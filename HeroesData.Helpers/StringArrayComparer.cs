using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Helpers
{
    public class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(string[] obj)
        {
            int value = 0;
            foreach (var item in obj)
            {
                value += item.GetHashCode() * 17;
            }

            return value;
        }
    }
}
