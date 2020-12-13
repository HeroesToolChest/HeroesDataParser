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
    public class BundleParser : ParserBase<Bundle, BundleDataOverride>, IParser<Bundle?, BundleParser>
    {
        public BundleParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CBundle";

        public BundleParser GetInstance()
        {
            return new BundleParser(XmlDataService);
        }

        public Bundle? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? bundleElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (bundleElement == null)
                return null;

            Bundle bundle = new Bundle()
            {
                Id = id,
            };

            SetDefaultValues(bundle);
            SetBundleData(bundleElement, bundle);

            if (bundle.ReleaseDate == DefaultData.HeroData?.HeroReleaseDate)
                bundle.ReleaseDate = DefaultData.HeroData?.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(bundle.HyperlinkId))
                bundle.HyperlinkId = id;

            return bundle;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return true;
        }

        private void SetBundleData(XElement bundleElement, Bundle bundle)
        {
            // parent lookup
            string? parentValue = bundleElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetBundleData(parentElement, bundle);
            }

            foreach (XElement element in bundleElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "SORTNAME")
                {
                    string? sortNameValue = element.Attribute("value")?.Value;
                    if (sortNameValue is not null)
                    {
                        if (GameData.TryGetGameString(sortNameValue, out string? text))
                            bundle.SortName = text;
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

                    bundle.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "HYPERLINKID")
                {
                    bundle.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            bundle.Name = text;
                    }
                }
                else if (elementName == "UNIVERSE")
                {
                    string? universe = element.Attribute("value")?.Value.ToUpperInvariant();

                    if (universe == "STARCRAFT")
                        bundle.Franchise = HeroFranchise.Starcraft;
                    else if (universe == "WARCRAFT")
                        bundle.Franchise = HeroFranchise.Warcraft;
                    else if (universe == "DIABLO")
                        bundle.Franchise = HeroFranchise.Diablo;
                    else if (universe == "OVERWATCH")
                        bundle.Franchise = HeroFranchise.Overwatch;
                    else if (universe == "HEROES" || universe == "NEXUS")
                        bundle.Franchise = HeroFranchise.Nexus;
                }
                else if (elementName == "TILETEXTURE")
                {
                    bundle.ImageFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value.Replace(DefaultData.IdPlaceHolder, bundle.Id, StringComparison.OrdinalIgnoreCase)))?.ToLowerInvariant();
                    if (!Path.HasExtension(bundle.ImageFileName))
                        bundle.ImageFileName = null;
                }
                else if (elementName == "EVENTNAME")
                {
                    bundle.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "HEROARRAY")
                {
                    string? heroValue = element.Attribute("value")?.Value;

                    if (heroValue is not null)
                    {
                        bundle.HeroIds.Add(heroValue);
                    }
                }
                else if (elementName == "MOUNTARRAY")
                {
                    string? mountValue = element.Attribute("value")?.Value;

                    if (mountValue is not null)
                    {
                        bundle.MountIds.Add(mountValue);
                    }
                }
                else if (elementName == "SKINARRAY")
                {
                    string? heroValue = element.Attribute("Hero")?.Value;
                    string? skinValue = element.Attribute("Skin")?.Value;

                    if (heroValue is not null && skinValue is not null)
                    {
                        bundle.AddHeroSkin(heroValue, skinValue);
                    }
                }
                else if (elementName == "BOOSTBONUS")
                {
                    bundle.BoostBonusId = element.Attribute("value")?.Value;
                }
                else if (elementName == "GOLDBONUS")
                {
                    bundle.GoldBonus = XmlParse.GetIntValue(bundle.Id, element, GameData);
                }
                else if (elementName == "GEMSBONUS")
                {
                    bundle.GemsBonus = XmlParse.GetIntValue(bundle.Id, element, GameData);
                }
                else if (elementName == "FLAGS")
                {
                    string? index = element.Attribute("index")?.Value;
                    string? value = element.Attribute("value")?.Value;

                    if (index == "ShowDynamicProductContent" && value == "1")
                    {
                        bundle.IsDynamicContext = true;
                    }
                }
            }
        }

        private void SetDefaultValues(Bundle bundle)
        {
            bundle.Name = GameData.GetGameString(DefaultData.BundleData?.BundleName?.Replace(DefaultData.IdPlaceHolder, bundle.Id, StringComparison.OrdinalIgnoreCase));
            bundle.SortName = GameData.GetGameString(DefaultData.BundleData?.BundleSortName?.Replace(DefaultData.IdPlaceHolder, bundle.Id, StringComparison.OrdinalIgnoreCase));
            bundle.HyperlinkId = DefaultData.BundleData?.BundleHyperlinkId?.Replace(DefaultData.IdPlaceHolder, bundle.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            bundle.ReleaseDate = DefaultData.BundleData?.BundleReleaseDate;
        }
    }
}
