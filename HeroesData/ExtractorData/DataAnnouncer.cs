using Heroes.Models;
using HeroesData.Parser;
using System;

namespace HeroesData.ExtractorData
{
    public class DataAnnouncer : DataExtractorBase<Announcer?, AnnouncerParser>, IData
    {
        public DataAnnouncer(AnnouncerParser parser)
            : base(parser)
        {
        }

        public override string Name => "announcers";

        protected override void Validation(Announcer? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Name))
                AddWarning($"{nameof(data.Name)} is empty");

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");

            if (!string.IsNullOrEmpty(data.HyperlinkId) && data.HyperlinkId.Contains("##heroid##", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(data.HyperlinkId)} ##heroid## not found");

            if (string.IsNullOrEmpty(data.AttributeId))
                AddWarning($"{nameof(data.AttributeId)} is empty");

            if (string.IsNullOrEmpty(data.CollectionCategory))
                AddWarning($"{nameof(data.CollectionCategory)} is empty");

            if (!data.ReleaseDate.HasValue)
                AddWarning($"{nameof(data.ReleaseDate)} is null");

            if (data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is unknown");

            if (string.IsNullOrEmpty(data.Gender))
                AddWarning($"{nameof(data.Gender)} is empty");

            if (string.IsNullOrEmpty(data.HeroId))
                AddWarning($"{nameof(data.HeroId)} is empty");

            if (!string.IsNullOrEmpty(data.HeroId) && data.HeroId.Contains("##heroid##", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(data.HeroId)} ##heroid## not found");

            if (string.IsNullOrEmpty(data.ImageFileName))
                AddWarning($"{nameof(data.ImageFileName)} is empty");

            if (!string.IsNullOrEmpty(data.ImageFileName) && data.ImageFileName.Contains("##heroid##", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(data.ImageFileName)} ##heroid## not found");

            if (data.Rarity == Rarity.None || data.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(data.Rarity)} is {data.Rarity}");
        }
    }
}
