using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HeroesData.Helpers
{
    public class MatchComparer : IEqualityComparer<Match>
    {
        public bool Equals(Match x, Match y)
        {
            return x.Value.Equals(y.Value);
        }

        public int GetHashCode(Match obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
