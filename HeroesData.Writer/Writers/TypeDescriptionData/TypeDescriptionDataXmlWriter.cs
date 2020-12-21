using Heroes.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.TypeDescriptionData
{
    internal class TypeDescriptionDataXmlWriter : TypeDescriptionDataWriter<XElement, XElement>
    {
        public TypeDescriptionDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "TypeDescriptions";
        }

        protected override XElement MainElement(TypeDescription typeDescription)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(typeDescription);

            return new XElement(
                XmlConvert.EncodeName(typeDescription.Id),
                string.IsNullOrEmpty(typeDescription.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", typeDescription.Name),
                string.IsNullOrEmpty(typeDescription.HyperlinkId) ? null! : new XAttribute("hyperlinkId", typeDescription.HyperlinkId),
                new XElement("IconSlot", typeDescription.IconSlot),
                GetImageObject(typeDescription),
                string.IsNullOrEmpty(typeDescription.ImageFileName) ? null! : new XElement("Image", Path.ChangeExtension(typeDescription.ImageFileName.ToLowerInvariant(), StaticImageExtension)));
        }

        protected override XElement GetImageObject(TypeDescription typeDescription)
        {
            return new XElement(
                "TextureSheet",
                new XElement("Image", Path.ChangeExtension(typeDescription.TextureSheet.Image?.ToLowerInvariant(), StaticImageExtension)),
                typeDescription.TextureSheet.Columns.HasValue ? new XElement("Columns", typeDescription.TextureSheet.Columns.Value) : null!,
                typeDescription.TextureSheet.Rows.HasValue ? new XElement("Rows", typeDescription.TextureSheet.Rows.Value) : null!);
        }
    }
}
