using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataPortrait : DataExtractorBase<Portrait, PortraitParser>, IData
    {
        public DataPortrait(PortraitParser parser)
            : base(parser)
        {
        }

        public override string Name => "portrait";

        protected override void Validation(Portrait portrait)
        {
            if (string.IsNullOrEmpty(portrait.Name))
                AddWarning($"{nameof(portrait.Name)} is empty");

            if (string.IsNullOrEmpty(portrait.Id))
                AddWarning($"{nameof(portrait.Id)} is empty");

            if (string.IsNullOrEmpty(portrait.HyperlinkId))
                AddWarning($"{nameof(portrait.HyperlinkId)} is empty");
        }
    }
}
