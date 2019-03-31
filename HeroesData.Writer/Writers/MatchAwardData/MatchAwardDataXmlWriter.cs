using Heroes.Models;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.MatchAwardData
{
    internal class MatchAwardDataXmlWriter : MatchAwardDataWriter<XElement, XElement>
    {
        public MatchAwardDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Awards";
        }

        protected override XElement MainElement(MatchAward matchAward)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(matchAward);

            return new XElement(
                matchAward.Id,
                string.IsNullOrEmpty(matchAward.Name) || FileOutputOptions.IsLocalizedText ? null : new XAttribute("name", matchAward.Name),
                string.IsNullOrEmpty(matchAward.HyperlinkId) ? null : new XAttribute("gameLink", matchAward.HyperlinkId),
                new XAttribute("tag", matchAward.Tag),
                new XElement("MVPScreenIcon", Path.ChangeExtension(matchAward.MVPScreenImageFileName, StaticImageExtension)),
                new XElement("ScoreScreenIcon", Path.ChangeExtension(matchAward.ScoreScreenImageFileName, StaticImageExtension)),
                FileOutputOptions.IsLocalizedText ? null : new XElement("Description", GetTooltip(matchAward.Description, FileOutputOptions.DescriptionType)));
        }
    }
}
