using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataTypeDescription : DataExtractorBase<TypeDescription?, TypeDescriptionParser>, IData
    {
        public DataTypeDescription(TypeDescriptionParser parser)
            : base(parser)
        {
        }

        public override string Name => "typedescriptions";

        protected override void Validation(TypeDescription? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(data.TextureSheet.Image))
                AddWarning($"{nameof(data.TextureSheet.Image)} is empty");
        }
    }
}
