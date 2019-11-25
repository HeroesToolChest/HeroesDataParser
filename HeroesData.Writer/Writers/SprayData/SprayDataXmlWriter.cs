using Heroes.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.SprayData
{
    internal class SprayDataXmlWriter : SprayDataWriter<XElement, XElement>
    {
        public SprayDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Sprays";
        }

        protected override XElement MainElement(Spray spray)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(spray);

            return new XElement(
                XmlConvert.EncodeName(spray.Id),
                string.IsNullOrEmpty(spray.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", spray.Name),
                new XAttribute("hyperlinkId", spray.HyperlinkId),
                new XAttribute("attributeId", spray.AttributeId),
                new XAttribute("rarity", spray.Rarity),
                string.IsNullOrEmpty(spray.CollectionCategory) ? null : new XAttribute("category", spray.CollectionCategory),
                string.IsNullOrEmpty(spray.EventName) ? null : new XAttribute("event", spray.EventName),
                spray.ReleaseDate.HasValue ? new XAttribute("releaseDate", spray.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                string.IsNullOrEmpty(spray.SortName) || FileOutputOptions.IsLocalizedText ? null : new XElement("SortName", spray.SortName),
                string.IsNullOrEmpty(spray.SearchText) || FileOutputOptions.IsLocalizedText ? null : new XElement("SearchText", spray.SearchText),
                string.IsNullOrEmpty(spray.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(spray.Description, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(spray.ImageFileName) ? null : new XElement("Image", spray.AnimationCount < 1 ? Path.ChangeExtension(spray.ImageFileName?.ToLower(), StaticImageExtension) : Path.ChangeExtension(spray.ImageFileName?.ToLower(), AnimatedImageExtension)),
                AnimationObject(spray));
        }

        protected override XElement GetAnimationObject(Spray spray)
        {
            return new XElement(
                "Animation",
                new XElement("Texture", Path.ChangeExtension(spray.ImageFileName?.ToLower(), StaticImageExtension)),
                new XElement("Frames", spray.AnimationCount),
                new XElement("Duration", spray.AnimationDuration));
        }
    }
}
