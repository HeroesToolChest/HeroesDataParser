using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class PortraitParser : ParserBase<Portrait, PortraitDataOverride>, IParser<Portrait?, PortraitParser>
    {
        public PortraitParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CPortraitPack";

        public PortraitParser GetInstance()
        {
            return new PortraitParser(XmlDataService);
        }

        public Portrait? Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement? portraitElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (portraitElement == null)
                return null;

            Portrait portrait = new Portrait()
            {
                Id = id,
            };

            SetDefaultValues(portrait);
            SetPortraitData(portraitElement, portrait);

            if (string.IsNullOrEmpty(portrait.HyperlinkId))
                portrait.HyperlinkId = id;

            return portrait;
        }

        private void SetPortraitData(XElement portraitElement, Portrait portrait)
        {
            // parent lookup
            string? parentValue = portraitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetPortraitData(parentElement, portrait);
            }

            foreach (XElement element in portraitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        portrait.SortName = text;
                }
                else if (elementName == "HYPERLINKID")
                {
                    portrait.HyperlinkId = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        portrait.Name = text;
                }
                else if (elementName == "EVENTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        portrait.EventName = text;
                    else
                        portrait.EventName = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Portrait portrait)
        {
            portrait.Name = GameData.GetGameString(DefaultData.PortraitPackData?.PortraitName?.Replace(DefaultData.IdPlaceHolder, portrait.Id));
            portrait.SortName = GameData.GetGameString(DefaultData.PortraitPackData?.PortraitSortName?.Replace(DefaultData.IdPlaceHolder, portrait.Id));
            portrait.HyperlinkId = DefaultData.PortraitPackData?.PortraitHyperlinkId?.Replace(DefaultData.IdPlaceHolder, portrait.Id) ?? string.Empty;
        }
    }
}
