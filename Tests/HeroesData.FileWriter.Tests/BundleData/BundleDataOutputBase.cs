using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.BundleData
{
    public class BundleDataOutputBase : FileOutputTestBase<Bundle>
    {
        public BundleDataOutputBase()
            : base(nameof(BundleData))
        {
        }

        protected override void SetTestData()
        {
            Bundle bundle = new Bundle()
            {
                BoostBonusId = "boos7day",
                EventName = "event1",
                Franchise = HeroFranchise.Overwatch,
                GemsBonus = 45,
                GoldBonus = 33,
                HyperlinkId = "bundle1",
                Id = "bundel1",
                ImageFileName = "some_image_name.png",
                Name = "Bundel one",
                ReleaseDate = new DateTime(2012, 3, 3),
                SortName = "xxBundle1",
            };

            bundle.HeroIds.Add("hero1");
            bundle.HeroIds.Add("hero2");
            bundle.HeroIds.Add("hero3");

            bundle.AddHeroSkin("hero1", "skin1");
            bundle.AddHeroSkin("hero1", "skin2");
            bundle.AddHeroSkin("hero1", "skin3");
            bundle.AddHeroSkin("hero2", "skin5");
            bundle.AddHeroSkin("hero2", "skin6");
            bundle.AddHeroSkin("hero2", "skin7");

            bundle.MountIds.Add("mount1");
            bundle.MountIds.Add("mount2");
            bundle.MountIds.Add("mount3");

            TestData.Add(bundle);

            Bundle bundle2 = new Bundle()
            {
                Id = "winterBundle",
                SortName = "xxwinterBundle",
                HyperlinkId = "winterbundle2",
                IsDynamicContext = true,
            };

            TestData.Add(bundle2);
        }
    }
}
