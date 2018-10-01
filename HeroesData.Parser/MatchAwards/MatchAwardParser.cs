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
        /// Loads all unit id data to be parsed.
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
        /// Loads all unit id data to be parsed.
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

            foreach (XElement awardInstance in matchAwardsGeneral.Elements("Instances"))
            {
                if (awardInstance.Attribute("Id")?.Value == "[Default]")
                    continue;

                string gameLink = awardInstance.Element("GameLink").Attribute("GameLink").Value;

                XElement scoreValueCustomElement = GameData.XmlGameData.Root.Elements("CScoreValueCustom").FirstOrDefault(x => x.Attribute("id")?.Value == gameLink);
                string scoreScreenIconFileName = scoreValueCustomElement.Element("Icon").Attribute("value")?.Value;

                MatchAward matchAward = new MatchAward()
                {
                    Name = awardInstance.Attribute("Id")?.Value,
                    ShortName = XmlConvert.EncodeLocalName(Regex.Replace(awardInstance.Attribute("Id")?.Value, @"\s+", string.Empty)),
                    Id = gameLink.Substring(0, gameLink.IndexOf("Boolean")).Remove(0, "EndOfMatchAward".Length),
                    ScoreScreenImageFileName = Path.GetFileName(PathExtensions.GetFilePath(scoreScreenIconFileName)),
                    MVPScreenImageFileName = $"storm_ui_mvp_icons_rewards_{scoreScreenIconFileName.Split('_')[4]}.dds",
                    Tag = scoreValueCustomElement.Element("UniqueTag").Attribute("value")?.Value,
                };

                if (ParsedGameStrings.TryGetValuesFromAll($"{GameStringPrefixes.ScoreValueTooltipPrefix}{gameLink}", out string description))
                    matchAward.Description = description;

                MatchAwards.Add(matchAward);
            }
        }
    }
}
