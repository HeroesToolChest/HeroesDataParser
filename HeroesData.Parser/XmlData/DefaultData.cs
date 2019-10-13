using HeroesData.Loader.XmlGameData;

namespace HeroesData.Parser.XmlData
{
    public class DefaultData
    {
        /// <summary>
        /// Id to be replaced in some strings.
        /// </summary>
        public const string IdPlaceHolder = "##id##";

        /// <summary>
        /// HeroId to be replaced in some strings.
        /// </summary>
        public const string HeroIdPlaceHolder = "##heroid##";

        public const string ReplacementCharacter = "%1";

        public const string StringRanged = "e_gameUIStringRanged";
        public const string StringMelee = "e_gameUIStringMelee";
        public const string StringChargeCooldownColon = "e_gameUIStringChargeCooldownColon";
        public const string StringCooldownColon = "e_gameUIStringCooldownColon";
        public const string AbilTooltipCooldownText = "UI/AbilTooltipCooldown";
        public const string AbilTooltipCooldownPluralText = "UI/AbilTooltipCooldownPlural";
        public const string MatchAwardMapSpecificInstanceNamePrefix = "UserData/EndOfMatchMapSpecificAward/";
        public const string HeroEnergyTypeManaText = "UI/HeroEnergyType/Mana";

        public const string DefaultHeroDifficulty = "Easy";

        public const string AbilMountLinkId = "Mount";

        private readonly GameData GameData;

        public DefaultData(GameData gameData)
        {
            GameData = gameData;
        }

        public DefaultDataHero? HeroData { get; private set; }

        public DefaultDataUnit? UnitData { get; private set; }

        public DefaultDataAbil? AbilData { get; private set; }

        public DefaultDataButton? ButtonData { get; private set; }

        public DefaultDataWeapon? WeaponData { get; private set; }

        public DefaultDataHeroSkin? HeroSkinData { get; private set; }

        public DefaultDataMount? MountData { get; private set; }

        public DefaultDataBanner? BannerData { get; private set; }

        public DefaultDataSpray? SprayData { get; private set; }

        public DefaultDataAnnouncer? AnnouncerData { get; private set; }

        public DefaultDataVoiceLine? VoiceLineData { get; private set; }

        public DefaultDataPortraitPack? PortraitPackData { get; private set; }

        public DefaultDataEmoticon? EmoticonData { get; private set; }

        public DefaultDataEmoticonPack? EmoticonPackData { get; private set; }

        public DefaultDataBehaviorVeterancy? BehaviorVeterancyData { get; private set; }

        /// <summary>
        /// Gets the default difficulty text. Contains ##id##.
        /// </summary>
        public string Difficulty { get; } = $"UI/HeroUtil/Difficulty/{IdPlaceHolder}";

        /// <summary>
        /// Load all default data.
        /// </summary>
        /// <remarks>Order is important.</remarks>
        public void Load()
        {
            if (GameData == null)
                return;

            HeroData = new DefaultDataHero(GameData);
            UnitData = new DefaultDataUnit(GameData);
            AbilData = new DefaultDataAbil(GameData);

            ButtonData = new DefaultDataButton(GameData);
            WeaponData = new DefaultDataWeapon(GameData);

            HeroSkinData = new DefaultDataHeroSkin(GameData);
            MountData = new DefaultDataMount(GameData);
            BannerData = new DefaultDataBanner(GameData);
            SprayData = new DefaultDataSpray(GameData);
            AnnouncerData = new DefaultDataAnnouncer(GameData);
            VoiceLineData = new DefaultDataVoiceLine(GameData);
            PortraitPackData = new DefaultDataPortraitPack(GameData);
            EmoticonData = new DefaultDataEmoticon(GameData);
            EmoticonPackData = new DefaultDataEmoticonPack(GameData);

            BehaviorVeterancyData = new DefaultDataBehaviorVeterancy(GameData);
        }
    }
}
