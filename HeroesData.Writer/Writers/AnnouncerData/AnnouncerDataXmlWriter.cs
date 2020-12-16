using Heroes.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.AnnouncerData
{
    internal class AnnouncerDataXmlWriter : AnnouncerDataWriter<XElement, XElement>
    {
        public AnnouncerDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Announcers";
        }

        protected override XElement MainElement(Announcer announcer)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(announcer);

            return new XElement(
                XmlConvert.EncodeName(announcer.Id),
                string.IsNullOrEmpty(announcer.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", announcer.Name),
                string.IsNullOrEmpty(announcer.HyperlinkId) ? null! : new XAttribute("hyperlinkId", announcer.HyperlinkId),
                string.IsNullOrEmpty(announcer.AttributeId) ? null! : new XAttribute("attributeId", announcer.AttributeId),
                new XAttribute("rarity", announcer.Rarity),
                new XAttribute("category", announcer.CollectionCategory!),
                string.IsNullOrEmpty(announcer.Gender) ? null! : new XAttribute("gender", announcer.Gender),
                new XAttribute("heroId", announcer.HeroId!),
                announcer.ReleaseDate.HasValue ? new XAttribute("releaseDate", announcer.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null!,
                string.IsNullOrEmpty(announcer.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", announcer.SortName),
                string.IsNullOrEmpty(announcer.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("Description", GetTooltip(announcer.Description, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(announcer.ImageFileName) ? null! : new XElement("Image", Path.ChangeExtension(announcer.ImageFileName.ToLowerInvariant(), StaticImageExtension)));
        }
    }
}
