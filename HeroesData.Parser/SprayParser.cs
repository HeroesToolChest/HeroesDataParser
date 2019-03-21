using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class SprayParser : ParserBase<Spray, SprayDataOverride>, IParser<Spray, SprayParser>
    {
        public SprayParser(GameData gameData, DefaultData defaultData)
            : base(gameData, defaultData)
        {
        }

        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                IEnumerable<XElement> cSprayElements = GameData.Elements("CSpray").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement sprayElement in cSprayElements)
                {
                    string id = sprayElement.Attribute("id").Value;
                    if (sprayElement.Element("AttributeId") != null && id != "RandomSpray")
                        items.Add(new string[] { id });
                }

                return items;
            }
        }

        public SprayParser GetInstance()
        {
            return new SprayParser(GameData, DefaultData);
        }

        public Spray Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement sprayElement = GameData.MergeXmlElements(GameData.Elements("CSpray").Where(x => x.Attribute("id")?.Value == id));
            if (sprayElement == null)
                return null;

            Spray spray = new Spray()
            {
                Id = id,
            };

            SetDefaultValues(spray);
            SetSprayData(sprayElement, spray);

            if (spray.ReleaseDate == DefaultData.HeroReleaseDate)
                spray.ReleaseDate = DefaultData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(spray.HyperlinkId))
                spray.HyperlinkId = id;

            return spray;
        }

        private void SetSprayData(XElement sprayElement, Spray spray)
        {
            // parent lookup
            string parentValue = sprayElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements("CSpray").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetSprayData(parentElement, spray);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.SprayDescription.Replace(DefaultData.IdPlaceHolder, sprayElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    spray.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in sprayElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "INFOTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        spray.Description = new TooltipDescription(text);
                }
                else if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        spray.SortName = text;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.SprayReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.SprayReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.SprayReleaseDate.Year;

                    spray.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    spray.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    spray.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        spray.Rarity = heroRarity;
                    else
                        spray.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        spray.Name = text;
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        spray.SearchText = text;
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    spray.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "EVENTNAME")
                {
                    spray.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "TEXTURE")
                {
                    spray.ImageFileName = Path.GetFileName(PathHelpers.GetFilePath(element.Attribute("value")?.Value)).ToLower();
                }
            }
        }

        private void SetDefaultValues(Spray spray)
        {
            spray.Name = GameData.GetGameString(DefaultData.SprayName.Replace(DefaultData.IdPlaceHolder, spray.Id));
            spray.SortName = GameData.GetGameString(DefaultData.SpraySortName.Replace(DefaultData.IdPlaceHolder, spray.Id));
            spray.Description = new TooltipDescription(GameData.GetGameString(DefaultData.SprayDescription.Replace(DefaultData.IdPlaceHolder, spray.Id)));
            spray.HyperlinkId = GameData.GetGameString(DefaultData.SprayHyperlinkId.Replace(DefaultData.IdPlaceHolder, spray.Id));
            spray.ReleaseDate = DefaultData.SprayReleaseDate;
            spray.Rarity = Rarity.None;

            spray.SearchText = GameData.GetGameString(DefaultData.SprayAdditionalSearchText.Replace(DefaultData.IdPlaceHolder, spray.Id));
            if (!string.IsNullOrEmpty(spray.SearchText))
                spray.SearchText = spray.SearchText.Trim();
        }
    }
}
