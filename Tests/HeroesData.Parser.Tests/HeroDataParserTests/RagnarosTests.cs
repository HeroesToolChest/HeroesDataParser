using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class RagnarosTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void HeroDescriptorsTests()
        {
            Assert.AreEqual(5, HeroRagnaros.HeroDescriptors.Count());

            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("EnergyImportant"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("Escaper"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("Overconfident"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("RoleCaster"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("WaveClearer"));
        }

        [TestMethod]
        public void RagnarosLivingMeteorMoltenPowerTalentTest()
        {
            Talent talent = HeroRagnaros.GetTalent("RagnarosLivingMeteorMoltenPower");

            Assert.AreEqual("Molten Power", talent.Name);
            Assert.AreEqual("After hitting Heroes, next Living Meteor deals increased damage", talent.Tooltip.ShortTooltip?.RawDescription);
            Assert.AreEqual("Each enemy Hero hit by Living Meteor increases the damage ", talent.Tooltip.FullTooltip?.RawDescription);
            Assert.AreEqual("storm_ui_icon_ragnaros_livingmeteor.dds", talent.IconFileName);
            Assert.AreEqual(AbilityType.W, talent.AbilityType);
        }
    }
}
