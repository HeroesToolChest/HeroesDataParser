using Heroes.Models;

namespace HeroesData.FileWriter.Tests.PortraitPackData
{
    public class PortraitDataOutputBase : FileOutputTestBase<PortraitPack>
    {
        public PortraitDataOutputBase()
            : base(nameof(PortraitPackData))
        {
        }

        protected override void SetTestData()
        {
            PortraitPack portrait = new PortraitPack()
            {
                Name = "Lag Force",
                Id = "LagForce",
                SortName = "xxLagForce",
                HyperlinkId = "LagForceId",
                EventName = "SunsOut",
                Rarity = Rarity.Epic,
            };

            portrait.RewardPortraitIds.Add("WhitemaneCat");
            portrait.RewardPortraitIds.Add("WhitemaneHat");
            portrait.RewardPortraitIds.Add("Kitty");

            TestData.Add(portrait);

            PortraitPack portrait2 = new PortraitPack()
            {
                Name = "00 Force",
                Id = "00Force",
                SortName = "xx00Force",
                HyperlinkId = "00ForceId",
                Rarity = Rarity.Epic,
            };

            TestData.Add(portrait2);
        }
    }
}
