using Heroes.Models;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.MountData
{
    internal class MountDataXmlWriter : MountDataWriter<XElement, XElement>
    {
        public MountDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Mounts";
        }

        protected override XElement MainElement(Mount mount)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(mount);

            return new XElement(
                XmlConvert.EncodeName(mount.Id),
                string.IsNullOrEmpty(mount.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", mount.Name),
                string.IsNullOrEmpty(mount.HyperlinkId) ? null! : new XAttribute("hyperlinkId", mount.HyperlinkId),
                string.IsNullOrEmpty(mount.AttributeId) ? null! : new XAttribute("attributeId", mount.AttributeId),
                new XAttribute("rarity", mount.Rarity),
                new XAttribute("type", mount.MountCategory!),
                new XAttribute("category", mount.CollectionCategory!),
                new XAttribute("franchise", mount.Franchise),
                string.IsNullOrEmpty(mount.EventName) ? null! : new XAttribute("event", mount.EventName),
                mount.ReleaseDate.HasValue ? new XAttribute("releaseDate", mount.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null!,
                string.IsNullOrEmpty(mount.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", mount.SortName),
                string.IsNullOrEmpty(mount.SearchText) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SearchText", mount.SearchText),
                string.IsNullOrEmpty(mount.InfoText?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("InfoText", GetTooltip(mount.InfoText, FileOutputOptions.DescriptionType)),
                mount.VariationMountIds.Count > 0 ? new XElement("VariationMountIds", mount.VariationMountIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(d => new XElement("VariationMountId", d))) : null!);
        }
    }
}
