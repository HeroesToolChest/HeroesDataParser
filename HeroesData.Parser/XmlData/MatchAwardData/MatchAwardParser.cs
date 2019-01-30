using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData.MatchAwardData
{
    public class MatchAwardParser : IParsableXmlData
    {
        private readonly GameData GameData;

        public MatchAwardParser(GameData gameData)
        {
            GameData = gameData;
        }

        /// <summary>
        /// Gets or sets the hots build number.
        /// </summary>
        public int? HotsBuild { get; set; }

        /// <summary>
        /// Parse all match awards.
        /// </summary>
        public IEnumerable<MatchAward> Parse()
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

                yield return ParseAward(instanceId, gameLink);
            }

            // manually add mvp award
            MatchAward mvpAward = ParseAward("[Override]Generic Instance", "EndOfMatchAwardMVPBoolean");
            mvpAward.MVPScreenImageFileNameOriginal = "storm_ui_mvp_icon.dds";
            yield return mvpAward;
        }

        private MatchAward ParseAward(string instanceId, string gameLink)
        {
            // get the name
            if (GameData.TryGetGameString($"{MapGameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}{gameLink}", out string awardNameText))
                instanceId = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);
            else if (GameData.TryGetGameString($"{MapGameStringPrefixes.MatchAwardInstanceNamePrefix}{gameLink}", out awardNameText))
                instanceId = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);

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
                ScoreScreenImageFileNameOriginal = Path.GetFileName(PathExtensions.GetFilePath(scoreScreenIconFilePath)),
                MVPScreenImageFileNameOriginal = $"storm_ui_mvp_icons_rewards_{awardSpecialName}.dds",
                Tag = scoreValueCustomElement.Element("UniqueTag").Attribute("value")?.Value,
            };

            string shortName = gameLink;
            if (shortName.StartsWith("EndOfMatchAward"))
                shortName = shortName.Remove(0, "EndOfMatchAward".Length);
            if (shortName.EndsWith("Boolean"))
                shortName = shortName.Substring(0, shortName.IndexOf("Boolean"));
            if (shortName.StartsWith("0"))
                shortName = shortName.ReplaceFirst("0", "Zero");
            if (shortName == "MostAltarDamageDone")
                shortName = "MostAltarDamage";

            matchAward.ShortName = shortName;

            // set new image file names for the extraction
            // change it back to the correct spelling
            if (awardSpecialName == "hottrick")
                awardSpecialName = "hattrick";

            matchAward.ScoreScreenImageFileName = matchAward.ScoreScreenImageFileNameOriginal.ToLower();
            matchAward.MVPScreenImageFileName = $"storm_ui_mvp_{awardSpecialName}_%color%.dds".ToLower();

            if (GameData.TryGetGameString($"{MapGameStringPrefixes.ScoreValueTooltipPrefix}{gameLink}", out string description))
                matchAward.Description = new TooltipDescription(description);

            return matchAward;
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
