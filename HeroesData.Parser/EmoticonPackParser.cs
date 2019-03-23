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
    public class EmoticonPackParser : ParserBase<EmoticonPack, EmoticonPackDataOverride>, IParser<EmoticonPack, EmoticonPackParser>
    {
        public EmoticonPackParser(GameData gameData, DefaultData defaultData)
            : base(gameData, defaultData)
        {
        }

        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                IEnumerable<XElement> cEmoticonPackElements = GameData.Elements("CEmoticonPack").Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement emoticonPackElement in cEmoticonPackElements)
                {
                    string id = emoticonPackElement.Attribute("id").Value;
                    items.Add(new string[] { id });
                }

                return items;
            }
        }

        public EmoticonPackParser GetInstance()
        {
            return new EmoticonPackParser(GameData, DefaultData);
        }

        public EmoticonPack Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement emoticonPackElement = GameData.MergeXmlElements(GameData.Elements("CEmoticonPack").Where(x => x.Attribute("id")?.Value == id));
            if (emoticonPackElement == null)
                return null;

            EmoticonPack emoticonPack = new EmoticonPack()
            {
                Id = id,
            };

            SetDefaultValues(emoticonPack);
            SetEmoticonPackData(emoticonPackElement, emoticonPack);

            return emoticonPack;
        }

        private void SetEmoticonPackData(XElement emoticonElement, EmoticonPack emoticonPack)
        {
            // parent lookup
            string parentValue = emoticonElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements("CEmoticonPack").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetEmoticonPackData(parentElement, emoticonPack);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.EmoticonPackDescription.Replace(DefaultData.IdPlaceHolder, emoticonElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    emoticonPack.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in emoticonElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "NAME")
                {
                    emoticonPack.Name = element.Attribute("value")?.Value;
                }
                else if (elementName == "SORTNAME")
                {
                    emoticonPack.SortName = element.Attribute("value")?.Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        emoticonPack.Description = new TooltipDescription(text);
                }
                else if (elementName == "HYPERLINKID")
                {
                    emoticonPack.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    emoticonPack.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "EVENTNAME")
                {
                    emoticonPack.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.HeroAlphaReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.HeroAlphaReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.HeroAlphaReleaseDate.Year;

                    emoticonPack.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "EMOTICONARRAY")
                {
                    if (emoticonPack.EmoticonIds == null)
                        emoticonPack.EmoticonIds = new List<string>();

                    string item = element.Attribute("value")?.Value;
                    if (!string.IsNullOrEmpty(item))
                        emoticonPack.EmoticonIds.Add(item);
                }
            }
        }

        private void SetDefaultValues(EmoticonPack emoticonPack)
        {
            emoticonPack.Name = GameData.GetGameString(DefaultData.EmoticonPackName.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id));
            emoticonPack.SortName = GameData.GetGameString(DefaultData.EmoticonPackSortName.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id));
            emoticonPack.Description = new TooltipDescription(GameData.GetGameString(DefaultData.EmoticonPackDescription.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id)));
            emoticonPack.HyperlinkId = DefaultData.EmoticonPackHyperlinkId.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id);
        }
    }
}
