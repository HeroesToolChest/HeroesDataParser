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
    public class VoiceLineParser : ParserBase<VoiceLine, VoiceLineDataOverride>, IParser<VoiceLine?, VoiceLineParser>
    {
        public VoiceLineParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CVoiceLine";

        public VoiceLineParser GetInstance()
        {
            return new VoiceLineParser(XmlDataService);
        }

        public VoiceLine? Parse(params string[] ids)
        {
            if (ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? voiceLineElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (voiceLineElement == null)
                return null;

            VoiceLine voiceLine = new VoiceLine()
            {
                Id = id,
            };

            SetDefaultValues(voiceLine);
            SetVoiceLineData(voiceLineElement, voiceLine);

            if (voiceLine.ReleaseDate == DefaultData.HeroData!.HeroReleaseDate)
                voiceLine.ReleaseDate = DefaultData.HeroData.HeroAlphaReleaseDate;

            if (string.IsNullOrEmpty(voiceLine.HyperlinkId))
                voiceLine.HyperlinkId = id;

            return voiceLine;
        }

        protected override bool ValidItem(XElement element)
        {
            return true;
        }

        private void SetVoiceLineData(XElement voiceLineElement, VoiceLine voiceLine, string? heroId = null)
        {
            // parent lookup
            string? parentValue = voiceLineElement.Attribute("parent")?.Value;

            if (voiceLineElement.HasElements && voiceLineElement.FirstNode?.GetType() == typeof(XProcessingInstruction))
            {
                XProcessingInstruction heroIdInstruction = (XProcessingInstruction)voiceLineElement.FirstNode;
                if (heroIdInstruction != null)
                {
                    XElement heroIdElement = XElement.Parse($"<HeroId {heroIdInstruction.Data}/>");
                    if (heroIdElement != null && string.IsNullOrEmpty(heroId) && heroIdElement.Attribute("id")?.Value == "heroid" && heroIdElement.Attribute("type")?.Value == "CHeroLink")
                        heroId = heroIdElement.Attribute("value")?.Value ?? string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue && x.Attribute("parent")?.Value != parentValue));
                if (parentElement != null)
                    SetVoiceLineData(parentElement, voiceLine, heroId);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.VoiceLineData?.VoiceLineDescription?.Replace(DefaultData.IdPlaceHolder, voiceLineElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(desc))
                    voiceLine.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in voiceLineElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "DESCRIPTION")
                {
                    string? descriptionValue = element.Attribute("value")?.Value;
                    if (descriptionValue is not null)
                    {
                        if (GameData.TryGetGameString(descriptionValue, out string? text))
                            voiceLine.Description = new TooltipDescription(text);
                    }
                }
                else if (elementName == "SORTNAME")
                {
                    string? sortNameValue = element.Attribute("value")?.Value;
                    if (sortNameValue is not null)
                    {
                        if (GameData.TryGetGameString(sortNameValue, out string? text))
                            voiceLine.SortName = text;
                    }
                }
                else if (elementName == "RELEASEDATE")
                {
                    if (!int.TryParse(element.Attribute("Day")?.Value, out int day))
                        day = DefaultData.VoiceLineData!.VoiceLineReleaseDate.Day;

                    if (!int.TryParse(element.Attribute("Month")?.Value, out int month))
                        month = DefaultData.VoiceLineData!.VoiceLineReleaseDate.Month;

                    if (!int.TryParse(element.Attribute("Year")?.Value, out int year))
                        year = DefaultData.VoiceLineData!.VoiceLineReleaseDate.Year;

                    voiceLine.ReleaseDate = new DateTime(year, month, day);
                }
                else if (elementName == "ATTRIBUTEID")
                {
                    voiceLine.AttributeId = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    voiceLine.HyperlinkId = element.Attribute("value")?.Value.Replace(DefaultData.HeroIdPlaceHolder, heroId, StringComparison.OrdinalIgnoreCase);
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            voiceLine.Name = text;
                    }
                }
                else if (elementName == "HERO")
                {
                    voiceLine.HeroId = element.Attribute("value")?.Value.Replace(DefaultData.HeroIdPlaceHolder, heroId, StringComparison.OrdinalIgnoreCase);
                }
                else if (elementName == "TILETEXTURE")
                {
                    voiceLine.ImageFileName = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value))?.ToLowerInvariant();

                    if (!string.IsNullOrEmpty(heroId))
                        voiceLine.ImageFileName = voiceLine.ImageFileName?.Replace(DefaultData.HeroIdPlaceHolder, heroId, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
                }
            }
        }

        private void SetDefaultValues(VoiceLine voiceLine)
        {
            voiceLine.Name = GameData.GetGameString(DefaultData.VoiceLineData?.VoiceLineName?.Replace(DefaultData.IdPlaceHolder, voiceLine.Id, StringComparison.OrdinalIgnoreCase));
            voiceLine.SortName = GameData.GetGameString(DefaultData.VoiceLineData?.VoiceLineSortName?.Replace(DefaultData.IdPlaceHolder, voiceLine.Id, StringComparison.OrdinalIgnoreCase));
            voiceLine.Description = new TooltipDescription(GameData.GetGameString(DefaultData.VoiceLineData?.VoiceLineDescription?.Replace(DefaultData.IdPlaceHolder, voiceLine.Id, StringComparison.OrdinalIgnoreCase)));
            voiceLine.HyperlinkId = string.Empty;
            voiceLine.ReleaseDate = DefaultData.VoiceLineData?.VoiceLineReleaseDate;
        }
    }
}
