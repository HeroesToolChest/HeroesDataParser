using Heroes.Models;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.EmoticonData
{
    internal class EmoticonDataXmlWriter : EmoticonDataWriter<XElement, XElement>
    {
        public EmoticonDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Emoticons";
        }

        protected override XElement MainElement(Emoticon emoticon)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(emoticon);

            return new XElement(
                XmlConvert.EncodeName(emoticon.Id),
                string.IsNullOrEmpty(emoticon.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("expression", emoticon.Name),
                string.IsNullOrEmpty(emoticon.HyperlinkId) ? null! : new XAttribute("hyperlinkId", emoticon.HyperlinkId),
                emoticon.IsAliasCaseSensitive == true ? new XAttribute("caseSensitive", true) : null!,
                emoticon.IsHidden == true ? new XAttribute("isHidden", true) : null!,
                emoticon.SearchTexts == null! || !emoticon.SearchTexts.Any() || FileOutputOptions.IsLocalizedText ? null! : new XElement("SearchText", string.Join(' ', emoticon.SearchTexts)),
                string.IsNullOrEmpty(emoticon.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("Description", GetTooltip(emoticon.Description, FileOutputOptions.DescriptionType)),
                string.IsNullOrEmpty(emoticon.DescriptionLocked?.RawDescription) || FileOutputOptions.IsLocalizedText ? null! : new XElement("DescriptionLocked", GetTooltip(emoticon.DescriptionLocked, FileOutputOptions.DescriptionType)),
                emoticon.LocalizedAliases != null! && emoticon.LocalizedAliases.Any() && !FileOutputOptions.IsLocalizedText ? new XElement("LocalizedAliases", emoticon.LocalizedAliases.Select(x => new XElement("Alias", x))) : null!,
                emoticon.UniversalAliases != null! && emoticon.UniversalAliases.Any() ? new XElement("Aliases", emoticon.UniversalAliases.Select(x => new XElement("Alias", x))) : null!,
                HeroElement(emoticon)!,
                string.IsNullOrEmpty(emoticon.Image.FileName) ? null! : new XElement("Image", !emoticon.Image.Count.HasValue ? Path.ChangeExtension(emoticon.Image.FileName?.ToLowerInvariant(), StaticImageExtension) : Path.ChangeExtension(emoticon.Image.FileName?.ToLowerInvariant(), AnimatedImageExtension)),
                AnimationObject(emoticon)!);
        }

        protected override XElement GetHeroElement(Emoticon emoticon)
        {
            return new XElement(
                "Hero",
                new XAttribute("id", emoticon.HeroId!),
                string.IsNullOrEmpty(emoticon.HeroSkinId) ? null! : new XAttribute("skinId", emoticon.HeroSkinId));
        }

        protected override XElement GetAnimationObject(Emoticon emoticon)
        {
            return new XElement(
                "Animation",
                new XElement("Texture", Path.ChangeExtension(emoticon.TextureSheet.Image?.ToLowerInvariant(), StaticImageExtension)),
                new XElement("Frames", emoticon.Image.Count),
                new XElement("Duration", emoticon.Image.DurationPerFrame),
                new XElement("Width", emoticon.Image.Width),
                emoticon.TextureSheet.Columns.HasValue ? new XElement("Columns", emoticon.TextureSheet.Columns.Value) : null!,
                emoticon.TextureSheet.Rows.HasValue ? new XElement("Rows", emoticon.TextureSheet.Rows.Value) : null!);
        }
    }
}
