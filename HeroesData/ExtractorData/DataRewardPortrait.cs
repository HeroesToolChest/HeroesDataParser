using Heroes.Models;
using HeroesData.Parser;
using System;
using System.Collections.Generic;

namespace HeroesData.ExtractorData
{
    public class DataRewardPortrait : DataExtractorBase<RewardPortrait?, RewardPortraitParser>, IData
    {
        public DataRewardPortrait(RewardPortraitParser parser)
            : base(parser)
        {
        }

        public override string Name => "rewardportraits";

        public override IEnumerable<RewardPortrait?> Parse(Localization localization)
        {
            return base.Parse(localization);
        }

        protected override void Validation(RewardPortrait? rewardPortrait)
        {
            if (rewardPortrait is null)
            {
                throw new ArgumentNullException(nameof(rewardPortrait));
            }

            if (string.IsNullOrEmpty(rewardPortrait.Name))
                AddWarning($"{nameof(rewardPortrait.Name)} is empty");

            if (string.IsNullOrEmpty(rewardPortrait.Id))
                AddWarning($"{nameof(rewardPortrait.Id)} is empty");

            if (string.IsNullOrEmpty(rewardPortrait.HyperlinkId))
                AddWarning($"{nameof(rewardPortrait.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(rewardPortrait.CollectionCategory))
                AddWarning($"{nameof(rewardPortrait.CollectionCategory)} is empty");

            if (string.IsNullOrEmpty(rewardPortrait.TextureSheet.Image))
                AddWarning($"{nameof(rewardPortrait.TextureSheet.Image)} is empty");

            if (rewardPortrait.TextureSheet.Columns < 1)
                AddWarning($"{nameof(rewardPortrait.TextureSheet.Columns)} is less than 1");

            if (rewardPortrait.TextureSheet.Rows < 1)
                AddWarning($"{nameof(rewardPortrait.TextureSheet.Rows)} is less than 1");

            if (rewardPortrait.Rarity == Rarity.None || rewardPortrait.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(rewardPortrait.Rarity)} is {rewardPortrait.Rarity}");
        }
    }
}
