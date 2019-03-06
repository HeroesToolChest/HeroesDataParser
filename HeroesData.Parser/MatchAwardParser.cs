using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class MatchAwardParser : ParserBase, IParser<MatchAward, MatchAwardParser>
    {
        private readonly string MVPGameLinkId = "EndOfMatchAwardMVPBoolean";

        public MatchAwardParser(GameData gameData, DefaultData defaultData)
            : base(gameData, defaultData)
        {
        }

        /// <summary>
        /// Returns a collection of all the parsable ids. Allows multiple ids.
        /// </summary>
        /// <returns></returns>
        public HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

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

                    items.Add(new string[] { instanceId, gameLink });
                }

                // manually add mvp award
                items.Add(new string[] { "[Override]Generic Instance", MVPGameLinkId });

                return items;
            }
        }

        /// <summary>
        /// Returns the parsed game data from the given ids. Multiple ids may be used to identify one item.
        /// </summary>
        /// <param name="id">The ids of the item to parse.</param>
        /// <returns></returns>
        public MatchAward Parse(params string[] ids)
        {
            if (ids == null || ids.Count() != 2)
                return null;

            string instanceId = ids[0];
            string gameLink = ids[1];

            // get the name
            if (GameData.TryGetGameString($"{MapGameStringPrefixes.MatchAwardMapSpecificInstanceNamePrefix}{gameLink}", out string awardNameText))
                instanceId = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);
            else if (GameData.TryGetGameString($"{MapGameStringPrefixes.MatchAwardInstanceNamePrefix}{gameLink}", out awardNameText))
                instanceId = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);

            XElement scoreValueCustomElement = GameData.MergeXmlElements(GameData.CScoreValueCustomElements.Where(x => x.Attribute("id")?.Value == gameLink));
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

            string id = gameLink;
            if (id.StartsWith("EndOfMatchAward"))
                id = id.Remove(0, "EndOfMatchAward".Length);
            if (id.EndsWith("Boolean"))
                id = id.Substring(0, id.IndexOf("Boolean"));
            if (id.StartsWith("0"))
                id = id.ReplaceFirst("0", "Zero");
            if (id == "MostAltarDamageDone")
                id = "MostAltarDamage";

            matchAward.Id = id;
            matchAward.HyperlinkId = gameLink;

            // set new image file names for the extraction
            // change it back to the correct spelling
            if (awardSpecialName == "hottrick")
                awardSpecialName = "hattrick";

            matchAward.ScoreScreenImageFileName = matchAward.ScoreScreenImageFileNameOriginal.ToLower();
            matchAward.MVPScreenImageFileName = $"storm_ui_mvp_{awardSpecialName}_%color%.dds".ToLower();

            if (GameData.TryGetGameString($"{MapGameStringPrefixes.ScoreValueTooltipPrefix}{gameLink}", out string description))
                matchAward.Description = new TooltipDescription(description);

            // mvp award only
            if (instanceId == "MVP" && gameLink == MVPGameLinkId)
                matchAward.MVPScreenImageFileNameOriginal = "storm_ui_mvp_icon.dds";

            return matchAward;
        }

        public MatchAwardParser GetInstance()
        {
            return new MatchAwardParser(GameData, DefaultData);
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
