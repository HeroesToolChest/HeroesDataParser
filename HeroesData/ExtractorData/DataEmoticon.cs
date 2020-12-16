using Heroes.Models;
using HeroesData.Parser;
using System.Linq;

namespace HeroesData.ExtractorData
{
    public class DataEmoticon : DataExtractorBase<Emoticon?, EmoticonParser>, IData
    {
        public DataEmoticon(EmoticonParser parser)
            : base(parser)
        {
        }

        public override string Name => "emoticons";

        protected override void Validation(Emoticon? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (!data.LocalizedAliases.Any() && !data.UniversalAliases.Any())
                AddWarning("Does not contain any aliases.");

            if (string.IsNullOrEmpty(data.TextureSheet.Image))
                AddWarning($"{nameof(data.TextureSheet.Image)} is empty");
        }
    }
}
