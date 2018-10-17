using Heroes.Models;
using HeroesData.Parser.MatchAwards;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HeroesData.Parser.Tests
{
    public class MatchAwardTests : HeroDataBaseTest
    {
        public MatchAwardTests()
        {
        }

        [Fact]
        public void MatchAwardListTests()
        {
            List<MatchAward> matchAwards = MatchAwardParser.GetParsedMatchAwards().ToList();

            Assert.Equal(6, matchAwards.Count);

            foreach (MatchAward matchAward in matchAwards)
            {
                if (matchAward.ShortName == "MVP")
                    continue;

                Assert.NotNull(matchAward.Description);
                Assert.NotNull(matchAward.MVPScreenImageFileNameOriginal);
                Assert.NotNull(matchAward.MVPScreenImageFileName);
                Assert.NotNull(matchAward.Name);
                Assert.NotNull(matchAward.ScoreScreenImageFileNameOriginal);
                Assert.NotNull(matchAward.ScoreScreenImageFileName);
                Assert.NotNull(matchAward.ShortName);
                Assert.NotNull(matchAward.Tag);
            }
        }
    }
}
