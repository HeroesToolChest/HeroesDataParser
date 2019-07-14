using Heroes.Models;

namespace HeroesData.FileWriter.Tests.MatchAwardData
{
    public class MatchAwardDataOutputBase : FileOutputTestBase<MatchAward>
    {
        public MatchAwardDataOutputBase()
            : base(nameof(MatchAwardData))
        {
        }

        protected override void SetTestData()
        {
            MatchAward matchAward = new MatchAward()
            {
                Id = "Bulwark",
                HyperlinkId = "EndOfMatchBulwark",
                Name = "Bulwark",
                Description = new TooltipDescription("Highest damage soaked"),
                Tag = "AwBK",
                MVPScreenImageFileName = "image_bulwark.png",
                ScoreScreenImageFileName = "image_scorescreen_bulwark.png",
            };

            TestData.Add(matchAward);

            MatchAward matchAward2 = new MatchAward()
            {
                Id = "Killer",
                HyperlinkId = "EndOfMatchKiller",
                Name = "Killer",
                Description = new TooltipDescription("Most Kills"),
                Tag = "AwKL",
                MVPScreenImageFileName = "image_killer.png",
                ScoreScreenImageFileName = "image_scorescreen_killer.png",
            };

            TestData.Add(matchAward2);
        }
    }
}
