using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class EmoticonPackParser : ParserBase<EmoticonPack, EmoticonPackDataOverride>, IParser<EmoticonPack?, EmoticonPackParser>
    {
        public EmoticonPackParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CEmoticonPack";

        public EmoticonPackParser GetInstance()
        {
            return new EmoticonPackParser(XmlDataService);
        }

        public EmoticonPack? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement? emoticonPackElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
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
            string? parentValue = emoticonElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetEmoticonPackData(parentElement, emoticonPack);
            }
            else
            {
                string? desc = GameData.GetGameString(DefaultData.EmoticonPackData?.EmoticonPackDescription?.Replace(DefaultData.IdPlaceHolder, emoticonElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(desc))
                    emoticonPack.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in emoticonElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    emoticonPack.Name = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "SORTNAME")
                {
                    emoticonPack.SortName = element.Attribute("value")?.Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        emoticonPack.Description = new TooltipDescription(text);
                }
                else if (elementName == "HYPERLINKID")
                {
                    emoticonPack.HyperlinkId = element.Attribute("value")?.Value ?? string.Empty;
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
                        day = DefaultData.HeroData!.HeroAlphaReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.HeroData!.HeroAlphaReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.HeroData!.HeroAlphaReleaseDate.Year;

                    emoticonPack.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "EMOTICONARRAY")
                {
                    string item = element.Attribute("value")?.Value ?? string.Empty;

                    if (!string.IsNullOrEmpty(item))
                        emoticonPack.EmoticonIds.Add(item);
                }
            }
        }

        private void SetDefaultValues(EmoticonPack emoticonPack)
        {
            emoticonPack.Name = GameData.GetGameString(DefaultData.EmoticonPackData?.EmoticonPackName?.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id, StringComparison.OrdinalIgnoreCase));
            emoticonPack.SortName = GameData.GetGameString(DefaultData.EmoticonPackData?.EmoticonPackSortName?.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id, StringComparison.OrdinalIgnoreCase));
            emoticonPack.Description = new TooltipDescription(GameData.GetGameString(DefaultData.EmoticonPackData?.EmoticonPackDescription?.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id, StringComparison.OrdinalIgnoreCase)));
            emoticonPack.HyperlinkId = DefaultData.EmoticonPackData?.EmoticonPackHyperlinkId?.Replace(DefaultData.IdPlaceHolder, emoticonPack.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
        }
    }
}
