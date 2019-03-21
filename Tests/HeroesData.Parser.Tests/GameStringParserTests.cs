using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class GameStringParserTests
    {
        private readonly GameData GameData;
        private readonly DefaultData DefaultData;
        private readonly GameStringParser GameStringParser;

        private readonly string ModsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");
        private readonly string DataReferenceText1 = "<d ref=\"100*Talent,AnubarakMasteryEpicenterBurrowCharge,AbilityModificationArray[0].Modifications[2].Value\"/>";
        private readonly string Tooltip1 = "Toxic Nests deal <c val=\"#TooltipNumbers\"><d ref=\"(Effect,AbathurToxicNestEnvenomedNestDamage,Amount* [d ref='Behavior,AbathurToxicNestEnvenomedNest,PeriodCount' player='0'/])/Effect,ToxicNestDamage,Amount*100\"/>%</c> more damage over <c val=\"#TooltipNumbers\"><d ref=\"Behavior,AbathurToxicNestEnvenomedNest,Duration\" player=\"0\"/></c> seconds.";
        private readonly string Tooltip2 = "Zarya's Basic Attack deals <c val=\"#TooltipNumbers\"><d ref=\"(Effect,ZaryaWeaponFeelTheHeatDamage,Amount/Effect,ZaryaWeaponDamage,Amount)-1*10)\" />0%</c> additional damage to enemies in melee range.";

        private readonly string ParsedTooltip1 = "Toxic Nests deal <c val=\"#TooltipNumbers\">75%</c> more damage over <c val=\"#TooltipNumbers\">3</c> seconds.";
        private readonly string ParsedTooltip2 = "Shields the assisted ally for <c val=\"#TooltipNumbers\">157~~0.04~~</c>. Lasts for <c val=\"#TooltipNumbers\">8</c> seconds.";
        private readonly string ParsedTooltip3 = "Increases Burrow Charge impact area by <c val=\"#TooltipNumbers\">60%</c> and lowers the cooldown by <c val=\"#TooltipNumbers\">1.25</c> seconds for each Hero hit.";
        private readonly string ParsedTooltip4 = "Harden Carapace grants <c val=\"#TooltipNumbers\">30%</c> increased Movement Speed for <c val=\"#TooltipNumbers\">3</c> seconds.";
        private readonly string ParsedTooltip5 = "Every <c val=\"#TooltipNumbers\">12</c> seconds, gain <c val=\"#TooltipNumbers\">30</c> Spell Armor against the next enemy Ability and subsequent Abilities for <c val=\"#TooltipNumbers\">1.5</c> seconds, reducing the damage taken by <c val=\"#TooltipNumbers\">30%</c>.";
        private readonly string ParsedTooltip6 = "Channel a death beam on an enemy, dealing <c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second. Damage increases over time, to a max of <c val=\"#TooltipNumbers\">200~~0.04~~</c> per second, and is increased by <c val=\"#TooltipNumbers\">25~~0.04~~</c> against structures. Azmodan can move at <c val=\"#TooltipNumbers\">40%</c> speed while channeling.";
        private readonly string ParsedTooltip7 = "Enemy Minions or captured Mercenaries killed near The Lost Vikings grant stacks of Bribe. Use <c val=\"#TooltipNumbers\">40</c> stacks to bribe target Mercenary, instantly defeating them. Does not work on Bosses. Maximum stacks available: <c val=\"#TooltipNumbers\">200</c>. If a camp is defeated entirely with Bribe, the camp respawns <c val=\"#TooltipNumbers\">50%</c> faster.<n/><n/><c val=\"ffff8a\">Current number of Bribe stacks: </c><c val=\"#TooltipNumbers\">0</c>";
        private readonly string ParsedTooltip8 = "Create a ring for <c val=\"#TooltipNumbers\">3</c> seconds that blocks enemies from entering the area teleported to using El'druin's Might.";
        private readonly string ParsedTooltip9 = "While at or below <c val=\"#TooltipNumbers\">50</c> Brew, gain <c val=\"#TooltipNumbers\">20%</c> Movement Speed. While at or above <c val=\"#TooltipNumbers\">50</c> Brew, regenerate an additional <c val=\"#TooltipNumbers\">18~~0.04~~</c> Health per second.";
        private readonly string ParsedTooltip10 = "Transform for <c val=\"#TooltipNumbers\">20</c> seconds, gaining <c val=\"#TooltipNumbers\">1053~~0.04~~</c> Health.";
        private readonly string ParsedTooltip11 = "Rain a small army of Demonic Grunts down on enemies, dealing <c val=\"#TooltipNumbers\">65~~0.04~~</c> damage per impact. Grunts deal <c val=\"#TooltipNumbers\">42~~0.04~~</c> damage, have <c val=\"#TooltipNumbers\">750~~0.04~~</c> Health and last up to <c val=\"#TooltipNumbers\">10</c> seconds. When Grunts die they explode, dealing <c val=\"#TooltipNumbers\">98~~0.04~~</c> damage to nearby enemies.<n/><n/>Usable while Channeling All Shall Burn.";
        private readonly string ParsedTooltip12 = "Instead of a single shot, Big Shot fires <c val=\"#TooltipNumbers\">3</c> shots over <c val=\"#TooltipNumbers\">0.5</c> seconds. Each shot deals <c val=\"#TooltipNumbers\">50</c>% damage.";
        private readonly string ParsedTooltip13 = "Shields Tyrael for <c val=\"#TooltipNumbers\">336~~0.04~~</c> damage and nearby allies for <c val=\"#TooltipNumbers\">40%</c> as much for <c val=\"#TooltipNumbers\">4</c> seconds.";
        private readonly string ParsedTooltip14 = "Deal <c val=\"#TooltipNumbers\">172~~0.04~~</c> damage to enemies within the target area.";
        private readonly string ParsedTooltip15 = "Zarya's Basic Attack deals <c val=\"#TooltipNumbers\">50%</c> additional damage to enemies in melee range.";
        private readonly string ParsedTooltip16 = "Channel on an allied or destroyed Fort or Keep to replace it with Ragnaros's ultimate form, temporarily gaining new Abilities, having <c val=\"#TooltipNumbers\">3996~~0.04~~</c> Health that burns away over <c val=\"#TooltipNumbers\">18</c> seconds.<n/><n/>Ragnaros returns to his normal form upon losing all Health in Molten Core.";
        private readonly string ParsedTooltip17 = "Globe of Annihilation deals <c val=\"#TooltipNumbers\">20%</c> more damage to non-Heroic targets.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"#TooltipQuest\">Quest:</c> After gaining <c val=\"#TooltipNumbers\">200</c> Annihilation, increase the range of All Shall Burn by <c val=\"#TooltipNumbers\">25%</c> and Demon Warriors gain <c val=\"#TooltipNumbers\">20%</c> Attack Speed and Movement Speed.";
        private readonly string ParsedTooltip18 = "Eject from the Mech, setting it to self-destruct after <c val=\"#TooltipNumbers\">4</c> seconds. Deals <c val=\"#TooltipNumbers\">1200</c> to <c val=\"#TooltipNumbers\">400</c> damage in a large area, depending on distance from center. Only deals <c val=\"#TooltipNumbers\">50%</c> damage against Structures.<n/><n/><c val=\"FF8000\">Gain </c><c val=\"#TooltipNumbers\">1%</c><c val=\"FF8000\"> Charge for every </c><c val=\"#TooltipNumbers\">2</c><c val=\"FF8000\"> seconds spent Basic Attacking, and </c><c val=\"#TooltipNumbers\">30%</c><c val=\"FF8000\"> Charge per </c><c val=\"#TooltipNumbers\">100%</c><c val=\"FF8000\"> of Mech Health lost.</c>";

        public GameStringParserTests()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.LoadAllData();

            GameStringParser = new GameStringParser(GameData);
            PreParse();

            DefaultData = new DefaultData(GameData);
            DefaultData.Load();
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

            Assert.IsTrue(GameStringParser.TryParseRawTooltip("ZaryaWeaponFeelTheHeatTalent", Tooltip2, out output));
            Assert.AreEqual("Zarya's Basic Attack deals <c val=\"#TooltipNumbers\">50%</c> additional damage to enemies in melee range.", output);
        }

        [TestMethod]
        public void GetParsedGameStringTests()
        {
            Assert.AreEqual(ParsedTooltip1, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AbathurToxicNestEnvenomedNestTalent")));
            Assert.AreEqual(ParsedTooltip2, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AbathurSymbioteCarapace")));
            Assert.AreEqual(ParsedTooltip3, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AnubarakBurrowChargeEpicenterTalent")));
            Assert.AreEqual(ParsedTooltip4, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AnubarakHardenCarapaceShedExoskeletonTalent")));
            Assert.AreEqual(ParsedTooltip5, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AnubarakNerubianArmor")));
            Assert.AreEqual(ParsedTooltip6, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AzmodanAllShallBurn")));
            Assert.AreEqual(ParsedTooltip7, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "LostVikingsVikingBribery")));
            Assert.AreEqual(ParsedTooltip8, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "TyraelElDruinsMightHolyGroundTalent")));
            Assert.AreEqual(ParsedTooltip9, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "ChenFortifyingBrewBrewmastersBalanceTalent")));
            Assert.AreEqual(ParsedTooltip10, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "MuradinAvatar")));
            Assert.AreEqual(ParsedTooltip11, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AzmodanDemonicInvasion")));
            Assert.AreEqual(ParsedTooltip12, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "DVaBigShotPewPewPew")));
            Assert.AreEqual(ParsedTooltip13, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "TyraelRighteousness")));
            Assert.AreEqual(ParsedTooltip14, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "DemonHunterMultishot")));
            Assert.AreEqual(ParsedTooltip15, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "ZaryaWeaponFeelTheHeatTalent")));
            Assert.AreEqual(ParsedTooltip16, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "RagnarosMoltenCore")));
            Assert.AreEqual(ParsedTooltip17, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AzmodanGreed")));
            Assert.AreEqual(ParsedTooltip18, GameData.GetGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "DVaMechSelfDestruct")));
        }

        private void PreParse()
        {
            foreach (string id in GameData.GetGameStringIds())
            {
                if (GameStringParser.TryParseRawTooltip(id, GameData.GetGameString(id), out string parsedGamestring))
                    GameData.AddGameString(id, parsedGamestring);
            }
        }
    }
}
