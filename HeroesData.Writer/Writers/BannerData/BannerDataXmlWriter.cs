using Heroes.Models;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.BannerData
{
    internal class BannerDataXmlWriter : BannerDataWriter<XElement, XElement>
    {
        public BannerDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Banners";
        }

        protected override XElement MainElement(Banner banner)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(banner);

            return new XElement(
                XmlConvert.EncodeName(banner.Id),
                string.IsNullOrEmpty(banner.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", banner.Name),
                new XAttribute("hyperlinkId", banner.HyperlinkId),
                new XAttribute("attributeId", banner.AttributeId),
                new XAttribute("rarity", banner.Rarity),
                banner.ReleaseDate.HasValue ? new XAttribute("releaseDate", banner.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                string.IsNullOrEmpty(banner.SortName) || FileOutputOptions.IsLocalizedText ? null : new XElement("SortName", banner.SortName),
                string.IsNullOrEmpty(banner.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(banner.Description, FileOutputOptions.DescriptionType)));
        }
    }
}
