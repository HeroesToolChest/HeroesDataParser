using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataBoost : DataExtractorBase<Boost?, BoostParser>, IData
    {
        public DataBoost(BoostParser parser)
            : base(parser)
        {
        }

        public override string Name => "boosts";

        protected override void Validation(Boost? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");
        }
    }
}
