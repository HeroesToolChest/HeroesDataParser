using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    [TestClass]
    public class SonyaTests : HeroDataBaseTest
    {
        [TestMethod]
        public void AbilityTests()
        {
            Ability ability = HeroSonya.Abilities["BarbarianSeismicSlam"];
            Assert.AreEqual("<s val=\"StandardTooltipDetails\">Fury: 25</s>", ability.Tooltip.Energy.EnergyTooltip.RawDescription);
        }
    }
}
