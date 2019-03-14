using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class HeroSkinParser : ParserBase<HeroSkin, HeroSkinDataOverride>, IParser<HeroSkin, HeroSkinParser>
    {
        public HeroSkinParser(GameData gameData, DefaultData defaultData)
            : base(gameData, defaultData)
        {
        }

        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                IEnumerable<XElement> cSkinElements = GameData.Elements("CSkin").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement skinElement in cSkinElements)
                {
                    string id = skinElement.Attribute("id").Value;
                    if (skinElement.Element("AttributeId") != null && id != "Random")
                        items.Add(new string[] { id });
                }

                return items;
            }
        }

        public HeroSkinParser GetInstance()
        {
            return new HeroSkinParser(GameData, DefaultData);
        }

        public HeroSkin Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement skinElement = GameData.MergeXmlElements(GameData.Elements("CSkin").Where(x => x.Attribute("id")?.Value == id));
            if (skinElement == null)
                return null;

            HeroSkin heroSkin = new HeroSkin()
            {
                Id = id,
            };

            SetDefaultValues(heroSkin);
            SetSkinData(skinElement, heroSkin);

            if (heroSkin.ReleaseDate == DefaultData.HeroReleaseDate)
                heroSkin.ReleaseDate = DefaultData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(heroSkin.HyperlinkId))
                heroSkin.HyperlinkId = id;

            return heroSkin;
        }

        private void SetSkinData(XElement skinElement, HeroSkin heroSkin)
        {
            // parent lookup
            string parentValue = skinElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements("CSkin").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetSkinData(parentElement, heroSkin);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.HeroSkinInfoText.Replace(DefaultData.IdPlaceHolder, skinElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    heroSkin.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in skinElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "INFOTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        heroSkin.Description = new TooltipDescription(text);
                }
                else if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        heroSkin.SortName = text;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.HeroSkinReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.HeroSkinReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.HeroSkinReleaseDate.Year;

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
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        heroSkin.Rarity = heroRarity;
                    else
                        heroSkin.Rarity = Rarity.Unknown;
                }
                else if (elementName == "FEATUREARRAY")
                {
                    heroSkin.Features.Add(element.Attribute("value")?.Value);
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        heroSkin.Name = text;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        heroSkin.SearchText = text;
                }
            }
        }

        private void SetDefaultValues(HeroSkin heroSkin)
        {
            heroSkin.Name = GameData.GetGameString(DefaultData.HeroSkinName.Replace(DefaultData.IdPlaceHolder, heroSkin.Id));
            heroSkin.SortName = GameData.GetGameString(DefaultData.HeroSkinSortName.Replace(DefaultData.IdPlaceHolder, heroSkin.Id));
            heroSkin.Description = new TooltipDescription(GameData.GetGameString(DefaultData.HeroSkinInfoText.Replace(DefaultData.IdPlaceHolder, heroSkin.Id)));
            heroSkin.HyperlinkId = GameData.GetGameString(DefaultData.HeroSkinHyperlinkId.Replace(DefaultData.IdPlaceHolder, heroSkin.Id));
            heroSkin.ReleaseDate = DefaultData.HeroReleaseDate;
            heroSkin.Rarity = Rarity.None;

            heroSkin.SearchText = GameData.GetGameString(DefaultData.HeroSkinAdditionalSearchText.Replace(DefaultData.IdPlaceHolder, heroSkin.Id));
            if (!string.IsNullOrEmpty(heroSkin.SearchText))
                heroSkin.SearchText = heroSkin.SearchText.Trim();
        }
    }
}
