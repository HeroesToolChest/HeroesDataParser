using Heroes.Models;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.LootChestData
{
    internal class LootChestDataXmlWriter : LootChestDataWriter<XElement, XElement>
    {
        public LootChestDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "LootChests";
        }

        protected override XElement MainElement(LootChest lootChest)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(lootChest);

            return new XElement(
                XmlConvert.EncodeName(lootChest.Id),
                string.IsNullOrEmpty(lootChest.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", lootChest.Name),
                string.IsNullOrEmpty(lootChest.HyperlinkId) ? null! : new XAttribute("hyperlinkId", lootChest.HyperlinkId),
                new XAttribute("rarity", lootChest.Rarity),
                new XAttribute("maxRerolls", lootChest.MaxRerolls),
                string.IsNullOrEmpty(lootChest.EventName) ? null! : new XAttribute("event", lootChest.EventName),
                lootChest.TypeDescription is null ? null! : new XElement("TypeDescription", lootChest.TypeDescription),
                string.IsNullOrEmpty(lootChest.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("Description", GetTooltip(lootChest.Description, FileOutputOptions.DescriptionType)));
        }
    }
}
