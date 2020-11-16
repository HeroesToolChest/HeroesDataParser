using Heroes.Models;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.PortraitPackData
{
    internal class PortraitPackDataXmlWriter : PortraitPackDataWriter<XElement, XElement>
    {
        public PortraitPackDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "PortraitPacks";
        }

        protected override XElement MainElement(PortraitPack portrait)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(portrait);

            return new XElement(
                XmlConvert.EncodeName(portrait.Id),
                string.IsNullOrEmpty(portrait.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", portrait.Name),
                string.IsNullOrEmpty(portrait.HyperlinkId) ? null! : new XAttribute("hyperlinkId", portrait.HyperlinkId),
                new XAttribute("rarity", portrait.Rarity),
                string.IsNullOrEmpty(portrait.EventName) ? null! : new XAttribute("event", portrait.EventName),
                string.IsNullOrEmpty(portrait.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", portrait.SortName),
                portrait.RewardPortraitIds != null && portrait.RewardPortraitIds.Any() ? new XElement("RewardPortraitIds", portrait.RewardPortraitIds.Select(x => new XElement("RewardPortraitId", x))) : null!);
        }
    }
}
