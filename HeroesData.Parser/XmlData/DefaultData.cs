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

        private readonly GameData _gameData;

        public DefaultData(GameData gameData)
        {
            _gameData = gameData;
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
            if (_gameData == null)
                return;

            HeroData = new DefaultDataHero(_gameData);
            UnitData = new DefaultDataUnit(_gameData);
            AbilData = new DefaultDataAbil(_gameData);

            ButtonData = new DefaultDataButton(_gameData);
            WeaponData = new DefaultDataWeapon(_gameData);

            HeroSkinData = new DefaultDataHeroSkin(_gameData);
            MountData = new DefaultDataMount(_gameData);
            BannerData = new DefaultDataBanner(_gameData);
            SprayData = new DefaultDataSpray(_gameData);
            AnnouncerData = new DefaultDataAnnouncer(_gameData);
            VoiceLineData = new DefaultDataVoiceLine(_gameData);
            PortraitPackData = new DefaultDataPortraitPack(_gameData);
            EmoticonData = new DefaultDataEmoticon(_gameData);
            EmoticonPackData = new DefaultDataEmoticonPack(_gameData);

            BehaviorVeterancyData = new DefaultDataBehaviorVeterancy(_gameData);
        }
    }
}
