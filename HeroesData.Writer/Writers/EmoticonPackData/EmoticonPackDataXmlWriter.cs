using Heroes.Models;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.EmoticonPackData
{
    internal class EmoticonPackDataXmlWriter : EmoticonPackDataWriter<XElement, XElement>
    {
        public EmoticonPackDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "EmoticonPacks";
        }

        protected override XElement MainElement(EmoticonPack emoticonPack)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(emoticonPack);

            return new XElement(
                XmlConvert.EncodeName(emoticonPack.Id),
                string.IsNullOrEmpty(emoticonPack.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", emoticonPack.Name),
                string.IsNullOrEmpty(emoticonPack.HyperlinkId) ? null : new XAttribute("hyperlinkId", emoticonPack.HyperlinkId),
                new XAttribute("rarity", emoticonPack.Rarity),
                string.IsNullOrEmpty(emoticonPack.CollectionCategory) ? null : new XAttribute("category", emoticonPack.CollectionCategory),
                string.IsNullOrEmpty(emoticonPack.EventName) ? null : new XAttribute("event", emoticonPack.EventName),
                emoticonPack.ReleaseDate.HasValue ? new XAttribute("releaseDate", emoticonPack.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null,
                string.IsNullOrEmpty(emoticonPack.SortName) || FileOutputOptions.IsLocalizedText ? null : new XElement("SortName", emoticonPack.SortName),
                string.IsNullOrEmpty(emoticonPack.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(emoticonPack.Description, FileOutputOptions.DescriptionType)),
                emoticonPack.EmoticonIds != null && emoticonPack.EmoticonIds.Any() ? new XElement("Emoticons", emoticonPack.EmoticonIds.Select(x => new XElement("Emoticon", x))) : null);
        }
    }
}
