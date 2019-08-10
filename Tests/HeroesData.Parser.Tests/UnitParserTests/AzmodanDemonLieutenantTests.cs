using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class AzmodanDemonLieutenantTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("Demon Lieutenant", AzmodanDemonLieutenant.Name);
            Assert.AreEqual("AzmodanDemonLieutenant", AzmodanDemonLieutenant.HyperlinkId);
            Assert.AreEqual(7, AzmodanDemonLieutenant.Sight);
            Assert.AreEqual(0.75, AzmodanDemonLieutenant.Radius);
            Assert.AreEqual(0.75, AzmodanDemonLieutenant.InnerRadius);
            Assert.AreEqual("Summon", AzmodanDemonLieutenant.DamageType);
            Assert.AreEqual("storm_ui_ingame_targetinfopanel_unit_azmodan_demonlieutenant.dds", AzmodanDemonLieutenant.UnitPortrait.TargetInfoPanelFileName);
        }

        [TestMethod]
        public void AbilitiesTests()
        {
            Ability ability1 = AzmodanDemonLieutenant.GetFirstAbility("AzmodanDemonicSmite");

            // ability button is pointed to demon lieutenant
            Assert.AreEqual("Demon Lieutenant", ability1.Name);
            Assert.AreEqual("Cooldown: 7 seconds", ability1.Tooltip.Cooldown.CooldownTooltip.PlainText);
            Assert.AreEqual(AbilityType.Hidden, ability1.AbilityType);
        }
    }
}
