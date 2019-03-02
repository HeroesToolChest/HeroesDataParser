using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataMatchAward : DataExtractorBase<MatchAward, MatchAwardParser>, IData
    {
        public DataMatchAward(MatchAwardParser parser)
            : base(parser)
        {
        }

        public override string Name => "awards";

        protected override void Validation(MatchAward matchAward)
        {
            if (string.IsNullOrEmpty(matchAward.Name))
                AddWarning($"{nameof(matchAward.Name)} is null or empty");

            if (matchAward.Name.Contains("_"))
                AddWarning($"{nameof(matchAward.Name)} contains an underscore, may have a duplicate name");

            if (string.IsNullOrEmpty(matchAward.HyperlinkId))
                AddWarning($"{nameof(matchAward.HyperlinkId)} is null or empty");

            if (matchAward.HyperlinkId.Contains(","))
                AddWarning($"{nameof(matchAward.HyperlinkId)} contains a comma, may have a duplicate short name");

            if (string.IsNullOrEmpty(matchAward.Tag))
                AddWarning($"{nameof(matchAward.Tag)} is null or empty");

            if (string.IsNullOrEmpty(matchAward.MVPScreenImageFileName))
                AddWarning($"{nameof(matchAward.MVPScreenImageFileName)} is null or empty");

            if (string.IsNullOrEmpty(matchAward.ScoreScreenImageFileName))
                AddWarning($"{nameof(matchAward.ScoreScreenImageFileName)} is null or empty");

            if (string.IsNullOrEmpty(matchAward.ScoreScreenImageFileNameOriginal))
                AddWarning($"{nameof(matchAward.ScoreScreenImageFileNameOriginal)} is null or empty");
        }
    }
}
