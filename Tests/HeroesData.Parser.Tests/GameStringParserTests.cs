using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class GameStringParserTests
    {
        private readonly GameStringParser GameStringParser;
        private readonly GameData GameData;

        private readonly string ModsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");
        private readonly string DataReferenceText1 = "<d ref=\"100*Talent,AnubarakMasteryEpicenterBurrowCharge,AbilityModificationArray[0].Modifications[2].Value\"/>";
        private readonly string Tooltip1 = "Toxic Nests deal <c val=\"#TooltipNumbers\"><d ref=\"(Effect,AbathurToxicNestEnvenomedNestDamage,Amount* [d ref='Behavior,AbathurToxicNestEnvenomedNest,PeriodCount' player='0'/])/Effect,ToxicNestDamage,Amount*100\"/>%</c> more damage over <c val=\"#TooltipNumbers\"><d ref=\"Behavior,AbathurToxicNestEnvenomedNest,Duration\" player=\"0\"/></c> seconds.";
        private readonly string Tolltip2 = "Zarya's Basic Attack deals <c val=\"#TooltipNumbers\"><d ref=\"(Effect,ZaryaWeaponFeelTheHeatDamage,Amount/Effect,ZaryaWeaponDamage,Amount)-1*10)\" />0%</c> additional damage to enemies in melee range.";

        public GameStringParserTests()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.Load();

            GameStringParser = new GameStringParser(GameData);
        }

        [TestMethod]
        public void ParseDataReferenceStringTest()
        {
            Assert.AreEqual(60, GameStringParser.ParseDRefString(DataReferenceText1));
        }

        [TestMethod]
        public void ParseTooltips()
        {
            Assert.IsTrue(GameStringParser.TryParseRawTooltip("AbathurToxicNestEnvenomedNestTalent", Tooltip1, out string output));
            Assert.AreEqual("Toxic Nests deal <c val=\"#TooltipNumbers\">75%</c> more damage over <c val=\"#TooltipNumbers\">3</c> seconds.", output);

            Assert.IsTrue(GameStringParser.TryParseRawTooltip("ZaryaWeaponFeelTheHeatTalent", Tolltip2, out output));
            Assert.AreEqual("Zarya's Basic Attack deals <c val=\"#TooltipNumbers\">50%</c> additional damage to enemies in melee range.", output);
        }
    }
}
