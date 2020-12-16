using Heroes.Models;
using HeroesData.Parser;
using System;

namespace HeroesData.ExtractorData
{
    public class DataMatchAward : DataExtractorBase<MatchAward?, MatchAwardParser>, IData
    {
        public DataMatchAward(MatchAwardParser parser)
            : base(parser)
        {
        }

        public override string Name => "awards";

        protected override void Validation(MatchAward? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (data.Name is not null && data.Name.Contains("_", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(data.Name)} contains an underscore, may have a duplicate name");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (data.HyperlinkId is not null && data.HyperlinkId.Contains(",", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(data.HyperlinkId)} contains a comma, may have a duplicate short name");

            if (string.IsNullOrEmpty(data.Tag))
                AddWarning($"{nameof(data.Tag)} is empty");

            if (string.IsNullOrEmpty(data.MVPScreenImageFileName))
                AddWarning($"{nameof(data.MVPScreenImageFileName)} is empty");

            if (string.IsNullOrEmpty(data.ScoreScreenImageFileName))
                AddWarning($"{nameof(data.ScoreScreenImageFileName)} is empty");

            if (string.IsNullOrEmpty(data.ScoreScreenImageFileNameOriginal))
                AddWarning($"{nameof(data.ScoreScreenImageFileNameOriginal)} is empty");
        }
    }
}
