using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.SprayData
{
    public class SprayDataOutputBase : FileOutputTestBase<Spray>
    {
        public SprayDataOutputBase()
            : base(nameof(SprayData))
        {
        }

        protected override void SetTestData()
        {
            Spray spray = new Spray()
            {
                Name = "Carbot Li Li",
                Id = "CarbotLiLi",
                AttributeId = "CLL",
                SortName = "33CarbotLiLi",
                Rarity = Rarity.Rare,
                SearchText = "Carbot Li LI",
                Description = new TooltipDescription("Carbot spray of Li Li"),
                HyperlinkId = "CarbotLiLiId",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "HeroStorm",
                EventName = "Carbot",
                AnimationCount = 5,
                AnimationDuration = 100,
            };

            spray.TextureSheet.Image = "carbotspray.png";

            TestData.Add(spray);

            Spray spray2 = new Spray()
            {
                Name = "Carbot Chen",
                Id = "CarbotChen",
                AttributeId = "CCH",
                SortName = "33CarbotChen",
                Rarity = Rarity.Epic,
                SearchText = "Carbot Chen",
                HyperlinkId = "CarbotChenId",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "HeroStorm",
            };

            spray2.TextureSheet.Image = "carbotspray.png";
            TestData.Add(spray2);
        }
    }
}
