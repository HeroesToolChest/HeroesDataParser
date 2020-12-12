using Heroes.Models;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.HeroSkinData
{
    internal class HeroSkinDataXmlWriter : HeroSkinDataWriter<XElement, XElement>
    {
        public HeroSkinDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "HeroSkins";
        }

        protected override XElement MainElement(HeroSkin heroSkin)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(heroSkin);

            return new XElement(
                XmlConvert.EncodeName(heroSkin.Id),
                string.IsNullOrEmpty(heroSkin.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", heroSkin.Name),
                string.IsNullOrEmpty(heroSkin.HyperlinkId) ? null! : new XAttribute("hyperlinkId", heroSkin.HyperlinkId),
                string.IsNullOrEmpty(heroSkin.AttributeId) ? null! : new XAttribute("attributeId", heroSkin.AttributeId),
                new XAttribute("rarity", heroSkin.Rarity),
                new XAttribute("franchise", heroSkin.Franchise),
                heroSkin.ReleaseDate.HasValue ? new XAttribute("releaseDate", heroSkin.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null!,
                string.IsNullOrEmpty(heroSkin.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", heroSkin.SortName),
                string.IsNullOrEmpty(heroSkin.SearchText) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SearchText", heroSkin.SearchText),
                string.IsNullOrEmpty(heroSkin.InfoText?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("InfoText", GetTooltip(heroSkin.InfoText, FileOutputOptions.DescriptionType)),
                heroSkin.Features.Any() ? new XElement("Features", heroSkin.Features.Select(f => new XElement("Feature", f))) : null!,
                heroSkin.VariationSkinIds.Count > 0 ? new XElement("VariationSkinIds", heroSkin.VariationSkinIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(d => new XElement("VariationSkinId", d))) : null!);
        }
    }
}
