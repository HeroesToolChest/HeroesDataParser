using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataEmoticon : DataExtractorBase<Emoticon, EmoticonParser>, IData
    {
        public DataEmoticon(EmoticonParser parser)
            : base(parser)
        {
        }

        public override string Name => "emoticons";

        protected override void Validation(Emoticon emoticon)
        {
            if (string.IsNullOrEmpty(emoticon.Name))
                AddWarning($"{nameof(emoticon.Name)} is null or empty");

            if (string.IsNullOrEmpty(emoticon.Id))
                AddWarning($"{nameof(emoticon.Id)} is null or empty");

            if (emoticon.UniversalAliases.Count < 0)
                AddWarning($"{nameof(emoticon.UniversalAliases)} does not contain any aliases.");

            if (string.IsNullOrEmpty(emoticon.TextureSheet.Image))
                AddWarning($"{nameof(emoticon.TextureSheet.Image)} is null or empty");
        }
    }
}
