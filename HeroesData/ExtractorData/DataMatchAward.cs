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

        protected override void Validation(MatchAward? matchAward)
        {
            if (matchAward is null)
            {
                throw new ArgumentNullException(nameof(matchAward));
            }

            if (string.IsNullOrEmpty(matchAward.Name))
                AddWarning($"{nameof(matchAward.Name)} is empty");

            if (matchAward.Name.Contains("_"))
                AddWarning($"{nameof(matchAward.Name)} contains an underscore, may have a duplicate name");

            if (string.IsNullOrEmpty(matchAward.HyperlinkId))
                AddWarning($"{nameof(matchAward.HyperlinkId)} is empty");

            if (matchAward.HyperlinkId.Contains(","))
                AddWarning($"{nameof(matchAward.HyperlinkId)} contains a comma, may have a duplicate short name");

            if (string.IsNullOrEmpty(matchAward.Tag))
                AddWarning($"{nameof(matchAward.Tag)} is empty");

            if (string.IsNullOrEmpty(matchAward.MVPScreenImageFileName))
                AddWarning($"{nameof(matchAward.MVPScreenImageFileName)} is empty");

            if (string.IsNullOrEmpty(matchAward.ScoreScreenImageFileName))
                AddWarning($"{nameof(matchAward.ScoreScreenImageFileName)} is empty");

            if (string.IsNullOrEmpty(matchAward.ScoreScreenImageFileNameOriginal))
                AddWarning($"{nameof(matchAward.ScoreScreenImageFileNameOriginal)} is empty");
        }
    }
}
