using Heroes.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.RewardPortraitData
{
    internal class RewardPortraitDataXmlWriter : RewardPortraitDataWriter<XElement, XElement>
    {
        public RewardPortraitDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "RewardPortraits";
        }

        protected override XElement MainElement(RewardPortrait rewardPortrait)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(rewardPortrait);

            return new XElement(
                XmlConvert.EncodeName(rewardPortrait.Id),
                string.IsNullOrEmpty(rewardPortrait.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", rewardPortrait.Name),
                new XAttribute("hyperlinkId", rewardPortrait.HyperlinkId),
                new XAttribute("rarity", rewardPortrait.Rarity),
                string.IsNullOrEmpty(rewardPortrait.HeroId) ? null : new XAttribute("heroId", rewardPortrait.HeroId),
                new XAttribute("iconSlot", rewardPortrait.IconSlot),
                string.IsNullOrEmpty(rewardPortrait.PortraitPackId) ? null : new XAttribute("PortraitPackId", rewardPortrait.PortraitPackId),
                string.IsNullOrEmpty(rewardPortrait.CollectionCategory) ? null : new XAttribute("category", rewardPortrait.CollectionCategory),
                string.IsNullOrEmpty(rewardPortrait.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(rewardPortrait.Description, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(rewardPortrait.DescriptionUnearned?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("DescriptionUnearned", GetTooltip(rewardPortrait.DescriptionUnearned, FileOutputOptions.DescriptionType)),
                GetImageObject(rewardPortrait));
        }

        protected override XElement GetImageObject(RewardPortrait rewardPortrait)
        {
            return new XElement(
                "TextureSheet",
                new XElement("Image", Path.ChangeExtension(rewardPortrait.TextureSheet.Image?.ToLowerInvariant(), StaticImageExtension)),
                rewardPortrait.TextureSheet.Columns.HasValue ? new XElement("Columns", rewardPortrait.TextureSheet.Columns.Value) : null,
                rewardPortrait.TextureSheet.Rows.HasValue ? new XElement("Rows", rewardPortrait.TextureSheet.Rows.Value) : null);
        }
    }
}
