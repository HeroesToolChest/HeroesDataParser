using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataEmoticonPack : DataExtractorBase<EmoticonPack, EmoticonPackParser>, IData
    {
        public DataEmoticonPack(EmoticonPackParser parser)
            : base(parser)
        {
        }

        public override string Name => "emoticonpacks";

        protected override void Validation(EmoticonPack emoticonPack)
        {
            if (string.IsNullOrEmpty(emoticonPack.Name))
                AddWarning($"{nameof(emoticonPack.Name)} is null or empty");

            if (string.IsNullOrEmpty(emoticonPack.Id))
                AddWarning($"{nameof(emoticonPack.Id)} is null or empty");

            if (string.IsNullOrEmpty(emoticonPack.HyperlinkId))
                AddWarning($"{nameof(emoticonPack.HyperlinkId)} is null or empty");

            if (string.IsNullOrEmpty(emoticonPack.CollectionCategory))
                AddWarning($"{nameof(emoticonPack.CollectionCategory)} is null or empty");

            if (string.IsNullOrEmpty(emoticonPack.EventName))
                AddWarning($"{nameof(emoticonPack.EventName)} is null or empty");

            if (!emoticonPack.ReleaseDate.HasValue)
                AddWarning($"{nameof(emoticonPack.ReleaseDate)} is null");

            if (string.IsNullOrEmpty(emoticonPack.Description?.RawDescription))
                AddWarning($"{nameof(emoticonPack.Description)} is null or empty");

            if (emoticonPack.EmoticonIds == null || emoticonPack.EmoticonIds.Count < 1)
                AddWarning($"{nameof(emoticonPack.EmoticonIds)} is null or does not contain any emoticons");

            if (emoticonPack.EmoticonIds.Count < 0)
                AddWarning($"{nameof(emoticonPack.EmoticonIds)} does not contain any aliases.");
        }
    }
}
