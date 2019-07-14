using Heroes.Models;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.PortraitData
{
    internal class PortraitDataXmlWriter : PortraitDataWriter<XElement, XElement>
    {
        public PortraitDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Portraits";
        }

        protected override XElement MainElement(Portrait portrait)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(portrait);

            return new XElement(
                XmlConvert.EncodeName(portrait.Id),
                string.IsNullOrEmpty(portrait.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", portrait.Name),
                new XAttribute("hyperlinkId", portrait.HyperlinkId),
                string.IsNullOrEmpty(portrait.EventName) ? null : new XAttribute("event", portrait.EventName),
                string.IsNullOrEmpty(portrait.SortName) || FileOutputOptions.IsLocalizedText ? null : new XElement("SortName", portrait.SortName));
        }
    }
}
