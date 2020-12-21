using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class BoostParser : ParserBase<Boost, BoostDataOverride>, IParser<Boost?, BoostParser>
    {
        public BoostParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CBoost";

        public BoostParser GetInstance()
        {
            return new BoostParser(XmlDataService);
        }

        public Boost? Parse(params string[] ids)
        {
            if (ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? boostElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (boostElement == null)
                return null;

            Boost boost = new Boost()
            {
                Id = id,
            };

            SetDefaultValues(boost);
            SetBoostData(boostElement, boost);

            if (boost.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                boost.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(boost.HyperlinkId))
                boost.HyperlinkId = id;

            return boost;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return true;
        }

        private void SetBoostData(XElement boostElement, Boost boost)
        {
            // parent lookup
            string? parentValue = boostElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue && x.Attribute("parent")?.Value != parentValue));
                if (parentElement != null)
                    SetBoostData(parentElement, boost);
            }

            foreach (XElement element in boostElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "SORTNAME")
                {
                    string? sortNameValue = element.Attribute("value")?.Value;
                    if (sortNameValue is not null)
                    {
                        if (GameData.TryGetGameString(sortNameValue, out string? text))
                            boost.SortName = text;
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

                    boost.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "HYPERLINKID")
                {
                    boost.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            boost.Name = text;
                    }
                }
                else if (elementName == "EVENTNAME")
                {
                    boost.EventName = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Boost boost)
        {
            boost.Name = GameData.GetGameString(DefaultData.BoostData?.BoostName?.Replace(DefaultData.IdPlaceHolder, boost.Id, StringComparison.OrdinalIgnoreCase));
            boost.SortName = GameData.GetGameString(DefaultData.BoostData?.BoostSortName?.Replace(DefaultData.IdPlaceHolder, boost.Id, StringComparison.OrdinalIgnoreCase));
            boost.HyperlinkId = DefaultData.BoostData?.BoostHyperlinkId?.Replace(DefaultData.IdPlaceHolder, boost.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            boost.ReleaseDate = DefaultData.BoostData?.BoostReleaseDate;
        }
    }
}
