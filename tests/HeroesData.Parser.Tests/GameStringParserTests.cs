using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlGameData;
using System.IO;
using Xunit;

namespace HeroesData.Parser.Tests
{
    public class GameStringParserTests
    {
        private const string TestDataFolder = "TestData";
        private readonly string ModsTestFolder = Path.Combine(TestDataFolder, "mods");
        private readonly string DataReferenceText1 = "<d ref=\"100*Talent,AnubarakMasteryEpicenterBurrowCharge,AbilityModificationArray[0].Modifications[2].Value\"/>";
        private readonly GameStringParser GameStringParser;

        private GameData GameData;
        private GameStringData GameStringData;

        public GameStringParserTests()
        {
            LoadTestData();
            GameStringParser = GameStringParser.ParseGameStrings(GameData, GameStringData);
        }

        [Fact]
        public void AllFullParsedTooltipsTest()
        {
            Assert.True(GameStringParser.FullParsedTooltipsByFullTooltipNameId.Count == GameStringData.FullTooltipsByFullTooltipNameId.Count);
        }

        [Fact]
        public void AllFullParsedTooltipsNoFailedTest()
        {
            Assert.True(GameStringParser.InvalidFullTooltipsByFullTooltipNameId.Count == 0);
        }

        [Fact]
        public void FullParsedTooltipSuccessTest()
        {
            Assert.Equal("Toxic Nests deal <c val=\"#TooltipNumbers\">75%</c> more damage over <c val=\"#TooltipNumbers\">3</c> seconds.", GameStringParser.FullParsedTooltipsByFullTooltipNameId["AbathurToxicNestEnvenomedNestTalent"]);
            Assert.Equal("Increases Burrow Charge impact area by <c val=\"#TooltipNumbers\">60%</c> and lowers the cooldown by <c val=\"#TooltipNumbers\">1.25</c> seconds for each Hero hit.", GameStringParser.FullParsedTooltipsByFullTooltipNameId["AnubarakBurrowChargeEpicenterTalent"]);
            Assert.Equal("Harden Carapace grants <c val=\"#TooltipNumbers\">30%</c> increased Movement Speed for <c val=\"#TooltipNumbers\">3</c> seconds.", GameStringParser.FullParsedTooltipsByFullTooltipNameId["AnubarakHardenCarapaceShedExoskeletonTalent"]);
            Assert.Equal("Every <c val=\"#TooltipNumbers\">12</c> seconds, gain <c val=\"#TooltipNumbers\">30</c> Spell Armor against the next enemy Ability and subsequent Abilities for <c val=\"#TooltipNumbers\">1.5</c> seconds, reducing the damage taken by <c val=\"#TooltipNumbers\">30%</c>.", GameStringParser.FullParsedTooltipsByFullTooltipNameId["AnubarakNerubianArmor"]);
            Assert.Equal("Channel a death beam on an enemy, dealing <c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second. Damage increases over time, to a max of <c val=\"#TooltipNumbers\">200~~0.04~~</c> per second, and is increased by <c val=\"#TooltipNumbers\">25~~0.04~~</c> against structures. Azmodan can move at <c val=\"#TooltipNumbers\">40%</c> speed while channeling.", GameStringParser.FullParsedTooltipsByFullTooltipNameId["AzmodanAllShallBurn"]);
            Assert.Equal("Enemy Minions or captured Mercenaries killed near The Lost Vikings grant stacks of Bribe. Use <c val=\"#TooltipNumbers\">40</c> stacks to bribe target Mercenary, instantly defeating them. Does not work on Bosses. Maximum stacks available: <c val=\"#TooltipNumbers\">200</c>. If a camp is defeated entirely with Bribe, the camp respawns <c val=\"#TooltipNumbers\">50%</c> faster.<n/><n/><c val=\"ffff8a\">Current number of Bribe stacks: </c><c val=\"#TooltipNumbers\">0</c>", GameStringParser.FullParsedTooltipsByFullTooltipNameId["LostVikingsVikingBribery"]);
            Assert.Equal("Create a ring for <c val=\"#TooltipNumbers\">3</c> seconds that blocks enemies from entering the area teleported to using El'druin's Might.", GameStringParser.FullParsedTooltipsByFullTooltipNameId["TyraelElDruinsMightHolyGroundTalent"]);
        }

        [Fact]
        public void ParseDataReferenceStringTest()
        {
            Assert.Equal(60, GameStringParser.ParseDRefString(GameData, DataReferenceText1));
        }

        private void LoadTestData()
        {
            GameData = GameData.Load(ModsTestFolder);
            GameStringData = GameStringData.Load(ModsTestFolder);
        }
    }
}
