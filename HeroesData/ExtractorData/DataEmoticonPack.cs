using Heroes.Models;
using HeroesData.Parser;
using System.Linq;

namespace HeroesData.ExtractorData
{
    public class DataEmoticonPack : DataExtractorBase<EmoticonPack?, EmoticonPackParser>, IData
    {
        public DataEmoticonPack(EmoticonPackParser parser)
            : base(parser)
        {
        }

        public override string Name => "emoticonpacks";

        protected override void Validation(EmoticonPack? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (string.IsNullOrEmpty(data.CollectionCategory))
                AddWarning($"{nameof(data.CollectionCategory)} is empty");

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");

            if (data.EmoticonIds == null || !data.EmoticonIds.Any())
                AddWarning($"{nameof(data.EmoticonIds)} is null or does not contain any emoticons");

            if (!data.EmoticonIds!.Any())
                AddWarning($"{nameof(data.EmoticonIds)} does not contain any aliases.");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
