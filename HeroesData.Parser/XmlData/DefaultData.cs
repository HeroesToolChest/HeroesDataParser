using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        public const string CButtonDefaultBaseId = "StormButtonParent";

        public const string DefaultHeroDifficulty = "Easy";

        public const string AbilMountLinkId = "Mount";

        private readonly GameData GameData;

        public DefaultData(GameData gameData)
        {
            GameData = gameData;
        }

        public DefaultDataHero HeroData { get; private set; }

        public DefaultDataUnit UnitData { get; private set; }

        /// <summary>
        /// Gets the default button name text. Contains ##id##.
        /// </summary>
        public string ButtonName { get; private set; }

        /// <summary>
        /// Gets the default button tooltip text. Contains ##id##. Full text.
        /// </summary>
        public string ButtonTooltip { get; private set; }

        /// <summary>
        /// Gets the default button simple display text. Contains ##id##. Short text.
        /// </summary>
        public string ButtonSimpleDisplayText { get; private set; }

        /// <summary>
        /// Gets the default button tooltip vital text.
        /// </summary>
        public string ButtonTooltipEnergyVitalName { get; private set; }

        /// <summary>
        /// Gets the default button hotkey text. Contains ##id##.
        /// </summary>
        public string ButtonHotkey { get; private set; }

        /// <summary>
        /// Gets the default button hotkey alias text. Contains ##id##.
        /// </summary>
        public string ButtonHotkeyAlias { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show name.
        /// </summary>
        public bool ButtonTooltipFlagShowName { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show hotkey.
        /// </summary>
        public bool ButtonTooltipFlagShowHotkey { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show usage.
        /// </summary>
        public bool ButtonTooltipFlagShowUsage { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show time.
        /// </summary>
        public bool ButtonTooltipFlagShowTime { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show cooldown.
        /// </summary>
        public bool ButtonTooltipFlagShowCooldown { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show requirements.
        /// </summary>
        public bool ButtonTooltipFlagShowRequirements { get; private set; }

        /// <summary>
        /// Gets the default button tooltip flag - show autocast.
        /// </summary>
        public bool ButtonTooltipFlagShowAutocast { get; private set; }

        /// <summary>
        /// Gets the defualt weapon name text. Contains ##id##.
        /// </summary>
        public string WeaponName { get; private set; }

        /// <summary>
        /// Gets the default weapon range value.
        /// </summary>
        public double WeaponRange { get; private set; }

        /// <summary>
        /// Gets the default weapon period value.
        /// </summary>
        public double WeaponPeriod { get; private set; }

        /// <summary>
        /// Gets the default weapon display effect name. Contains ##id##.
        /// </summary>
        public string WeaponDisplayEffect { get; private set; }

        /// <summary>
        /// Gets the default hero skin name. Contains ##id##.
        /// </summary>
        public string HeroSkinName { get; private set; }

        /// <summary>
        /// Gets the default hero skin name used for sorting. Contains ##id##.
        /// </summary>
        public string HeroSkinSortName { get; private set; }

        /// <summary>
        /// Gets the default hero skin info text. Contains ##id##.
        /// </summary>
        public string HeroSkinInfoText { get; private set; }

        /// <summary>
        /// Gets the default hero skin additional search text. Contains ##id##.
        /// </summary>
        public string HeroSkinAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default hero skin hyperlinkId. Contains ##id##.
        /// </summary>
        public string HeroSkinHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default hero skins release date.
        /// </summary>
        public DateTime HeroSkinReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default mount name. Contains ##id##.
        /// </summary>
        public string MountName { get; private set; }

        /// <summary>
        /// Gets the default mount name used for sorting. Contains ##id##.
        /// </summary>
        public string MountSortName { get; private set; }

        /// <summary>
        /// Gets the default mount info text. Contains ##id##.
        /// </summary>
        public string MountInfoText { get; private set; }

        /// <summary>
        /// Gets the default mount additional search text. Contains ##id##.
        /// </summary>
        public string MountAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default mount hyperlinkId. Contains ##id##.
        /// </summary>
        public string MountHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default mount release date.
        /// </summary>
        public DateTime MountReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default banner name. Contains ##id##.
        /// </summary>
        public string BannerName { get; private set; }

        /// <summary>
        /// Gets the default banner name used for sorting. Contains ##id##.
        /// </summary>
        public string BannerSortName { get; private set; }

        /// <summary>
        /// Gets the default banner description. Contains ##id##.
        /// </summary>
        public string BannerDescription { get; private set; }

        /// <summary>
        /// Gets the default banner release date.
        /// </summary>
        public DateTime BannerReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default spray name. Contains ##id##.
        /// </summary>
        public string SprayName { get; private set; }

        /// <summary>
        /// Gets the default spray name used for sorting. Contains ##id##.
        /// </summary>
        public string SpraySortName { get; private set; }

        /// <summary>
        /// Gets the default spray description. Contains ##id##.
        /// </summary>
        public string SprayDescription { get; private set; }

        /// <summary>
        /// Gets the default spray additional search text. Contains ##id##.
        /// </summary>
        public string SprayAdditionalSearchText { get; private set; }

        /// <summary>
        /// Gets the default spray hyperlinkId. Contains ##id##.
        /// </summary>
        public string SprayHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default spray release date.
        /// </summary>
        public DateTime SprayReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default spray animation count.
        /// </summary>
        public int SprayAnimationCount { get; private set; }

        /// <summary>
        /// Gets the default spray animation duration.
        /// </summary>
        public int SprayAnimationDuration { get; private set; }

        /// <summary>
        /// Gets the default announcer name. Contains ##id##.
        /// </summary>
        public string AnnouncerName { get; private set; }

        /// <summary>
        /// Gets the default announcer name used for sorting. Contains ##id##.
        /// </summary>
        public string AnnouncerSortName { get; private set; }

        /// <summary>
        /// Gets the default announcer description. Contains ##id##.
        /// </summary>
        public string AnnouncerDescription { get; private set; }

        /// <summary>
        /// Gets the default announcer release date.
        /// </summary>
        public DateTime AnnouncerReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default voice line name. Contains ##id##.
        /// </summary>
        public string VoiceLineName { get; private set; }

        /// <summary>
        /// Gets the default voice line name used for sorting. Contains ##id##.
        /// </summary>
        public string VoiceLineSortName { get; private set; }

        /// <summary>
        /// Gets the default voice line description. Contains ##id##.
        /// </summary>
        public string VoiceLineDescription { get; private set; }

        /// <summary>
        /// Gets the default voice line attribute id.
        /// </summary>
        public string VoiceLineAttributeId { get; private set; }

        /// <summary>
        /// Gets the default voice line release date.
        /// </summary>
        public DateTime VoiceLineReleaseDate { get; private set; }

        /// <summary>
        /// Gets the default portrait name. Contains ##id##.
        /// </summary>
        public string PortraitName { get; private set; }

        /// <summary>
        /// Gets the default portrait name used for sorting. Contains ##id##.
        /// </summary>
        public string PortraitSortName { get; private set; }

        /// <summary>
        /// Gets the default portrait hyperlinkId. Contains ##id##.
        /// </summary>
        public string PortraitHyperlinkId { get; private set; }

        /// <summary>
        /// Gets the default emoticon localized alias array value. Contains ##id##.
        /// </summary>
        public string EmoticonLocalizedAliasArray { get; private set; }

        /// <summary>
        /// Gets the default emoticon description. Contains ##id##.
        /// </summary>
        public string EmoticonDescription { get; private set; }

        /// <summary>
        /// Gets the default emoticon expression.
        /// </summary>
        public string EmoticonExpression { get; private set; }

        /// <summary>
        /// Gets the default emoticon texture sheet.
        /// </summary>
        public string EmoticonTextureSheet { get; private set; }

        /// <summary>
        /// Gets the default texture sheet image file name. Contains ##id##.
        /// </summary>
        public string TextureSheetImage { get; private set; }

        /// <summary>
        /// Gets the default texture sheet rows.
        /// </summary>
        public int TextureSheetRows { get; private set; }

        /// <summary>
        /// Gets the default texture sheet columns.
        /// </summary>
        public int TextureSheetColumns { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack name. Contains ##id##.
        /// </summary>
        public string EmoticonPackName { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack name used for sorting. Contains ##id##.
        /// </summary>
        public string EmoticonPackSortName { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack description. Contains ##id##.
        /// </summary>
        public string EmoticonPackDescription { get; private set; }

        /// <summary>
        /// Gets the default emoticon pack hyperlinkId. Contains ##id##.
        /// </summary>
        public string EmoticonPackHyperlinkId { get; private set; }

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

            LoadCButtonDefault();
            LoadCButtonDefaultStormButtonParent();

            LoadCWeaponDefault();

            LoadCSkinDefault();
            LoadCMountDefault();
            LoadCBannerDefault();
            LoadCSprayDefault();
            LoadCAnnouncerPackDefault();
            LoadCVoiceLineDefault();
            LoadCPortraitPackDefault();
            LoadCEmoticonDefault();
            LoadCTextureSheetDefault();
            LoadCEmoticonPackDefault();
        }

        // <CButton default="1">
        private void LoadCButtonDefault()
        {
            CButtonElement(GameData.Elements("CButton").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CButton default="1" id="StormButtonParent">
        private void LoadCButtonDefaultStormButtonParent()
        {
            CButtonElement(GameData.Elements("CButton").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CButtonDefaultBaseId));
        }

        // <CWeapon default="1">
        private void LoadCWeaponDefault()
        {
            CWeaponElement(GameData.Elements("CWeapon").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CSkin default="1">
        private void LoadCSkinDefault()
        {
            CSkinElement(GameData.Elements("CSkin").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CMount default="1">
        private void LoadCMountDefault()
        {
            CMountElement(GameData.Elements("CMount").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CBanner default="1">
        private void LoadCBannerDefault()
        {
            CBannerElement(GameData.Elements("CBanner").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CSpray default="1">
        private void LoadCSprayDefault()
        {
            CSprayElement(GameData.Elements("CSpray").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CAnnouncerPack default="1">
        private void LoadCAnnouncerPackDefault()
        {
            CAnnouncerPackElement(GameData.Elements("CAnnouncerPack").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CVoiceLine default="1">
        private void LoadCVoiceLineDefault()
        {
            CVoiceLineElement(GameData.Elements("CVoiceLine").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CPortraitPack default="1">
        private void LoadCPortraitPackDefault()
        {
            CPortraitPackElement(GameData.Elements("CPortraitPack").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CEmoticon default="1">
        private void LoadCEmoticonDefault()
        {
            CEmoticonElement(GameData.Elements("CEmoticon").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CTextureSheet default="1">
        private void LoadCTextureSheetDefault()
        {
            CTextureSheetElement(GameData.Elements("CTextureSheet").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CEmoticonPack default="1">
        private void LoadCEmoticonPackDefault()
        {
            CEmoticonPackElement(GameData.Elements("CEmoticonPack").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CButtonElement(IEnumerable<XElement> cButtonElements)
        {
            foreach (XElement element in cButtonElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    ButtonName = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIP")
                {
                    ButtonTooltip = element.Attribute("value").Value;
                }
                else if (elementName == "HOTKEY")
                {
                    ButtonHotkey = element.Attribute("value").Value;
                }
                else if (elementName == "HOTKEYALIAS")
                {
                    ButtonHotkeyAlias = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIPFLAGS")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "ShowName")
                        ButtonTooltipFlagShowName = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowHotkey")
                        ButtonTooltipFlagShowHotkey = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowUsage")
                        ButtonTooltipFlagShowUsage = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowTime")
                        ButtonTooltipFlagShowTime = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowCooldown")
                        ButtonTooltipFlagShowCooldown = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowRequirements")
                        ButtonTooltipFlagShowRequirements = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowAutocast")
                        ButtonTooltipFlagShowAutocast = element.Attribute("value").Value == "1" ? true : false;
                }
                else if (elementName == "SIMPLEDISPLAYTEXT")
                {
                    ButtonSimpleDisplayText = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIPVITALNAME")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "Energy")
                        ButtonTooltipEnergyVitalName = element.Attribute("value").Value;
                }
            }
        }

        private void CWeaponElement(IEnumerable<XElement> cWeaponElements)
        {
            foreach (XElement element in cWeaponElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    WeaponName = element.Attribute("value").Value;
                }
                else if (elementName == "DISPLAYEFFECT")
                {
                    WeaponDisplayEffect = element.Attribute("value").Value;
                }
                else if (elementName == "RANGE")
                {
                    WeaponRange = double.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "PERIOD")
                {
                    WeaponPeriod = double.Parse(element.Attribute("value").Value);
                }
            }
        }

        private void CSkinElement(IEnumerable<XElement> cSkinElements)
        {
            foreach (XElement element in cSkinElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    HeroSkinName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    HeroSkinSortName = element.Attribute("value").Value;
                }
                else if (elementName == "INFOTEXT")
                {
                    HeroSkinInfoText = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    HeroSkinAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    HeroSkinHyperlinkId = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 3;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    HeroSkinReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CMountElement(IEnumerable<XElement> cMountElements)
        {
            foreach (XElement element in cMountElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    MountName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    MountSortName = element.Attribute("value").Value;
                }
                else if (elementName == "INFOTEXT")
                {
                    MountInfoText = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    MountAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    MountHyperlinkId = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    MountReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CBannerElement(IEnumerable<XElement> cBannerElements)
        {
            foreach (XElement element in cBannerElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    BannerName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    BannerSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    BannerDescription = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    BannerReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CSprayElement(IEnumerable<XElement> cSprayElements)
        {
            foreach (XElement element in cSprayElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    SprayName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    SpraySortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    SprayDescription = element.Attribute("value").Value;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    SprayAdditionalSearchText = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    SprayReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "HYPERLINKID")
                {
                    SprayHyperlinkId = element.Attribute("value").Value;
                }
                else if (elementName == "ANIMCOUNT")
                {
                    SprayAnimationCount = int.Parse(element.Attribute("value").Value);
                }
                else if (elementName == "ANIMDURATION")
                {
                    SprayAnimationDuration = int.Parse(element.Attribute("value").Value);
                }
            }
        }

        private void CAnnouncerPackElement(IEnumerable<XElement> cAnnouncerPackElements)
        {
            foreach (XElement element in cAnnouncerPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    AnnouncerName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    AnnouncerSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    AnnouncerDescription = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    AnnouncerReleaseDate = new DateTime(year, month, day);
                }
            }
        }

        private void CVoiceLineElement(IEnumerable<XElement> cVoiceLineElements)
        {
            foreach (XElement element in cVoiceLineElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    VoiceLineName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    VoiceLineSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    VoiceLineDescription = element.Attribute("value").Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Element("Year").Attribute("value").Value, out int year))
                        year = 2014;

                    if (!int.TryParse(element.Element("Month").Attribute("value").Value, out int month))
                        month = 1;

                    if (!int.TryParse(element.Element("Day").Attribute("value").Value, out int day))
                        day = 1;

                    VoiceLineReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    VoiceLineAttributeId = element.Attribute("value").Value;
                }
            }
        }

        private void CPortraitPackElement(IEnumerable<XElement> cPortraitPackElements)
        {
            foreach (XElement element in cPortraitPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    PortraitName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    PortraitSortName = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    PortraitHyperlinkId = element.Attribute("value").Value;
                }
            }
        }

        private void CEmoticonElement(IEnumerable<XElement> cEmoticonElements)
        {
            foreach (XElement element in cEmoticonElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "LOCALIZEDALIASARRAY")
                {
                    EmoticonLocalizedAliasArray = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    EmoticonDescription = element.Attribute("value").Value;
                }
                else if (elementName == "EXPRESSION")
                {
                    EmoticonExpression = element.Attribute("value").Value;
                }
                else if (elementName == "IMAGE")
                {
                    EmoticonTextureSheet = element.Attribute("TextureSheet").Value;
                }
            }
        }

        private void CTextureSheetElement(IEnumerable<XElement> cTextureSheetElements)
        {
            foreach (XElement element in cTextureSheetElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "IMAGE")
                {
                    TextureSheetImage = element.Attribute("value").Value;
                }
                else if (elementName == "ROWS")
                {
                    if (int.TryParse(element.Attribute("value").Value, out int value))
                        TextureSheetRows = value;
                }
                else if (elementName == "COLUMNS")
                {
                    if (int.TryParse(element.Attribute("value").Value, out int value))
                        TextureSheetColumns = value;
                }
            }
        }

        private void CEmoticonPackElement(IEnumerable<XElement> cEmoticonPackElements)
        {
            foreach (XElement element in cEmoticonPackElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    EmoticonPackName = element.Attribute("value").Value;
                }
                else if (elementName == "SORTNAME")
                {
                    EmoticonPackSortName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    EmoticonPackDescription = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    EmoticonPackHyperlinkId = element.Attribute("value").Value;
                }
            }
        }
    }
}
