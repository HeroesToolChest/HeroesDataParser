using Heroes.Models;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.BoostData
{
    internal class BoostDataXmlWriter : BoostDataWriter<XElement, XElement>
    {
        public BoostDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Boosts";
        }

        protected override XElement MainElement(Boost boost)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(boost);

            return new XElement(
                XmlConvert.EncodeName(boost.Id),
                string.IsNullOrEmpty(boost.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", boost.Name),
                string.IsNullOrEmpty(boost.HyperlinkId) ? null! : new XAttribute("hyperlinkId", boost.HyperlinkId),
                boost.ReleaseDate.HasValue ? new XAttribute("releaseDate", boost.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null!,
                string.IsNullOrEmpty(boost.EventName) ? null! : new XAttribute("event", boost.EventName),
                string.IsNullOrEmpty(boost.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", boost.SortName));
        }
    }
}
