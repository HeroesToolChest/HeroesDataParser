using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HeroesData.Helpers
{
    public class MatchComparer : IEqualityComparer<Match>
    {
        public bool Equals(Match x, Match y)
        {
            if (x is null || y is null)
                return false;

            return x.Value.Equals(y.Value, StringComparison.Ordinal);
        }

        public int GetHashCode(Match obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            return obj.Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }
    }
}
