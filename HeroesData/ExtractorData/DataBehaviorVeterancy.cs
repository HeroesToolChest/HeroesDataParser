using Heroes.Models.Veterancy;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataBehaviorVeterancy : DataExtractorBase<BehaviorVeterancy, BehaviorVeterancyParser>, IData
    {
        public DataBehaviorVeterancy(BehaviorVeterancyParser parser)
            : base(parser)
        {
        }

        public override string Name => "veterancies";

        protected override void Validation(BehaviorVeterancy behaviorVeterancy)
        {
        }
    }
}
