using Heroes.Models;
using System;

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
                Id = "AbathurBone",
                HyperlinkId = "AbathurBone",
                SortName = "xxAbathurBone",
                Rarity = Rarity.None,
                AttributeId = "Aba1",
                InfoText = new TooltipDescription("Evolution Master of Kerrigan's Swarm"),
                SearchText = "White Pink",
                ReleaseDate = new DateTime(2014, 3, 13),
                Franchise = Franchise.Nexus,
            };

            heroSkin.VariationSkinIds.Add("var1");
            heroSkin.VariationSkinIds.Add("var2");
            heroSkin.VariationSkinIds.Add("var3");

            heroSkin.Features.Add("ThemedAbilities");
            heroSkin.Features.Add("ThemedAbilities");
            heroSkin.Features.Add("ThemedAnimations");

            TestData.Add(heroSkin);

            HeroSkin heroSkin2 = new HeroSkin()
            {
                Name = "Mecha Abathur",
                Id = "AbathurMechaVar1",
                HyperlinkId = "AbathurMecha",
                SortName = "xxyAbathurMecha",
                Rarity = Rarity.Legendary,
                AttributeId = "Aba2",
                ReleaseDate = new DateTime(2014, 3, 1),
            };

            TestData.Add(heroSkin2);
        }
    }
}
