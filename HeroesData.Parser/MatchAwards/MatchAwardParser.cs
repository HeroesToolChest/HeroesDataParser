using Heroes.Models;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.Parser.MatchAwards
{
    public class MatchAwardParser
    {
        private readonly int? HotsBuild;
        private readonly GameData GameData;
        private readonly ParsedGameStrings ParsedGameStrings;

        private readonly Dictionary<string, MatchAward> MatchAwards = new Dictionary<string, MatchAward>();

        private int ParsedCount = 0;

        public MatchAwardParser(GameData gameData, ParsedGameStrings parsedGameStrings)
        {
            GameData = gameData;
            ParsedGameStrings = parsedGameStrings;
        }

        public MatchAwardParser(GameData gameData, ParsedGameStrings parsedGameStrings, int? hotsBuild)
        {
            GameData = gameData;
            ParsedGameStrings = parsedGameStrings;
            HotsBuild = hotsBuild;
        }

        /// <summary>
        /// Returns all parsed match awards.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MatchAward> GetParsedMatchAwards()
        {
            return MatchAwards.Values;
        }

        /// <summary>
        /// Returns the total count of parsed awards.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return ParsedCount;
        }

        /// <summary>
        /// Parse all match awards.
        /// </summary>
        public void Parse()
        {
            XElement matchAwardsGeneral = GameData.XmlGameData.Root.Elements("CUser").FirstOrDefault(x => x.Attribute("id")?.Value == "EndOfMatchGeneralAward");
            IEnumerable<XElement> matchAwardsMapSpecific = GameData.XmlGameData.Root.Elements("CUser").Where(x => x.Attribute("id")?.Value == "EndOfMatchMapSpecificAward");

            // combine both
            IEnumerable<XElement> mapAwardsInstances = matchAwardsMapSpecific.Elements("Instances").Concat(matchAwardsGeneral.Elements("Instances"));

            foreach (XElement awardInstance in mapAwardsInstances)
            {
                string instanceId = awardInstance.Attribute("Id")?.Value;

                if (instanceId == "[Default]" || !awardInstance.HasElements)
                    continue;

                string gameLink = awardInstance.Element("GameLink").Attribute("GameLink").Value;

                ParseAward(instanceId, gameLink);
            }

            // manually add mvp award
            ParseAward("[Override]Generic Instance", "EndOfMatchAwardMVPBoolean");
        }

        private void ParseAward(string instanceId, string gameLink)
        {
            if (instanceId == "[Override]Generic Instance")
            {
                // get the name
                if (ParsedGameStrings.TryGetValuesFromAll($"{GameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}{gameLink}", out string awardNameText))
                    instanceId = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);
                else if (ParsedGameStrings.TryGetValuesFromAll($"{GameStringPrefixes.MatchAwardInstanceNamePrefix}{gameLink}", out awardNameText))
                    instanceId = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);
            }

            XElement scoreValueCustomElement = GameData.XmlGameData.Root.Elements("CScoreValueCustom").FirstOrDefault(x => x.Attribute("id")?.Value == gameLink);
            string scoreScreenIconFilePath = scoreValueCustomElement.Element("Icon").Attribute("value")?.Value;

            // get the name being used in the dds file
            string awardSpecialName = Path.GetFileName(PathExtensions.GetFilePath(scoreScreenIconFilePath)).Split('_')[4];

            // set some correct names for looking up the icons
            if (awardSpecialName == "hattrick")
                awardSpecialName = "hottrick";
            else if (awardSpecialName == "skull")
                awardSpecialName = "dominator";

            MatchAward matchAward = new MatchAward()
            {
                Name = instanceId,
                ShortName = XmlConvert.EncodeLocalName(Regex.Replace(instanceId, @"\s+", string.Empty)),
                ScoreScreenImageFileNameOriginal = Path.GetFileName(PathExtensions.GetFilePath(scoreScreenIconFilePath)),
                MVPScreenImageFileNameOriginal = $"storm_ui_mvp_icons_rewards_{awardSpecialName}.dds",
                Tag = scoreValueCustomElement.Element("UniqueTag").Attribute("value")?.Value,
            };

            string id = gameLink;
            if (id.StartsWith("EndOfMatchAward"))
                id = id.Remove(0, "EndOfMatchAward".Length);
            if (id.EndsWith("Boolean"))
                id = id.Substring(0, id.IndexOf("Boolean"));
            if (id.StartsWith("0"))
                id = id.ReplaceFirst("0", "Zero");

            matchAward.Id = id;

            // set new image file names for the extraction
            // change it back to the correct spelling
            if (awardSpecialName == "hottrick")
                awardSpecialName = "hattrick";

            matchAward.ScoreScreenImageFileName = matchAward.ScoreScreenImageFileNameOriginal.ToLower();
            matchAward.MVPScreenImageFileName = $"storm_ui_mvp_{awardSpecialName}_%color%.dds".ToLower();

            if (ParsedGameStrings.TryGetValuesFromAll($"{GameStringPrefixes.ScoreValueTooltipPrefix}{gameLink}", out string description))
                matchAward.Description = new TooltipDescription(description);

            MatchAwards[matchAward.Id] = matchAward;
            ParsedCount++;
        }

        private string GetNameFromGenderRule(string name)
        {
            string[] parts = name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                return parts[0];
            }

            return name;
        }
    }
}
