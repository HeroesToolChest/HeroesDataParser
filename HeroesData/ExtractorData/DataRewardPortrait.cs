using Heroes.Models;
using HeroesData.Parser;
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

        protected override void Validation(RewardPortrait? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(data.CollectionCategory))
                AddWarning($"{nameof(data.CollectionCategory)} is empty");

            if (string.IsNullOrEmpty(data.TextureSheet.Image))
                AddWarning($"{nameof(data.TextureSheet.Image)} is empty");

            if (!data.TextureSheet.Columns.HasValue || (data.TextureSheet.Columns.HasValue && data.TextureSheet.Columns < 1))
                AddWarning($"{nameof(data.TextureSheet.Columns)} is less than 1");

            if (!data.TextureSheet.Rows.HasValue || (data.TextureSheet.Rows.HasValue && data.TextureSheet.Rows < 1))
                AddWarning($"{nameof(data.TextureSheet.Rows)} is less than 1");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
