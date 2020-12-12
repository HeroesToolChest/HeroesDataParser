﻿using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class HeroSkinParser : ParserBase<HeroSkin, HeroSkinDataOverride>, IParser<HeroSkin?, HeroSkinParser>
    {
        public HeroSkinParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CSkin";

        public HeroSkinParser GetInstance()
        {
            return new HeroSkinParser(XmlDataService);
        }

        public HeroSkin? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? skinElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (skinElement == null)
                return null;

            HeroSkin heroSkin = new HeroSkin()
            {
                Id = id,
            };

            SetDefaultValues(heroSkin);
            SetSkinData(skinElement, heroSkin);

            if (heroSkin.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                heroSkin.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(heroSkin.HyperlinkId))
                heroSkin.HyperlinkId = id;

            return heroSkin;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return element.Element("AttributeId") != null;
        }

        private void SetSkinData(XElement skinElement, HeroSkin heroSkin)
        {
            // parent lookup
            string? parentValue = skinElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetSkinData(parentElement, heroSkin);
            }
            else
            {
                string? infoText = GameData.GetGameString(DefaultData.HeroSkinData?.HeroSkinInfoText?.Replace(DefaultData.IdPlaceHolder, skinElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(infoText))
                    heroSkin.InfoText = new TooltipDescription(infoText);
            }

            foreach (XElement element in skinElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "INFOTEXT")
                {
                    string? infoText = element.Attribute("value")?.Value;
                    if (infoText is not null)
                    {
                        if (GameData.TryGetGameString(infoText, out string? text))
                            heroSkin.InfoText = new TooltipDescription(text);
                    }
                }
                else if (elementName == "SORTNAME")
                {
                    string? sortNameValue = element.Attribute("value")?.Value;
                    if (sortNameValue is not null)
                    {
                        if (GameData.TryGetGameString(sortNameValue, out string? text))
                            heroSkin.SortName = text;
                    }
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.HeroSkinData!.HeroSkinReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.HeroSkinData!.HeroSkinReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.HeroSkinData!.HeroSkinReleaseDate.Year;

                    heroSkin.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    heroSkin.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    heroSkin.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity heroRarity))
                        heroSkin.Rarity = heroRarity;
                    else
                        heroSkin.Rarity = Rarity.Unknown;
                }
                else if (elementName == "FEATUREARRAY")
                {
                    string? value = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(value))
                        heroSkin.Features.Add(value);
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            heroSkin.Name = text;
                    }
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    string? additionalSearchTextValue = element.Attribute("value")?.Value;
                    if (additionalSearchTextValue is not null)
                    {
                        if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                            heroSkin.SearchText = text;
                    }
                }
                else if (elementName == "UNIVERSE")
                {
                    string? universe = element.Attribute("value")?.Value.ToUpperInvariant();

                    if (universe == "STARCRAFT")
                        heroSkin.Franchise = HeroFranchise.Starcraft;
                    else if (universe == "WARCRAFT")
                        heroSkin.Franchise = HeroFranchise.Warcraft;
                    else if (universe == "DIABLO")
                        heroSkin.Franchise = HeroFranchise.Diablo;
                    else if (universe == "OVERWATCH")
                        heroSkin.Franchise = HeroFranchise.Overwatch;
                    else if (universe == "HEROES" || universe == "NEXUS")
                        heroSkin.Franchise = HeroFranchise.Nexus;
                }
                else if (elementName == "VARIATIONARRAY")
                {
                    string? variation = element.Attribute("value")?.Value;

                    if (variation is not null)
                    {
                        heroSkin.VariationSkinIds.Add(variation);
                    }
                }
            }
        }

        private void SetDefaultValues(HeroSkin heroSkin)
        {
            heroSkin.Name = GameData.GetGameString(DefaultData.HeroSkinData?.HeroSkinName?.Replace(DefaultData.IdPlaceHolder, heroSkin.Id, StringComparison.OrdinalIgnoreCase));
            heroSkin.SortName = GameData.GetGameString(DefaultData.HeroSkinData?.HeroSkinSortName?.Replace(DefaultData.IdPlaceHolder, heroSkin.Id, StringComparison.OrdinalIgnoreCase));
            heroSkin.InfoText = new TooltipDescription(GameData.GetGameString(DefaultData.HeroSkinData?.HeroSkinInfoText?.Replace(DefaultData.IdPlaceHolder, heroSkin.Id, StringComparison.OrdinalIgnoreCase)));
            heroSkin.HyperlinkId = DefaultData.HeroSkinData?.HeroSkinHyperlinkId?.Replace(DefaultData.IdPlaceHolder, heroSkin.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            heroSkin.ReleaseDate = DefaultData.HeroData?.HeroReleaseDate;
            heroSkin.Franchise = HeroFranchise.Unknown;

            heroSkin.SearchText = GameData.GetGameString(DefaultData.HeroSkinData?.HeroSkinAdditionalSearchText?.Replace(DefaultData.IdPlaceHolder, heroSkin.Id, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(heroSkin.SearchText))
                heroSkin.SearchText = heroSkin.SearchText.Trim();
        }
    }
}
