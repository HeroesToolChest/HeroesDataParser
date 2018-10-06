using Heroes.Models;
using HeroesData.Parser.GameStrings;
using HeroesData.Parser.XmlGameData;
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

        private MatchAwardParser(GameData gameData, ParsedGameStrings parsedGameStrings)
        {
            GameData = gameData;
            ParsedGameStrings = parsedGameStrings;

            Initialize();
        }

        private MatchAwardParser(GameData gameData, ParsedGameStrings parsedGameStrings, int? hotsBuild)
        {
            GameData = gameData;
            ParsedGameStrings = parsedGameStrings;
            HotsBuild = hotsBuild;

            Initialize();
        }

        public List<MatchAward> MatchAwards { get; set; } = new List<MatchAward>();

        /// <summary>
        /// Loads all match awards data to be parsed.
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="gameStringParser"></param>
        /// <param name="overrideData"></param>
        /// <returns></returns>
        public static MatchAwardParser Load(GameData gameData, ParsedGameStrings parsedGameStrings)
        {
            return new MatchAwardParser(gameData, parsedGameStrings);
        }

        /// <summary>
        /// Loads all match awards data to be parsed.
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="gameStringParser"></param>
        /// <param name="overrideData"></param>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <returns></returns>
        public static MatchAwardParser Load(GameData gameData, ParsedGameStrings parsedGameStrings, int? hotsBuild)
        {
            return new MatchAwardParser(gameData, parsedGameStrings, hotsBuild);
        }

        private void Initialize()
        {
            GetAwardInstances();
        }

        private void GetAwardInstances()
        {
            XElement matchAwardsGeneral = GameData.XmlGameData.Root.Elements("CUser").FirstOrDefault(x => x.Attribute("id")?.Value == "EndOfMatchGeneralAward");
            IEnumerable<XElement> matchAwardsMapSpecific = GameData.XmlGameData.Root.Elements("CUser").Where(x => x.Attribute("id")?.Value == "EndOfMatchMapSpecificAward");

            // combine both
            IEnumerable<XElement> mapAwardsInstances = matchAwardsMapSpecific.Elements("Instances").Concat(matchAwardsGeneral.Elements("Instances"));

            foreach (XElement awardInstance in mapAwardsInstances)
            {
                string instanceId = awardInstance.Attribute("Id")?.Value;

                if (instanceId == "[Default]" || !awardInstance.HasElements)
                {
                    continue;
                }
                else if (instanceId == "[Override]Generic Instance")
                {
                    // get the name
                    if (ParsedGameStrings.TryGetValuesFromAll($"UserData/EndOfMatchMapSpecificAward/[Override]Generic Instance_Award Name", out string awardNameText))
                        instanceId = new TooltipDescription(awardNameText).PlainText;
                }

                string gameLink = awardInstance.Element("GameLink").Attribute("GameLink").Value;

                XElement scoreValueCustomElement = GameData.XmlGameData.Root.Elements("CScoreValueCustom").FirstOrDefault(x => x.Attribute("id")?.Value == gameLink);
                string scoreScreenIconFilePath = scoreValueCustomElement.Element("Icon").Attribute("value")?.Value;
                string awardSpecialName = Path.GetFileName(PathExtensions.GetFilePath(scoreScreenIconFilePath)).Split('_')[4];

                MatchAward matchAward = new MatchAward()
                {
                    Name = instanceId,
                    ShortName = XmlConvert.EncodeLocalName(Regex.Replace(instanceId, @"\s+", string.Empty)),
                    Id = gameLink.Substring(0, gameLink.IndexOf("Boolean")).Remove(0, "EndOfMatchAward".Length),
                    ScoreScreenImageFileNameOriginal = Path.GetFileName(PathExtensions.GetFilePath(scoreScreenIconFilePath)),
                    MVPScreenImageFileNameOriginal = $"storm_ui_mvp_icons_rewards_{awardSpecialName}.dds",
                    Tag = scoreValueCustomElement.Element("UniqueTag").Attribute("value")?.Value,
                };

                matchAward.ScoreScreenImageFileName = matchAward.ScoreScreenImageFileNameOriginal;
                matchAward.MVPScreenImageFileName = $"storm_ui_mvp_{awardSpecialName}_%color%.dds";

                if (ParsedGameStrings.TryGetValuesFromAll($"{GameStringPrefixes.ScoreValueTooltipPrefix}{gameLink}", out string description))
                    matchAward.Description = new TooltipDescription(description);

                MatchAwards.Add(matchAward);
            }
        }
    }
}
