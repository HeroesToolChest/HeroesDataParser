using Heroes.Models;

namespace HeroesData.FileWriter.Tests.PortraitData
{
    public class PortraitDataOutputBase : FileOutputTestBase<Portrait>
    {
        public PortraitDataOutputBase()
            : base(nameof(PortraitData))
        {
        }

        protected override void SetTestData()
        {
            Portrait portrait = new Portrait()
            {
                Name = "Lag Force",
                Id = "LagForce",
                SortName = "xxLagForce",
                HyperlinkId = "LagForceId",
                EventName = "SunsOut",
            };

            TestData.Add(portrait);
        }
    }
}
