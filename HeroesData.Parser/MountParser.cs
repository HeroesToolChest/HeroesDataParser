using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class MountParser : ParserBase<Mount, MountDataOverride>, IParser<Mount?, MountParser>
    {
        public MountParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CMount";

        public MountParser GetInstance()
        {
            return new MountParser(XmlDataService);
        }

        public Mount? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? mountElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (mountElement == null)
                return null;

            Mount mount = new Mount()
            {
                Id = id,
            };

            SetDefaultValues(mount);
            SetMountData(mountElement, mount);

            if (mount.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                mount.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(mount.HyperlinkId))
                mount.HyperlinkId = id;

            return mount;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return element.Element("AttributeId") != null;
        }

        private void SetMountData(XElement mountElement, Mount mount)
        {
            // parent lookup
            string? parentValue = mountElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetMountData(parentElement, mount);
            }
            else
            {
                string infoText = GameData.GetGameString(DefaultData.MountData?.MountInfoText?.Replace(DefaultData.IdPlaceHolder, mountElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(infoText))
                    mount.InfoText = new TooltipDescription(infoText);
            }

            foreach (XElement element in mountElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "INFOTEXT")
                {
                    string? infoTextValue = element.Attribute("value")?.Value;
                    if (infoTextValue is not null)
                    {
                        if (GameData.TryGetGameString(infoTextValue, out string? text))
                            mount.InfoText = new TooltipDescription(text);
                    }
                }
                else if (elementName == "SORTNAME")
                {
                    string? sortNameValue = element.Attribute("value")?.Value;
                    if (sortNameValue is not null)
                    {
                        if (GameData.TryGetGameString(sortNameValue, out string? text))
                            mount.SortName = text;
                    }
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.MountData!.MountReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.MountData!.MountReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.MountData!.MountReleaseDate.Year;

                    mount.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    mount.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    mount.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity heroRarity))
                        mount.Rarity = heroRarity;
                    else
                        mount.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            mount.Name = text;
                    }
                }
                else if (elementName == "ADDITIONALSEARCHTEXT")
                {
                    string? additionalSearchTextValue = element.Attribute("value")?.Value;
                    if (additionalSearchTextValue is not null)
                    {
                        if (GameData.TryGetGameString(additionalSearchTextValue, out string? text))
                            mount.SearchText = text;
                    }
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    mount.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "EVENTNAME")
                {
                    mount.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "MOUNTCATEGORY")
                {
                    mount.MountCategory = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Mount mount)
        {
            mount.Name = GameData.GetGameString(DefaultData.MountData?.MountName?.Replace(DefaultData.IdPlaceHolder, mount.Id, StringComparison.OrdinalIgnoreCase));
            mount.SortName = GameData.GetGameString(DefaultData.MountData?.MountSortName?.Replace(DefaultData.IdPlaceHolder, mount.Id, StringComparison.OrdinalIgnoreCase));
            mount.InfoText = new TooltipDescription(GameData.GetGameString(DefaultData.MountData?.MountInfoText?.Replace(DefaultData.IdPlaceHolder, mount.Id, StringComparison.OrdinalIgnoreCase)));
            mount.HyperlinkId = DefaultData.MountData?.MountHyperlinkId?.Replace(DefaultData.IdPlaceHolder, mount.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            mount.ReleaseDate = DefaultData.MountData?.MountReleaseDate;

            mount.SearchText = GameData.GetGameString(DefaultData.MountData?.MountAdditionalSearchText?.Replace(DefaultData.IdPlaceHolder, mount.Id, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(mount.SearchText))
                mount.SearchText = mount.SearchText.Trim();
        }
    }
}
