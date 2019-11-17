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
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

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
                string desc = GameData.GetGameString(DefaultData.BannerData?.BannerDescription?.Replace(DefaultData.IdPlaceHolder, bannerElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    banner.Description = new TooltipDescription(DescriptionValidator.Validate(desc));
            }

            foreach (XElement element in bannerElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "DESCRIPTION")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        banner.Description = new TooltipDescription(DescriptionValidator.Validate(text));
                }
                else if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        banner.SortName = text;
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
                    banner.AttributeId = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "HYPERLINKID")
                {
                    banner.HyperlinkId = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        banner.Rarity = heroRarity;
                    else
                        banner.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        banner.Name = text;
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
            banner.Name = GameData.GetGameString(DefaultData.BannerData?.BannerName?.Replace(DefaultData.IdPlaceHolder, banner.Id));
            banner.SortName = GameData.GetGameString(DefaultData.BannerData?.BannerSortName?.Replace(DefaultData.IdPlaceHolder, banner.Id));
            banner.Description = new TooltipDescription(DescriptionValidator.Validate(GameData.GetGameString(DefaultData.BannerData?.BannerDescription?.Replace(DefaultData.IdPlaceHolder, banner.Id))));
            banner.ReleaseDate = DefaultData?.BannerData?.BannerReleaseDate;
            banner.Rarity = Rarity.None;
        }
    }
}
