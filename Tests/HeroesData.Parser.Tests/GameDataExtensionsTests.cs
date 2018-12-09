using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.UnitData.Data;
using System.IO;
using System.Reflection;
using Xunit;

namespace HeroesData.Parser.Tests
{
    public class GameDataExtensionsTests
    {
        private readonly GameData GameData;
        private readonly DefaultData DefaultData;

        private readonly string ModsTestFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "mods");

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
        private readonly string ParsedTooltip11 = "Rain a small army of Demonic Grunts down on enemies, dealing <c val=\"#TooltipNumbers\">65~~0.04~~</c> damage per impact. Grunts deal <c val=\"#TooltipNumbers\">42~~0.04~~</c> damage, have <c val=\"#TooltipNumbers\">750~~0.04~~</c> health and last up to <c val=\"#TooltipNumbers\">8</c> seconds. When Grunts die they explode, dealing <c val=\"#TooltipNumbers\">65~~0.04~~</c> damage to nearby enemies.";
        private readonly string ParsedTooltip12 = "Instead of a single shot, Big Shot fires <c val=\"#TooltipNumbers\">3</c> shots over <c val=\"#TooltipNumbers\">0.5</c> seconds. Each shot deals <c val=\"#TooltipNumbers\">50</c>% damage.";
        private readonly string ParsedTooltip13 = "Shields Tyrael for <c val=\"#TooltipNumbers\">336~~0.04~~</c> damage and nearby allies for <c val=\"#TooltipNumbers\">40%</c> as much for <c val=\"#TooltipNumbers\">4</c> seconds.";
        private readonly string ParsedTooltip14 = "Deal <c val=\"#TooltipNumbers\">172~~0.04~~</c> damage to enemies within the target area.";
        private readonly string ParsedTooltip15 = "Zarya's Basic Attack deals <c val=\"#TooltipNumbers\">50%</c> additional damage to enemies in melee range.";
        private readonly string ParsedTooltip16 = "Channel on an allied or destroyed Fort or Keep to replace it with Ragnaros's ultimate form, temporarily gaining new Abilities, having <c val=\"#TooltipNumbers\">3996~~0.04~~</c> Health that burns away over <c val=\"#TooltipNumbers\">18</c> seconds.<n/><n/>Ragnaros returns to his normal form upon losing all Health in Molten Core.";

        public GameDataExtensionsTests()
        {
            GameData = new FileGameData(ModsTestFolder);
            GameData.Load();

            DefaultData = new DefaultData(GameData);
            DefaultData.Load();
        }

        [Fact]
        public void GetParsedGameStringTests()
        {
            Assert.Equal(ParsedTooltip1, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AbathurToxicNestEnvenomedNestTalent")));
            Assert.Equal(ParsedTooltip2, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AbathurSymbioteCarapace")));
            Assert.Equal(ParsedTooltip3, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AnubarakBurrowChargeEpicenterTalent")));
            Assert.Equal(ParsedTooltip4, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AnubarakHardenCarapaceShedExoskeletonTalent")));
            Assert.Equal(ParsedTooltip5, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AnubarakNerubianArmor")));
            Assert.Equal(ParsedTooltip6, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AzmodanAllShallBurn")));
            Assert.Equal(ParsedTooltip7, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "LostVikingsVikingBribery")));
            Assert.Equal(ParsedTooltip8, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "TyraelElDruinsMightHolyGroundTalent")));
            Assert.Equal(ParsedTooltip9, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "ChenFortifyingBrewBrewmastersBalanceTalent")));
            Assert.Equal(ParsedTooltip10, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "MuradinAvatar")));
            Assert.Equal(ParsedTooltip11, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AzmodanDemonicInvasion")));
            Assert.Equal(ParsedTooltip12, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "DVaBigShotPewPewPew")));
            Assert.Equal(ParsedTooltip13, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "TyraelRighteousness")));
            Assert.Equal(ParsedTooltip14, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "DemonHunterMultishot")));
            Assert.Equal(ParsedTooltip15, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "ZaryaWeaponFeelTheHeatTalent")));
            Assert.Equal(ParsedTooltip16, GameData.GetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "RagnarosMoltenCore")));
        }

        [Fact]
        public void TryGetParsedGameStringsTests()
        {
            if (GameData.TryGetParsedGameString(DefaultData.ButtonTooltip.Replace(DefaultData.IdReplacer, "AbathurToxicNestEnvenomedNestTalent"), out string parsedText))
            {
                Assert.Equal(ParsedTooltip1, parsedText);
            }
        }
    }
}
