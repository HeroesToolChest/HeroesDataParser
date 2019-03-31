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
            RootNodeName = "Emoticon";
        }

        protected override XElement MainElement(Emoticon emoticon)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(emoticon);

            return new XElement(
                XmlConvert.EncodeName(emoticon.Id),
                string.IsNullOrEmpty(emoticon.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("expression", emoticon.Name),
                string.IsNullOrEmpty(emoticon.HyperlinkId) ? null : new XAttribute("hyperlinkId", emoticon.HyperlinkId),
                emoticon.IsAliasCaseSensitive == true ? new XAttribute("caseSensitive", true) : null,
                (emoticon.SearchTexts == null || emoticon.SearchTexts.Count < 1) || FileOutputOptions.IsLocalizedText ? null : new XElement("SearchText", string.Join(' ', emoticon.SearchTexts)),
                string.IsNullOrEmpty(emoticon.Description?.RawDescription) || FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(emoticon.Description, FileOutputOptions.DescriptionType)),
                emoticon.LocalizedAliases != null && emoticon.LocalizedAliases.Count > 0 && !FileOutputOptions.IsLocalizedText ? new XElement("LocalizedAliases", emoticon.LocalizedAliases.Select(x => new XElement("Alias", x))) : null,
                emoticon.UniversalAliases != null && emoticon.UniversalAliases.Count > 0 ? new XElement("Aliases", emoticon.UniversalAliases.Select(x => new XElement("Alias", x))) : null,
                HeroElement(emoticon),
                string.IsNullOrEmpty(emoticon.Image.FileName) ? null : new XElement("Image", !emoticon.Image.Count.HasValue ? Path.ChangeExtension(emoticon.Image.FileName, StaticImageExtension) : Path.ChangeExtension(emoticon.Image.FileName, AnimatedImageExtension)));
        }

        protected override XElement GetHeroElement(Emoticon emoticon)
        {
            return new XElement(
                "Hero",
                new XAttribute("id", emoticon.HeroId),
                string.IsNullOrEmpty(emoticon.HeroSkinId) ? null : new XAttribute("skinId", emoticon.HeroSkinId));
        }
    }
}
