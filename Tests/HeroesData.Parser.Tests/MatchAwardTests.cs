using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class MatchAwardTests : HeroDataBaseTest
    {
        [TestMethod]
        public void MatchAwardListTests()
        {
            List<MatchAward> matchAwards = MatchAwardParser.Parse(Localization.ENUS).ToList();

            Assert.AreEqual(6, matchAwards.Count);

            foreach (MatchAward matchAward in matchAwards)
            {
                if (matchAward.ShortName == "MVP")
                    continue;

                Assert.IsNotNull(matchAward.Description);
                Assert.IsNotNull(matchAward.MVPScreenImageFileNameOriginal);
                Assert.IsNotNull(matchAward.MVPScreenImageFileName);
                Assert.IsNotNull(matchAward.Name);
                Assert.IsNotNull(matchAward.ScoreScreenImageFileNameOriginal);
                Assert.IsNotNull(matchAward.ScoreScreenImageFileName);
                Assert.IsNotNull(matchAward.ShortName);
                Assert.IsNotNull(matchAward.Tag);
            }
        }
    }
}
