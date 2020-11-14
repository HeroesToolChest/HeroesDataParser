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

        protected override void Validation(PortraitPack? portraitPack)
        {
            if (portraitPack is null)
                return;

            if (string.IsNullOrEmpty(portraitPack.Name))
                AddWarning($"{nameof(portraitPack.Name)} is empty");

            if (string.IsNullOrEmpty(portraitPack.Id))
                AddWarning($"{nameof(portraitPack.Id)} is empty");

            if (string.IsNullOrEmpty(portraitPack.HyperlinkId))
                AddWarning($"{nameof(portraitPack.HyperlinkId)} is empty");

            if (portraitPack.Rarity == Rarity.None || portraitPack.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(portraitPack.Rarity)} is {portraitPack.Rarity}");
        }
    }
}
