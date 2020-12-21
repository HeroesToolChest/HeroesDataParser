using Heroes.Models;

namespace HeroesData.FileWriter.Tests.RewardPortraitData
{
    public class RewardPortraitDataOutputBase : FileOutputTestBase<RewardPortrait>
    {
        public RewardPortraitDataOutputBase()
            : base(nameof(RewardPortraitData))
        {
        }

        protected override void SetTestData()
        {
            RewardPortrait rewardPortrait = new RewardPortrait()
            {
                Name = "Whitemane Cat",
                Id = "WhitemaneCat",
                HyperlinkId = "WhitemaneCatId",
                CollectionCategory = "AchievementCategory",
                Description = new TooltipDescription("This is some description"),
                DescriptionUnearned = new TooltipDescription("This is an unearned description"),
                HeroId = "Aba",
                IconSlot = 27,
                Rarity = Rarity.Epic,
                PortraitPackId = "LagForce",
                ImageFileName = "some file.png",
            };

            rewardPortrait.TextureSheet.Image = "this_is_an_image_file.dds";
            rewardPortrait.TextureSheet.Columns = 8;
            rewardPortrait.TextureSheet.Rows = 8;

            TestData.Add(rewardPortrait);

            RewardPortrait rewardPortrait2 = new RewardPortrait()
            {
                Name = "Whitemane Bat",
                Id = "WhitemaneBat",
                HyperlinkId = "WhitemaneBatId",
            };

            rewardPortrait2.TextureSheet.Image = "this_is_an_image_file.dds";
            rewardPortrait2.TextureSheet.Columns = 8;
            rewardPortrait2.TextureSheet.Rows = 8;

            TestData.Add(rewardPortrait2);
        }
    }
}
