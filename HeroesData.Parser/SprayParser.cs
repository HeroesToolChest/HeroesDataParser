using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class SprayParser : ParserBase<Spray, SprayDataOverride>, IParser<Spray?, SprayParser>
    {
        public SprayParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CSpray";

        public SprayParser GetInstance()
        {
            return new SprayParser(XmlDataService);
        }

        public Spray? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? sprayElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (sprayElement == null)
                return null;

            Spray spray = new Spray()
            {
                Id = id,
            };

            SetDefaultValues(spray);
            SetSprayData(sprayElement, spray);

            if (spray.ReleaseDate == DefaultData.HeroData!.HeroReleaseDate)
                spray.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(spray.HyperlinkId))
                spray.HyperlinkId = id;

            return spray;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return element.Element("AttributeId") != null;
        }

        private void SetSprayData(XElement sprayElement, Spray spray)
        {
            // parent lookup
            string? parentValue = sprayElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetSprayData(parentElement, spray);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.SprayData?.SprayDescription?.Replace(DefaultData.IdPlaceHolder, sprayElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(desc))
                    spray.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in sprayElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "INFOTEXT")
                {
                    string? infoTextValue = element.Attribute("value")?.Value;
                    if (infoTextValue is not null)
                    {
                        if (GameData.TryGetGameString(infoTextValue, out string? text))
                            spray.Description = new TooltipDescription(text);
                    }
                }
                else if (elementName == "SORTNAME")
                {
                    string? sortNameValue = element.Attribute("value")?.Value;
                    if (sortNameValue is not null)
                    {
                        if (GameData.TryGetGameString(sortNameValue, out string? text))
                            spray.SortName = text;
                    }
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.SprayData!.SprayReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.SprayData!.SprayReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.SprayData!.SprayReleaseDate.Year;

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
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity heroRarity))
                        spray.Rarity = heroRarity;
                    else
                        spray.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            spray.Name = text;
                    }
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    string? additionalSearchTextValue = element.Attribute("value")?.Value;
                    if (additionalSearchTextValue is not null)
                    {
                        if (GameData.TryGetGameString(additionalSearchTextValue, out string? text))
                            spray.SearchText = text;
                    }
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
                    spray.TextureSheet.Image = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();
                }
                else if (elementName == "ANIMCOUNT")
                {
                    string? animCountValue = element.Attribute("value")?.Value;
                    if (animCountValue is not null)
                        spray.AnimationCount = int.Parse(animCountValue);
                }
                else if (elementName == "ANIMDURATION")
                {
                    string? animDurationValue = element.Attribute("value")?.Value;
                    if (animDurationValue is not null)
                        spray.AnimationDuration = int.Parse(animDurationValue);
                }
            }
        }

        private void SetDefaultValues(Spray spray)
        {
            spray.Name = GameData.GetGameString(DefaultData.SprayData?.SprayName?.Replace(DefaultData.IdPlaceHolder, spray.Id, StringComparison.OrdinalIgnoreCase));
            spray.SortName = GameData.GetGameString(DefaultData.SprayData?.SpraySortName?.Replace(DefaultData.IdPlaceHolder, spray.Id, StringComparison.OrdinalIgnoreCase));
            spray.Description = new TooltipDescription(GameData.GetGameString(DefaultData.SprayData?.SprayDescription?.Replace(DefaultData.IdPlaceHolder, spray.Id, StringComparison.OrdinalIgnoreCase)));
            spray.HyperlinkId = DefaultData.SprayData?.SprayHyperlinkId?.Replace(DefaultData.IdPlaceHolder, spray.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            spray.ReleaseDate = DefaultData.SprayData?.SprayReleaseDate;
            spray.AnimationCount = DefaultData.SprayData!.SprayAnimationCount;
            spray.AnimationDuration = DefaultData.SprayData.SprayAnimationDuration;

            spray.SearchText = GameData.GetGameString(DefaultData.SprayData?.SprayAdditionalSearchText?.Replace(DefaultData.IdPlaceHolder, spray.Id, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(spray.SearchText))
                spray.SearchText = spray.SearchText.Trim();
        }
    }
}
