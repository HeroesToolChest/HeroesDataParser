using Heroes.Models;
using System;
using System.Collections.Generic;

namespace HeroesData.FileWriter.Tests.HeroSkinData
{
    public class HeroSkinDataOutputBase : FileOutputTestBase<HeroSkin>
    {
        public HeroSkinDataOutputBase()
            : base(nameof(HeroSkinData))
        {
        }

        protected override void SetTestData()
        {
            HeroSkin heroSkin = new HeroSkin()
            {
                Name = "Bone Abathur",
                SkinId = "AbathurBone",
                ShortName = "AbathurBone",
                SortName = "xxAbathurBone",
                Rarity = Rarity.None,
                AttributeId = "Aba1",
                Description = new TooltipDescription("Evolution Master of Kerrigan's Swarm"),
                SearchText = "White Pink",
                ReleaseDate = new DateTime(2014, 3, 13),
                Features = new List<string>() { "ThemedAbilities", "ThemedAnimations" },
            };

            TestData.Add(heroSkin);

            HeroSkin heroSkin2 = new HeroSkin()
            {
                Name = "Mecha Abathur",
                SkinId = "AbathurMechaVar1",
                ShortName = "AbathurMecha",
                SortName = "xxyAbathurMecha",
                Rarity = Rarity.Legendary,
                AttributeId = "Aba2",
                ReleaseDate = new DateTime(2014, 3, 1),
            };

            TestData.Add(heroSkin2);
        }
    }
}
