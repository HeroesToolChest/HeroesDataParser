using Heroes.Models;
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
                XmlConvert.EncodeName(mount.ShortName),
                string.IsNullOrEmpty(mount.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", mount.Name),
                new XAttribute("mountId", mount.MountId),
                new XAttribute("attributeId", mount.AttributeId),
                new XAttribute("rarity", mount.Rarity),
                mount.ReleaseDate.HasValue ? new XAttribute("releaseDate", mount.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                string.IsNullOrEmpty(mount.SortName) || FileOutputOptions.IsLocalizedText ? null : new XElement("SortName", mount.SortName),
                string.IsNullOrEmpty(mount.SearchText) || FileOutputOptions.IsLocalizedText ? null : new XElement("SearchText", mount.SearchText),
                string.IsNullOrEmpty(mount.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(mount.Description, FileOutputOptions.DescriptionType)));
        }
    }
}
