using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.BoostData
{
    public class BoostDataOutputBase : FileOutputTestBase<Boost>
    {
        public BoostDataOutputBase()
            : base(nameof(BoostData))
        {
        }

        protected override void SetTestData()
        {
            Boost boost = new Boost()
            {
                EventName = "event1",
                HyperlinkId = "boost1",
                Id = "boost1",
                Name = "boost one",
                ReleaseDate = new DateTime(2012, 3, 3),
                SortName = "xxBoost1",
            };

            TestData.Add(boost);

            Boost boost2 = new Boost()
            {
                Id = "winterBoost",
                SortName = "xxwinterBoost",
                HyperlinkId = "winterboost2",
            };

            TestData.Add(boost2);
        }
    }
}
