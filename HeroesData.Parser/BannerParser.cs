using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class BannerParser : ParserBase<Banner, BannerDataOverride>, IParser<Banner?, BannerParser>
    {
        public BannerParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CBanner";

        public BannerParser GetInstance()
        {
            return new BannerParser(XmlDataService);
        }

        public Banner? Parse(params string[] ids)
        {
            if (ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? bannerElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (bannerElement == null)
                return null;

            Banner banner = new Banner()
            {
                Id = id,
            };

            SetDefaultValues(banner);
            SetBannerData(bannerElement, banner);

            if (banner.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                banner.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(banner.HyperlinkId))
                banner.HyperlinkId = id;

            return banner;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return element.Element("AttributeId") != null;
        }

        private void SetBannerData(XElement bannerElement, Banner banner)
        {
            // parent lookup
            string? parentValue = bannerElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetBannerData(parentElement, banner);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.BannerData?.BannerDescription?.Replace(DefaultData.IdPlaceHolder, bannerElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(desc))
                    banner.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in bannerElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "DESCRIPTION")
                {
                    string? descriptionValue = element.Attribute("value")?.Value;
                    if (descriptionValue is not null)
                    {
                        if (GameData.TryGetGameString(descriptionValue, out string? text))
                            banner.Description = new TooltipDescription(text);
                    }
                }
                else if (elementName == "SORTNAME")
                {
                    string? sortName = element.Attribute("value")?.Value;
                    if (sortName is not null)
                    {
                        if (GameData.TryGetGameString(sortName, out string? text))
                            banner.SortName = text;
                    }
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.BannerData!.BannerReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.BannerData!.BannerReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.BannerData!.BannerReleaseDate.Year;

                    banner.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    banner.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    banner.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity heroRarity))
                        banner.Rarity = heroRarity;
                    else
                        banner.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            banner.Name = text;
                    }
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    banner.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "EVENTNAME")
                {
                    banner.EventName = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Banner banner)
        {
            banner.Name = GameData.GetGameString(DefaultData.BannerData?.BannerName?.Replace(DefaultData.IdPlaceHolder, banner.Id, StringComparison.OrdinalIgnoreCase));
            banner.SortName = GameData.GetGameString(DefaultData.BannerData?.BannerSortName?.Replace(DefaultData.IdPlaceHolder, banner.Id, StringComparison.OrdinalIgnoreCase));
            banner.Description = new TooltipDescription(GameData.GetGameString(DefaultData.BannerData?.BannerDescription?.Replace(DefaultData.IdPlaceHolder, banner.Id, StringComparison.OrdinalIgnoreCase)));
            banner.ReleaseDate = DefaultData?.BannerData?.BannerReleaseDate;
        }
    }
}
