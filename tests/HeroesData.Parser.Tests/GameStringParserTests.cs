using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlGameData;
using System.Collections.Generic;
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

        private int FailedParsedCount = 0;
        private Dictionary<string, string> FullParsedTooltipsByFullTooltipNameId = new Dictionary<string, string>();

        public GameStringParserTests()
        {
            LoadTestData();
            GameStringParser = new GameStringParser(GameData);

            ParseTooltips();
        }

        [Fact]
        public void AllFullParsedTooltipsTest()
        {
            Assert.True(FullParsedTooltipsByFullTooltipNameId.Count == GameStringData.FullTooltipsByFullTooltipNameId.Count);
        }

        [Fact]
        public void AllFullParsedTooltipsNoFailedTest()
        {
            Assert.True(FailedParsedCount == 0);
        }

        [Fact]
        public void FullParsedTooltipSuccessTest()
        {
            Assert.Equal("Toxic Nests deal <c val=\"#TooltipNumbers\">75%</c> more damage over <c val=\"#TooltipNumbers\">3</c> seconds.", FullParsedTooltipsByFullTooltipNameId["AbathurToxicNestEnvenomedNestTalent"]);
            Assert.Equal("Shields the assisted ally for <c val=\"#TooltipNumbers\">157~~0.04~~</c>. Lasts for <c val=\"#TooltipNumbers\">8</c> seconds.", FullParsedTooltipsByFullTooltipNameId["AbathurSymbioteCarapace"]);
            Assert.Equal("Increases Burrow Charge impact area by <c val=\"#TooltipNumbers\">60%</c> and lowers the cooldown by <c val=\"#TooltipNumbers\">1.25</c> seconds for each Hero hit.", FullParsedTooltipsByFullTooltipNameId["AnubarakBurrowChargeEpicenterTalent"]);
            Assert.Equal("Harden Carapace grants <c val=\"#TooltipNumbers\">30%</c> increased Movement Speed for <c val=\"#TooltipNumbers\">3</c> seconds.", FullParsedTooltipsByFullTooltipNameId["AnubarakHardenCarapaceShedExoskeletonTalent"]);
            Assert.Equal("Every <c val=\"#TooltipNumbers\">12</c> seconds, gain <c val=\"#TooltipNumbers\">30</c> Spell Armor against the next enemy Ability and subsequent Abilities for <c val=\"#TooltipNumbers\">1.5</c> seconds, reducing the damage taken by <c val=\"#TooltipNumbers\">30%</c>.", FullParsedTooltipsByFullTooltipNameId["AnubarakNerubianArmor"]);
            Assert.Equal("Channel a death beam on an enemy, dealing <c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second. Damage increases over time, to a max of <c val=\"#TooltipNumbers\">200~~0.04~~</c> per second, and is increased by <c val=\"#TooltipNumbers\">25~~0.04~~</c> against structures. Azmodan can move at <c val=\"#TooltipNumbers\">40%</c> speed while channeling.", FullParsedTooltipsByFullTooltipNameId["AzmodanAllShallBurn"]);
            Assert.Equal("Enemy Minions or captured Mercenaries killed near The Lost Vikings grant stacks of Bribe. Use <c val=\"#TooltipNumbers\">40</c> stacks to bribe target Mercenary, instantly defeating them. Does not work on Bosses. Maximum stacks available: <c val=\"#TooltipNumbers\">200</c>. If a camp is defeated entirely with Bribe, the camp respawns <c val=\"#TooltipNumbers\">50%</c> faster.<n/><n/><c val=\"ffff8a\">Current number of Bribe stacks: </c><c val=\"#TooltipNumbers\">0</c>", FullParsedTooltipsByFullTooltipNameId["LostVikingsVikingBribery"]);
            Assert.Equal("Create a ring for <c val=\"#TooltipNumbers\">3</c> seconds that blocks enemies from entering the area teleported to using El'druin's Might.", FullParsedTooltipsByFullTooltipNameId["TyraelElDruinsMightHolyGroundTalent"]);
            Assert.Equal("While at or below <c val=\"#TooltipNumbers\">50</c> Brew, gain <c val=\"#TooltipNumbers\">20%</c> Movement Speed. While at or above <c val=\"#TooltipNumbers\">50</c> Brew, regenerate an additional <c val=\"#TooltipNumbers\">18~~0.04~~</c> Health per second.", FullParsedTooltipsByFullTooltipNameId["ChenFortifyingBrewBrewmastersBalanceTalent"]);
            Assert.Equal("Transform for <c val=\"#TooltipNumbers\">20</c> seconds, gaining <c val=\"#TooltipNumbers\">1053~~0.04~~</c> Health.", FullParsedTooltipsByFullTooltipNameId["MuradinAvatar"]);
            Assert.Equal("Rain a small army of Demonic Grunts down on enemies, dealing <c val=\"#TooltipNumbers\">65~~0.04~~</c> damage per impact. Grunts deal <c val=\"#TooltipNumbers\">42~~0.04~~</c> damage, have <c val=\"#TooltipNumbers\">750~~0.04~~</c> health and last up to <c val=\"#TooltipNumbers\">8</c> seconds. When Grunts die they explode, dealing <c val=\"#TooltipNumbers\">65~~0.04~~</c> damage to nearby enemies.", FullParsedTooltipsByFullTooltipNameId["AzmodanDemonicInvasion"]);
            Assert.Equal("Instead of a single shot, Big Shot fires <c val=\"#TooltipNumbers\">3</c> shots over <c val=\"#TooltipNumbers\">0.5</c> seconds. Each shot deals <c val=\"#TooltipNumbers\">50</c>% damage.", FullParsedTooltipsByFullTooltipNameId["DVaBigShotPewPewPew"]);
            Assert.Equal("Shields Tyrael for <c val=\"#TooltipNumbers\">336~~0.04~~</c> damage and nearby allies for <c val=\"#TooltipNumbers\">40%</c> as much for <c val=\"#TooltipNumbers\">4</c> seconds.", FullParsedTooltipsByFullTooltipNameId["TyraelRighteousness"]);
            Assert.Equal("Deal <c val=\"#TooltipNumbers\">172~~0.04~~</c> damage to enemies within the target area.", FullParsedTooltipsByFullTooltipNameId["DemonHunterMultishot"]);
            Assert.Equal("Zarya's Basic Attack deals <c val=\"#TooltipNumbers\">50%</c> additional damage to enemies in melee range.", FullParsedTooltipsByFullTooltipNameId["ZaryaWeaponFeelTheHeatTalent"]);
            Assert.Equal("Channel on an allied or destroyed Fort or Keep to replace it with Ragnaros's ultimate form, temporarily gaining new Abilities, having <c val=\"#TooltipNumbers\">3996~~0.04~~</c> Health that burns away over <c val=\"#TooltipNumbers\">18</c> seconds.<n/><n/>Ragnaros returns to his normal form upon losing all Health in Molten Core.", FullParsedTooltipsByFullTooltipNameId["RagnarosMoltenCore"]);
        }

        [Fact]
        public void ParseDataReferenceStringTest()
        {
            Assert.Equal(60, GameStringParser.ParseDRefString(DataReferenceText1));
        }

        private void LoadTestData()
        {
            GameData = GameData.Load(ModsTestFolder);
            GameStringData = GameStringData.Load(ModsTestFolder);
        }

        private void ParseTooltips()
        {
            foreach (KeyValuePair<string, string> tooltip in GameStringData.FullTooltipsByFullTooltipNameId)
            {
                if (GameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                    FullParsedTooltipsByFullTooltipNameId.Add(tooltip.Key, parsedTooltip);
                else
                    FailedParsedCount++;
            }
        }
    }
}
