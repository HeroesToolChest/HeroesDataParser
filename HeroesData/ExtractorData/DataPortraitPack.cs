using Heroes.Models;
using HeroesData.Parser;
using System.Collections.Generic;

namespace HeroesData.ExtractorData
{
    public class DataPortraitPack : DataExtractorBase<PortraitPack?, PortraitPackParser>, IData
    {
        public DataPortraitPack(PortraitPackParser parser)
            : base(parser)
        {
        }

        public override string Name => "portraitpacks";

        public override IEnumerable<PortraitPack?> Parse(Localization localization)
        {
            return base.Parse(localization);
        }

        protected override void Validation(PortraitPack? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
