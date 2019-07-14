using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.BannerData
{
    public class BannerDataOutputBase : FileOutputTestBase<Banner>
    {
        public BannerDataOutputBase()
            : base(nameof(BannerData))
        {
        }

        protected override void SetTestData()
        {
            Banner banner = new Banner()
            {
                Name = "Wizard Warbanner",
                Id = "WizardWarbanner",
                AttributeId = "WWZ",
                SortName = "22WizardWarbanner",
                Rarity = Rarity.Rare,
                Description = new TooltipDescription("Magically infused warbanner"),
                HyperlinkId = "WizardWarbannerId",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "Warcraft",
                EventName = "Wizards",
            };

            TestData.Add(banner);

            Banner banner2 = new Banner()
            {
                Name = "Orc Warbanner",
                Id = "OrcWarbanner",
                AttributeId = "OWO",
                SortName = "11OrcWarbanner",
                Rarity = Rarity.Legendary,
                Description = new TooltipDescription("Orc infused warbanner"),
                HyperlinkId = "OrcWarbannerId",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "Warcraft",
            };

            TestData.Add(banner2);
        }
    }
}
