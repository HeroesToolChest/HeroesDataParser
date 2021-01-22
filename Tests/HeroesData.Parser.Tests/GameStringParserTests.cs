using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace HeroesData.Parser.Tests
{
    [TestClass]
    public class GameStringParserTests
    {
        private readonly GameData _gameData;
        private readonly DefaultData _defaultData;
        private readonly GameStringParser _gameStringParser;
        private readonly Configuration _configuration;

        private readonly string _modsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");
        private readonly string _dataReferenceText1 = "<d ref=\"100*Talent,AnubarakMasteryEpicenterBurrowCharge,AbilityModificationArray[0].Modifications[2].Value\"/>";
        private readonly string _tooltip1 = "Toxic Nests deal <c val=\"bfd4fd\"><d ref=\"(Effect,AbathurToxicNestEnvenomedNestDamage,Amount* [d ref='Behavior,AbathurToxicNestEnvenomedNest,PeriodCount' player='0'/])/Effect,ToxicNestDamage,Amount*100\"/>%</c> more damage over <c val=\"bfd4fd\"><d ref=\"Behavior,AbathurToxicNestEnvenomedNest,Duration\" player=\"0\"/></c> seconds.";
        private readonly string _tooltip2 = "Zarya's Basic Attack deals <c val=\"bfd4fd\"><d ref=\"(Effect,ZaryaWeaponFeelTheHeatDamage,Amount/Effect,ZaryaWeaponDamage,Amount)-1*10)\" />0%</c> additional damage to enemies in melee range.";
        private readonly string _tooltip3 = "Yrel sanctifies the ground around her, gaining <c val=\"#bfd4fd\"><d const=\"$YrelSacredGroundArmorBonus\" precision=\"2\"/></c> Armor until she leaves the area.";
        private readonly string _tooltip4 = "If Sand Blast travels at least <c val=\"bfd4fd\"><d ref=\"Validator,ChromieFastForwardDistanceCheck,Range/Effect,ChromieSandBlastLaunchMissile,ImpactLocation.ProjectionDistanceScale*100\"/>%</c> of its base distance and hits a Hero, its cooldown is reduced to <c val=\"bfd4fd\"><d ref=\"Effect,ChromieSandBlastFastForwardCooldownReduction,Cost[0].CooldownTimeUse\" precision=\"2\"/></c> seconds.";
        private readonly string _tooltip5 = "After <c val=\"#TooltipNumbers\"><d ref=\"Abil,GuldanHorrify,CastIntroTime+Effect,GuldanHorrifyAbilityStartCreatePersistent,PeriodicPeriodArray[0]\" precision=\"2\"/></c> seconds..";
        private readonly string _tooltip6 = "reduces its cooldown by <c val=\"#TooltipNumbers\"><d ref=\"(1-Effect,AnduinHolyWordSalvationLightOfStormwindCooldownReduction,Cost[0].CooldownTimeUse)*-1\"/></c>";

        private readonly string _failedTooltip1 = "Surround Yrel in a barrier for <c val=\"bfd4fd\"><d const=\"$NotExistingVariable\" precision=\"2\"/></c> seconds, absorbing all damage taken and healing her for <c val=\"bfd4fd\"><d ref=\"Effect,YrelArdentDefenderDamageConversionScaleDummyModifyUnit,XP*100\" player=\"0\" precision=\"2\"/>%</c> of the damage received.";

        private readonly string _exceptionTooltip1 = "Fire a laser that deals <d ref=\"Shield,LaserCannon,Amount\"/>.";
        private readonly string _exceptionTooltip2 = "Fire a laser that deals <d ref=\"Behavior,LaserCannon,Amount\"/>.";

        private readonly string _parsedTooltip1 = "Toxic Nests deal <c val=\"#TooltipNumbers\">75%</c> more damage over <c val=\"#TooltipNumbers\">3</c> seconds.";
        private readonly string _parsedTooltip2 = "Shields the assisted ally for <c val=\"#TooltipNumbers\">150~~0.04~~</c>. Lasts for <c val=\"#TooltipNumbers\">6</c> seconds.";
        private readonly string _parsedTooltip3 = "Increases Burrow Charge impact area by <c val=\"#TooltipNumbers\">60%</c> and lowers the cooldown by <c val=\"#TooltipNumbers\">1.25</c> seconds for each Hero hit.";
        private readonly string _parsedTooltip4 = "Harden Carapace grants <c val=\"#TooltipNumbers\">30%</c> increased Movement Speed for <c val=\"#TooltipNumbers\">3</c> seconds.";
        private readonly string _parsedTooltip5 = "Increase Hardened Carapace's Spell Armor by <c val=\"#TooltipNumbers\">20</c>.";
        private readonly string _parsedTooltip6 = "Channel a death beam on an enemy, dealing <c val=\"#TooltipNumbers\">100~~0.04~~</c> damage per second. Damage increases over time, to a max of <c val=\"#TooltipNumbers\">200~~0.04~~</c> per second, and is increased by <c val=\"#TooltipNumbers\">25~~0.04~~</c> against structures. Azmodan can move at <c val=\"#TooltipNumbers\">40%</c> speed while channeling.";
        private readonly string _parsedTooltip7 = "Enemy Minions or captured Mercenaries killed near The Lost Vikings grant stacks of Bribe. Use <c val=\"#TooltipNumbers\">40</c> stacks to bribe target Mercenary, instantly defeating them. Does not work on Bosses. Maximum stacks available: <c val=\"#TooltipNumbers\">200</c>. If a camp is defeated entirely with Bribe, the camp respawns <c val=\"#TooltipNumbers\">50%</c> faster.<n/><n/><c val=\"ffff8a\">Current number of Bribe stacks: </c><c val=\"#TooltipNumbers\">0</c>";
        private readonly string _parsedTooltip8 = "Create a ring for <c val=\"#TooltipNumbers\">3</c> seconds that blocks enemies from entering the area teleported to using El'druin's Might.";
        private readonly string _parsedTooltip9 = "While at or below <c val=\"#TooltipNumbers\">50</c> Brew, gain <c val=\"#TooltipNumbers\">20%</c> Movement Speed. While at or above <c val=\"#TooltipNumbers\">50</c> Brew, regenerate an additional <c val=\"#TooltipNumbers\">18~~0.04~~</c> Health per second.";
        private readonly string _parsedTooltip10 = "Transform for <c val=\"#TooltipNumbers\">20</c> seconds, gaining <c val=\"#TooltipNumbers\">1053~~0.04~~</c> Health.";
        private readonly string _parsedTooltip11 = "Rain a small army of Demonic Grunts down on enemies, dealing <c val=\"#TooltipNumbers\">65~~0.04~~</c> damage per impact. Grunts deal <c val=\"#TooltipNumbers\">42~~0.04~~</c> damage, have <c val=\"#TooltipNumbers\">750~~0.04~~</c> Health and last up to <c val=\"#TooltipNumbers\">10</c> seconds. When Grunts die they explode, dealing <c val=\"#TooltipNumbers\">98~~0.04~~</c> damage to nearby enemies.<n/><n/>Usable while Channeling All Shall Burn.";
        private readonly string _parsedTooltip12 = "Instead of a single shot, Big Shot fires <c val=\"#TooltipNumbers\">3</c> shots over <c val=\"#TooltipNumbers\">0.5</c> seconds. Each shot deals <c val=\"#TooltipNumbers\">50</c>% damage.";
        private readonly string _parsedTooltip13 = "Shields Tyrael for <c val=\"#TooltipNumbers\">336~~0.04~~</c> damage and nearby allies for <c val=\"#TooltipNumbers\">40%</c> as much for <c val=\"#TooltipNumbers\">4</c> seconds.";
        private readonly string _parsedTooltip14 = "Deal <c val=\"#TooltipNumbers\">172~~0.04~~</c> damage to enemies within the target area.";
        private readonly string _parsedTooltip15 = "Zarya's Basic Attack deals <c val=\"#TooltipNumbers\">50%</c> additional damage to enemies in melee range.";
        private readonly string _parsedTooltip16 = "Channel on an allied or destroyed Fort or Keep to replace it with Ragnaros's ultimate form, temporarily gaining new Abilities, having <c val=\"#TooltipNumbers\">3996~~0.04~~</c> Health that burns away over <c val=\"#TooltipNumbers\">18</c> seconds.<n/><n/>Ragnaros returns to his normal form upon losing all Health in Molten Core.";
        private readonly string _parsedTooltip17 = "Globe of Annihilation deals <c val=\"#TooltipNumbers\">20%</c> more damage to non-Heroic targets.<n/><n/><img path=\"@UI/StormTalentInTextQuestIcon\" alignment=\"uppermiddle\" color=\"B48E4C\" width=\"20\" height=\"22\"/><c val=\"e4b800\">Quest:</c> After gaining <c val=\"#TooltipNumbers\">200</c> Annihilation, increase the range of All Shall Burn by <c val=\"#TooltipNumbers\">25%</c> and Demon Warriors gain <c val=\"#TooltipNumbers\">20%</c> Attack Speed and Movement Speed.";
        private readonly string _parsedTooltip18 = "Eject from the Mech, setting it to self-destruct after <c val=\"#TooltipNumbers\">4</c> seconds. Deals <c val=\"#TooltipNumbers\">1200~~0.04~~</c> to <c val=\"#TooltipNumbers\">400~~0.04~~</c> damage in a large area, depending on distance from center. Only deals <c val=\"#TooltipNumbers\">50%</c> damage against Structures.<n/><n/><c val=\"FF8000\">Gain </c><c val=\"#TooltipNumbers\">1%</c><c val=\"FF8000\"> Charge for every </c><c val=\"#TooltipNumbers\">2</c><c val=\"FF8000\"> seconds spent Basic Attacking, and </c><c val=\"#TooltipNumbers\">30%</c><c val=\"FF8000\"> Charge per </c><c val=\"#TooltipNumbers\">100%</c><c val=\"FF8000\"> of Mech Health lost.</c>";
        private readonly string _parsedTooltip19 = "Increase the damage of Octo-Grab by <c val=\"#TooltipNumbers\">13700%~~0.04~~</c>.";

        public GameStringParserTests()
        {
            _gameData = new FileGameData(_modsTestFolder);
            _gameData.LoadAllData();

            _configuration = new Configuration();
            _configuration.Load();

            _gameStringParser = new GameStringParser(_configuration, _gameData);
            PreParse();

            _defaultData = new DefaultData(_gameData);
            _defaultData.Load();
        }

        [TestMethod]
        public void ParseDataReferenceStringTest()
        {
            Assert.AreEqual(60, _gameStringParser.ParseDRefString(_dataReferenceText1));
        }

        [TestMethod]
        public void ParseTooltips()
        {
            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("AbathurToxicNestEnvenomedNestTalent", _tooltip1, out string output));
            Assert.AreEqual("Toxic Nests deal <c val=\"bfd4fd\">75%</c> more damage over <c val=\"bfd4fd\">3</c> seconds.", output);

            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("ZaryaWeaponFeelTheHeatTalent", _tooltip2, out output));
            Assert.AreEqual("Zarya's Basic Attack deals <c val=\"bfd4fd\">50%</c> additional damage to enemies in melee range.", output);

            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("YrelArdentDefender", _failedTooltip1, out output));
            Assert.AreEqual("Surround Yrel in a barrier for <c val=\"bfd4fd\">0##ERROR##</c> seconds, absorbing all damage taken and healing her for <c val=\"bfd4fd\">50%</c> of the damage received.", output);

            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("YrelSacredGround", _tooltip3, out output));
            Assert.AreEqual("Yrel sanctifies the ground around her, gaining <c val=\"bfd4fd\">50</c> Armor until she leaves the area.", output);

            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("ChromieSandBlastFastForward", _tooltip4, out output));
            Assert.AreEqual("If Sand Blast travels at least <c val=\"bfd4fd\">50%</c> of its base distance and hits a Hero, its cooldown is reduced to <c val=\"bfd4fd\">0.5</c> seconds.", output);

            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("GuldanHorrify", _tooltip5, out output));
            Assert.AreEqual("After <c val=\"#TooltipNumbers\">0.5</c> seconds..", output);

            Assert.IsTrue(_gameStringParser.TryParseRawTooltip("AnduinHolyWordSalvationLightOfStormwind", _tooltip6, out output));
            Assert.AreEqual("reduces its cooldown by <c val=\"#TooltipNumbers\">60</c>", output);
        }

        [TestMethod]
        public void GetParsedGameStringTests()
        {
            Assert.AreEqual(_parsedTooltip1, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AbathurToxicNestEnvenomedNestTalent", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip2, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AbathurSymbioteCarapace", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip3, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AnubarakBurrowChargeEpicenterTalent", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip4, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AnubarakHardenCarapaceShedExoskeletonTalent", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip5, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AnubarakNerubianArmor", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip6, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AzmodanAllShallBurn", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip7, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "LostVikingsVikingBribery", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip8, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "TyraelElDruinsMightHolyGroundTalent", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip9, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "ChenFortifyingBrewBrewmastersBalanceTalent", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip10, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "MuradinAvatar", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip11, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AzmodanDemonicInvasion", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip12, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "DVaBigShotPewPewPew", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip13, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "TyraelRighteousness", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip14, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "DemonHunterMultishot", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip15, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "ZaryaWeaponFeelTheHeatTalent", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip16, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "RagnarosMoltenCore", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip17, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "AzmodanGreed", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip18, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "DVaMechSelfDestruct", StringComparison.OrdinalIgnoreCase)));
            Assert.AreEqual(_parsedTooltip19, _gameData.GetGameString(_defaultData.ButtonData.ButtonTooltip.Replace(DefaultData.IdPlaceHolder, "MurkyOctoGrabAndASharkTooTalent", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void ExceptionParsingTests()
        {
            Assert.ThrowsException<GameStringParseException>(() =>
            {
                _gameStringParser.TryParseRawTooltip("LaserFighter", _exceptionTooltip1, out string output);
            });

            Assert.ThrowsException<GameStringParseException>(() =>
            {
                _gameStringParser.TryParseRawTooltip("LaserFighter", _exceptionTooltip2, out string output);
            });
        }

        private void PreParse()
        {
            foreach (string id in _gameData.GameStringIds)
            {
                if (_gameStringParser.TryParseRawTooltip(id, _gameData.GetGameString(id), out string parsedGamestring))
                    _gameData.AddGameString(id, parsedGamestring);
            }
        }
    }
}
