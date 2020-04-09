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
    public class MatchAwardParser : ParserBase<MatchAward, MatchAwardDataOverride>, IParser<MatchAward?, MatchAwardParser>
    {
        private readonly string _awardNameId = "UserData/EndOfMatchMapSpecificAward/[Override]Generic Instance_Award Name";
        private readonly string _awardNamePrefix = "ScoreValue/Name/";
        private readonly string _awardDescriptionPrefix = "ScoreValue/Tooltip/";

        private readonly MatchAwardOverrideLoader _matchAwardOverrideLoader;
        private readonly string _mvpGameLinkId = "EndOfMatchAwardMVPBoolean";

        private MatchAwardDataOverride? _matchAwardDataOverride;

        public MatchAwardParser(IXmlDataService xmlDataService, MatchAwardOverrideLoader matchAwardOverrideLoader)
            : base(xmlDataService)
        {
            _matchAwardOverrideLoader = matchAwardOverrideLoader;
        }

        /// <summary>
        /// Gets a collection of all the parsable ids. Allows multiple ids.
        /// </summary>
        /// <returns></returns>
        public override HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());

                // general map gamelinks
                IEnumerable<XElement>? matchAwards = GameData.Elements("CUser").FirstOrDefault(x => x.Attribute("id")?.Value == "EndOfMatchGeneralAward")?.Elements("Instances");
                IEnumerable<XElement>? matchAwardsSpecific = GameData.Elements("CUser").FirstOrDefault(x => x.Attribute("id")?.Value == "EndOfMatchMapSpecificAward")?.Elements("Instances");

                if (matchAwards == null && matchAwardsSpecific != null)
                    matchAwards = matchAwardsSpecific;
                else if (matchAwards != null && matchAwardsSpecific != null)
                    matchAwards = matchAwards.Concat(matchAwardsSpecific);

                AddItems(GeneralMapName, matchAwards!, items);

                // map specific gamelinks
                foreach (string mapName in GameData.MapIds)
                {
                    matchAwards = GameData.GetMapGameData(mapName).Elements("CUser").Where(x => x.Attribute("id")?.Value == "EndOfMatchMapSpecificAward");

                    AddItems(mapName, matchAwards.Elements("Instances"), items);
                }

                // manually add mvp award
                items.Add(new string[] { _mvpGameLinkId });

                return items;
            }
        }

        protected override string ElementType => throw new NotSupportedException();

        /// <summary>
        /// Returns the parsed game data from the given ids. Multiple ids may be used to identify one item.
        /// </summary>
        /// <param name="ids">The ids of the item to parse.</param>
        /// <returns></returns>
        public MatchAward? Parse(params string[] ids)
        {
            if (ids == null)
                return null;

            string gameLink = ids[0];
            string mapNameId = string.Empty;

            if (ids.Length == 2)
                mapNameId = ids[1];

            // get the name
            string awardName = string.Empty;

            MatchAward? matchAward = null;

            if (GameData.TryGetGameString($"{_awardNameId}", out string? awardNameText))
                awardName = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);
            else if (GameData.TryGetGameString($"{_awardNamePrefix}{gameLink}", out awardNameText))
                awardName = GetNameFromGenderRule(new TooltipDescription(awardNameText).PlainText);

            XElement? scoreValueCustomElement = GameData.MergeXmlElements(GameData.Elements("CScoreValueCustom").Where(x => x.Attribute("id")?.Value == gameLink));
            if (scoreValueCustomElement != null)
            {
                string? scoreScreenIconFilePath = scoreValueCustomElement.Element("Icon").Attribute("value")?.Value;

                // get the name being used in the dds file
                if (!string.IsNullOrEmpty(scoreScreenIconFilePath))
                {
                    string awardSpecialName = Path.GetFileName(PathHelper.GetFilePath(scoreScreenIconFilePath)).Split('_')[4];

                    matchAward = new MatchAward()
                    {
                        Name = awardName,
                        ScoreScreenImageFileNameOriginal = Path.GetFileName(PathHelper.GetFilePath(scoreScreenIconFilePath)),
                        MVPScreenImageFileNameOriginal = $"storm_ui_mvp_icons_rewards_{awardSpecialName}.dds",
                        Tag = scoreValueCustomElement.Element("UniqueTag").Attribute("value")?.Value ?? string.Empty,
                    };

                    matchAward.Id = ModifyId(gameLink).ToString();
                    matchAward.HyperlinkId = gameLink;

                    // set new image file names for the extraction
                    matchAward.ScoreScreenImageFileName = matchAward.ScoreScreenImageFileNameOriginal.ToLowerInvariant();
                    matchAward.MVPScreenImageFileName = $"storm_ui_mvp_{awardSpecialName}_%color%.dds".ToLowerInvariant();

                    // set description
                    if (GameData.TryGetGameString($"{_awardDescriptionPrefix}{gameLink}", out string? description))
                        matchAward.Description = new TooltipDescription(description);

                    // overrides
                    _matchAwardDataOverride = _matchAwardOverrideLoader.GetOverride(matchAward.Id);

                    if (_matchAwardDataOverride != null)
                        ApplyOverrides(matchAward, _matchAwardDataOverride);
                }
            }

            return matchAward;
        }

        public MatchAwardParser GetInstance()
        {
            return new MatchAwardParser(XmlDataService, _matchAwardOverrideLoader);
        }

        protected override void ApplyAdditionalOverrides(MatchAward matchAward, MatchAwardDataOverride dataOverride)
        {
            if (matchAward is null)
                throw new ArgumentNullException(nameof(matchAward));
            if (dataOverride is null)
                throw new ArgumentNullException(nameof(dataOverride));

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
            if (value.StartsWith("EndOfMatchAward", StringComparison.OrdinalIgnoreCase))
                value = value.Slice("EndOfMatchAward".Length);
            if (value.EndsWith("Boolean", StringComparison.OrdinalIgnoreCase))
                value = value.Slice(0, value.IndexOf("Boolean", StringComparison.OrdinalIgnoreCase));
            if (value[0] == '0')
                value = ("Zero" + value.Slice(1).ToString()).AsSpan();

            return value;
        }

        private void AddItems(string mapName, IEnumerable<XElement> elements, HashSet<string[]> items)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            foreach (XElement element in elements)
            {
                string? instanceId = element.Attribute("Id")?.Value;

                if (instanceId == "[Default]" || !element.HasElements)
                    continue;

                string? id = element.Element("GameLink")?.Attribute("GameLink")?.Value;

                if (!string.IsNullOrEmpty(id))
                {
                    if (string.IsNullOrEmpty(mapName))
                        items.Add(new string[] { id });
                    else
                        items.Add(new string[] { id, mapName });
                }
            }
        }
    }
}
