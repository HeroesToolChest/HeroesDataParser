using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class RewardPortraitParser : ParserBase<RewardPortrait, RewardPortraitDataOverride>, IParser<RewardPortrait?, RewardPortraitParser>
    {
        public RewardPortraitParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CRewardPortrait";

        public RewardPortraitParser GetInstance()
        {
            return new RewardPortraitParser(XmlDataService);
        }

        public RewardPortrait? Parse(params string[] ids)
        {
            if (ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? rewardPortraitElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (rewardPortraitElement == null)
                return null;

            RewardPortrait rewardPortrait = new RewardPortrait()
            {
                Id = id,
            };

            SetDefaultValues(rewardPortrait);
            SetRewardPortraitData(rewardPortraitElement, rewardPortrait);

            if (string.IsNullOrEmpty(rewardPortrait.HyperlinkId))
                rewardPortrait.HyperlinkId = id;

            if (!string.IsNullOrEmpty(rewardPortrait.TextureSheet.Image))
                rewardPortrait.ImageFileName = $"storm_portrait_{rewardPortrait.Id.ToLowerInvariant()}.dds";

            return rewardPortrait;
        }

        private void SetRewardPortraitData(XElement portraitElement, RewardPortrait rewardPortrait)
        {
            // parent lookup
            string? parentValue = portraitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetRewardPortraitData(parentElement, rewardPortrait);
            }

            foreach (XElement element in portraitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "ICONCOLS")
                {
                    if (int.TryParse(element.Attribute("value")?.Value, out int result))
                        rewardPortrait.TextureSheet.Columns = result;
                }
                else if (elementName == "ICONROWS")
                {
                    if (int.TryParse(element.Attribute("value")?.Value, out int result))
                        rewardPortrait.TextureSheet.Rows = result;
                }
                else if (elementName == "ICONFILE")
                {
                    rewardPortrait.TextureSheet.Image = GameData.GetValueFromAttribute(element.Attribute("value")?.Value);
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    rewardPortrait.CollectionCategory = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity rarity))
                    {
                        rewardPortrait.Rarity = rarity;
                    }
                }
                else if (elementName == "ICONSLOT")
                {
                    if (int.TryParse(element.Attribute("value")?.Value, out int result))
                        rewardPortrait.IconSlot = result;
                }
                else if (elementName == "HYPERLINKID")
                {
                    rewardPortrait.HyperlinkId = element.Attribute("value")?.Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    string? descriptionValue = element.Attribute("value")?.Value;
                    if (descriptionValue is not null)
                    {
                        rewardPortrait.Description = new TooltipDescription(GameData.GetGameString(descriptionValue));
                    }
                }
                else if (elementName == "DESCRIPTIONUNEARNED")
                {
                    string? descriptionUnearnedValue = element.Attribute("value")?.Value;
                    if (descriptionUnearnedValue is not null)
                    {
                        rewardPortrait.DescriptionUnearned = new TooltipDescription(GameData.GetGameString(descriptionUnearnedValue));
                    }
                }
                else if (elementName == "HERO")
                {
                    rewardPortrait.HeroId = element.Attribute("value")?.Value;
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value;
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            rewardPortrait.Name = text;
                    }
                }
                else if (elementName == "PORTRAITPACK")
                {
                    rewardPortrait.PortraitPackId = element.Attribute("value")?.Value.Replace(DefaultData.IdPlaceHolder, rewardPortrait.Id, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private void SetDefaultValues(RewardPortrait rewardPortrait)
        {
            rewardPortrait.Name = GameData.GetGameString(DefaultData.RewardData?.RewardName.Replace(DefaultData.IdPlaceHolder, rewardPortrait.Id, StringComparison.OrdinalIgnoreCase));
            rewardPortrait.Description = new TooltipDescription(GameData.GetGameString(DefaultData.RewardData?.RewardDescription.Replace(DefaultData.IdPlaceHolder, rewardPortrait.Id, StringComparison.OrdinalIgnoreCase)));
            rewardPortrait.DescriptionUnearned = new TooltipDescription(GameData.GetGameString(DefaultData.RewardData?.RewardDescriptionUnearned.Replace(DefaultData.IdPlaceHolder, rewardPortrait.Id, StringComparison.OrdinalIgnoreCase)));
            rewardPortrait.HyperlinkId = GameData.GetGameString(DefaultData.RewardData?.RewardHyperlinkId.Replace(DefaultData.IdPlaceHolder, rewardPortrait.Id, StringComparison.OrdinalIgnoreCase));

            rewardPortrait.TextureSheet.Columns = DefaultData.RewardPortraitData?.PortraitIconColumns ?? 0;
            rewardPortrait.TextureSheet.Rows = DefaultData.RewardPortraitData?.PortraitIconRows ?? 0;
            rewardPortrait.CollectionCategory = DefaultData.RewardPortraitData?.PortraitCollectionCategory;
            rewardPortrait.TextureSheet.Image = DefaultData.RewardPortraitData?.PortraitIconFileName ?? string.Empty;
            rewardPortrait.Rarity = DefaultData.RewardPortraitData?.PortraitRarity ?? Rarity.Common;
        }
    }
}
