using Heroes.Models;
using System.IO;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writer.MatchAwardData
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
            if (IsLocalizedText)
                AddLocalizedGameString(matchAward);

            return new XElement(
                matchAward.ShortName,
                string.IsNullOrEmpty(matchAward.Name) || IsLocalizedText ? null : new XAttribute("name", matchAward.Name),
                new XAttribute("tag", matchAward.Tag),
                new XElement("MVPScreenIcon", Path.ChangeExtension(matchAward.MVPScreenImageFileName, FileSettings.ImageExtension)),
                new XElement("ScoreScreenIcon", Path.ChangeExtension(matchAward.ScoreScreenImageFileName, FileSettings.ImageExtension)),
                IsLocalizedText ? null : new XElement("Description", GetTooltip(matchAward.Description, FileSettings.DescriptionType)));
        }
    }
}
