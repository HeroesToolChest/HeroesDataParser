using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class AnnouncerParser : ParserBase<Announcer, AnnouncerDataOverride>, IParser<Announcer, AnnouncerParser>
    {
        public AnnouncerParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CAnnouncerPack";

        public AnnouncerParser GetInstance()
        {
            return new AnnouncerParser(XmlDataService);
        }

        public Announcer Parse(params string[] ids)
        {
            if (ids == null || ids.Count() < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement announcerPackElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (announcerPackElement == null)
                return null;

            Announcer announcer = new Announcer()
            {
                Id = id,
            };

            SetDefaultValues(announcer);
            SetAnnouncerData(announcerPackElement, announcer);

            if (announcer.ReleaseDate == DefaultData.HeroData.HeroReleaseDate)
                announcer.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(announcer.HyperlinkId))
                announcer.HyperlinkId = id;

            return announcer;
        }

        protected override bool ValidItem(XElement element)
        {
            return element.Element("AttributeId") != null;
        }

        private void SetAnnouncerData(XElement announcerPackElement, Announcer announcer, string heroId = null)
        {
            // parent lookup
            string parentValue = announcerPackElement.Attribute("parent")?.Value;
            string heroIdValue = announcerPackElement.Attribute("heroid")?.Value;

            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetAnnouncerData(parentElement, announcer, heroIdValue);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.AnnouncerData.AnnouncerDescription.Replace(DefaultData.IdPlaceHolder, announcerPackElement.Attribute("id")?.Value));
                if (!string.IsNullOrEmpty(desc))
                    announcer.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in announcerPackElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpper();

                if (elementName == "INFOTEXT")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        announcer.Description = new TooltipDescription(text);
                }
                else if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        announcer.SortName = text;
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.MountData.MountReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.MountData.MountReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.MountData.MountReleaseDate.Year;

                    announcer.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    announcer.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    announcer.HyperlinkId = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(heroId))
                        announcer.HyperlinkId = announcer.HyperlinkId.Replace(DefaultData.HeroIdPlaceHolder, heroId);
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity heroRarity))
                        announcer.Rarity = heroRarity;
                    else
                        announcer.Rarity = Rarity.Unknown;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value, out string text))
                        announcer.Name = text;
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    announcer.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "TILETEXTURE")
                {
                    announcer.ImageFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value)).ToLower();

                    if (!string.IsNullOrEmpty(heroId))
                        announcer.ImageFileName = announcer.ImageFileName.Replace(DefaultData.HeroIdPlaceHolder, heroId).ToLower();
                }
                else if (elementName == "HERO")
                {
                    announcer.HeroId = element.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(heroId))
                        announcer.HeroId = announcer.HeroId.Replace(DefaultData.HeroIdPlaceHolder, heroId);
                }
                else if (elementName == "GENDER")
                {
                    announcer.Gender = element.Attribute("value")?.Value;
                }
            }
        }

        private void SetDefaultValues(Announcer announcer)
        {
            announcer.Name = GameData.GetGameString(DefaultData.AnnouncerData.AnnouncerName.Replace(DefaultData.IdPlaceHolder, announcer.Id));
            announcer.SortName = GameData.GetGameString(DefaultData.AnnouncerData.AnnouncerSortName.Replace(DefaultData.IdPlaceHolder, announcer.Id));
            announcer.Description = new TooltipDescription(GameData.GetGameString(DefaultData.AnnouncerData.AnnouncerDescription.Replace(DefaultData.IdPlaceHolder, announcer.Id)));
            announcer.ReleaseDate = DefaultData.AnnouncerData.AnnouncerReleaseDate;
            announcer.Rarity = Rarity.None;
        }
    }
}
