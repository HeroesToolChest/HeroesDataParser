using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class MatchAwardParser : ParserBase<MatchAward, MatchAwardDataOverride>, IParser<MatchAward, MatchAwardParser>
    {
        private readonly string GeneralMapName = "general";
        private readonly string AwardNameId = "UserData/EndOfMatchMapSpecificAward/[Override]Generic Instance_Award Name";
        private readonly string AwardNamePrefix = "ScoreValue/Name/";
        private readonly string AwardDescriptionPrefix = "ScoreValue/Tooltip/";

        private readonly MatchAwardOverrideLoader MatchAwardOverrideLoader;
        private readonly string MVPGameLinkId = "EndOfMatchAwardMVPBoolean";

        private MatchAwardDataOverride MatchAwardDataOverride;

        public MatchAwardParser(Configuration configuration, GameData gameData, DefaultData defaultData, MatchAwardOverrideLoader matchAwardOverrideLoader)
            : base(configuration, gameData, defaultData)
        {
            MatchAwardOverrideLoader = matchAwardOverrideLoader;
        }

        /// <summary>
        /// Returns a collection of all the parsable ids. Allows multiple ids.
        /// </summary>
        /// <returns></returns>
        public override HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                // general map gamelinks
                IEnumerable<XElement> matchAwards = GameData.Elements("CUser").FirstOrDefault(x => x.Attribute("id")?.Value == "EndOfMatchGeneralAward").Elements("Instances");
                IEnumerable<XElement> matchAwardsSpecific = GameData.Elements("CUser").FirstOrDefault(x => x.Attribute("id")?.Value == "EndOfMatchMapSpecificAward").Elements("Instances");

                matchAwards = matchAwards.Concat(matchAwardsSpecific);

                foreach (XElement awardInstance in matchAwards)
                {
                    string instanceId = awardInstance.Attribute("Id")?.Value;

                    if (instanceId == "[Default]" || !awardInstance.HasElements)
                        continue;

                    items.Add(new string[] { GeneralMapName, awardInstance.Element("GameLink").Attribute("GameLink").Value });
                }

                // map specific gamelinks
                List<XElement> matchAwardsMapSpecific = new List<XElement>();
                foreach (string mapName in GameData.UniqueIds)
                {
                    if (Configuration.RemoveDataXmlElementIds("MapStormmod").Contains(mapName))
                        continue;

                    matchAwards = GameData.GetUniqueGameData(mapName).Elements("CUser").Where(x => x.Attribute("id")?.Value == "EndOfMatchMapSpecificAward");

                    foreach (XElement awardInstance in matchAwards.Elements("Instances"))
                    {
                        string instanceId = awardInstance.Attribute("Id")?.Value;

                        if (instanceId == "[Default]" || !awardInstance.HasElements)
                            continue;

                        items.Add(new string[] { mapName, awardInstance.Element("GameLink")?.Attribute("GameLink")?.Value });
                    }
                }

                // manually add mvp award
                items.Add(new string[] { GeneralMapName, MVPGameLinkId });

                return items;
            }
        }

        protected override string ElementType => throw new NotImplementedException();

        /// <summary>
        /// Returns the parsed game data from the given ids. Multiple ids may be used to identify one item.
        /// </summary>
        /// <param name="id">The ids of the item to parse.</param>
        /// <returns></returns>
        public MatchAward Parse(params string[] ids)
        {
            if (ids == null || ids.Count() != 2)
                return null;

            string mapName = ids[0];
            string gameLink = ids[1];

            // get the name
            string awardName = string.Empty;

            if (mapName != GeneralMapName)
            {
                GameData mapGameData = GameData.GetUniqueGameData(mapName);
                if (mapGameData.TryGetGameString(AwardNameId, out string mapAwardNameText))
                    awardName = GetNameFromGenderRule(new TooltipDescription(mapAwardNameText).PlainText);
            }

            if (string.IsNullOrEmpty(awardName) && GameData.TryGetGameString($"{AwardNamePrefix}{gameLink}", out string awardNameText))
                awardName = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);

            XElement scoreValueCustomElement = GameData.MergeXmlElements(GameData.Elements("CScoreValueCustom").Where(x => x.Attribute("id")?.Value == gameLink));
            string scoreScreenIconFilePath = scoreValueCustomElement.Element("Icon").Attribute("value")?.Value;

            // get the name being used in the dds file
            string awardSpecialName = Path.GetFileName(PathHelpers.GetFilePath(scoreScreenIconFilePath)).Split('_')[4];

            MatchAward matchAward = new MatchAward()
            {
                Name = awardName,
                ScoreScreenImageFileNameOriginal = Path.GetFileName(PathHelpers.GetFilePath(scoreScreenIconFilePath)),
                MVPScreenImageFileNameOriginal = $"storm_ui_mvp_icons_rewards_{awardSpecialName}.dds",
                Tag = scoreValueCustomElement.Element("UniqueTag").Attribute("value")?.Value,
            };

            matchAward.Id = ModifyId(gameLink).ToString();
            matchAward.HyperlinkId = gameLink;

            // set new image file names for the extraction
            matchAward.ScoreScreenImageFileName = matchAward.ScoreScreenImageFileNameOriginal.ToLower();
            matchAward.MVPScreenImageFileName = $"storm_ui_mvp_{awardSpecialName}_%color%.dds".ToLower();

            // set description
            if (GameData.TryGetGameString($"{AwardDescriptionPrefix}{gameLink}", out string description))
                matchAward.Description = new TooltipDescription(description);

            // overrides
            MatchAwardDataOverride = MatchAwardOverrideLoader.GetOverride(matchAward.Id);
            ApplyOverrides(matchAward, MatchAwardDataOverride);

            return matchAward;
        }

        public MatchAwardParser GetInstance()
        {
            return new MatchAwardParser(Configuration, GameData, DefaultData, MatchAwardOverrideLoader);
        }

        protected override void ApplyAdditionalOverrides(MatchAward matchAward, MatchAwardDataOverride dataOverride)
        {
            if (dataOverride.MVPScreenImageFileNameOriginalOverride.Enabled)
                matchAward.MVPScreenImageFileNameOriginal = dataOverride.MVPScreenImageFileNameOriginalOverride.Value;

            if (dataOverride.MVPScreenImageFileNameOverride.Enabled)
                matchAward.MVPScreenImageFileName = dataOverride.MVPScreenImageFileNameOverride.Value;

            if (dataOverride.ScoreScreenImageFileNameOriginalOverride.Enabled)
                matchAward.ScoreScreenImageFileNameOriginal = dataOverride.ScoreScreenImageFileNameOriginalOverride.Value;

            if (dataOverride.ScoreScreenImageFileNameOverride.Enabled)
                matchAward.ScoreScreenImageFileName = dataOverride.ScoreScreenImageFileNameOverride.Value;

            if (dataOverride.DescriptionOverride.Enabled)
                matchAward.Description = new TooltipDescription(dataOverride.DescriptionOverride.Value);
        }

        protected override bool ValidItem(XElement element)
        {
            throw new NotImplementedException();
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

        private ReadOnlySpan<char> ModifyId(ReadOnlySpan<char> value)
        {
            if (value.StartsWith("EndOfMatchAward"))
                value = value.Slice("EndOfMatchAward".Length);
            if (value.EndsWith("Boolean"))
                value = value.Slice(0, value.IndexOf("Boolean"));
            if (value[0] == '0')
                value = ("Zero" + value.Slice(1).ToString()).AsSpan();

            return value;
        }
    }
}
