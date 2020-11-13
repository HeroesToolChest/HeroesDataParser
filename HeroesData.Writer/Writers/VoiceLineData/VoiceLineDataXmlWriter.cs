using Heroes.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.VoiceLineData
{
    internal class VoiceLineDataXmlWriter : VoiceLineDataWriter<XElement, XElement>
    {
        public VoiceLineDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "VoiceLines";
        }

        protected override XElement MainElement(VoiceLine voiceLine)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(voiceLine);

            return new XElement(
                XmlConvert.EncodeName(voiceLine.Id),
                string.IsNullOrEmpty(voiceLine.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", voiceLine.Name),
                new XAttribute("hyperlinkId", voiceLine.HyperlinkId),
                string.IsNullOrEmpty(voiceLine.AttributeId) ? null! : new XAttribute("attributeId", voiceLine.AttributeId),
                new XAttribute("rarity", voiceLine.Rarity),
                voiceLine.ReleaseDate.HasValue ? new XAttribute("releaseDate", voiceLine.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null!,
                string.IsNullOrEmpty(voiceLine.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", voiceLine.SortName),
                string.IsNullOrEmpty(voiceLine.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("Description", GetTooltip(voiceLine.Description, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(voiceLine.ImageFileName) ? null! : new XElement("Image", Path.ChangeExtension(voiceLine.ImageFileName?.ToLowerInvariant(), StaticImageExtension)));
        }
    }
}
