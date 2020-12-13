using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.MountData
{
    public class MountDataOutputBase : FileOutputTestBase<Mount>
    {
        public MountDataOutputBase()
            : base(nameof(MountData))
        {
        }

        protected override void SetTestData()
        {
            Mount mount = new Mount()
            {
                Name = "Cloud Nine",
                Id = "CloudNine",
                AttributeId = "CN9",
                SortName = "99CloudeNine",
                Rarity = Rarity.Rare,
                SearchText = "Blue",
                InfoText = new TooltipDescription("A floating cloud"),
                HyperlinkId = "CloudNine",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "Magical",
                MountCategory = "Ride",
                EventName = "WhimsyDale",
                Franchise = Franchise.Classic,
            };

            mount.VariationMountIds.Add("var1");
            mount.VariationMountIds.Add("var2");
            mount.VariationMountIds.Add("var3");

            TestData.Add(mount);

            Mount mount2 = new Mount()
            {
                Name = "Magic Carpet",
                Id = "MagixCarpet",
                AttributeId = "MC4",
                SortName = "xxMagixCarpet",
                Rarity = Rarity.Epic,
                SearchText = "Blue",
                InfoText = new TooltipDescription("A flying magic carpet"),
                HyperlinkId = "MagicCarpet",
                ReleaseDate = new DateTime(2018, 4, 1),
                CollectionCategory = "Magical",
                MountCategory = "Ride",
                Franchise = Franchise.Unknown,
            };

            TestData.Add(mount2);
        }
    }
}
