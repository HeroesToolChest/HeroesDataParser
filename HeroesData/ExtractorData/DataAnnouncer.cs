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

        protected override void Validation(Announcer? announcer)
        {
            if (announcer is null)
            {
                throw new ArgumentNullException(nameof(announcer));
            }

            if (string.IsNullOrEmpty(announcer.Name))
                AddWarning($"{nameof(announcer.Name)} is empty");

            if (string.IsNullOrEmpty(announcer.Id))
                AddWarning($"{nameof(announcer.Id)} is empty");

            if (string.IsNullOrEmpty(announcer.HyperlinkId))
                AddWarning($"{nameof(announcer.HyperlinkId)} is empty");

            if (!string.IsNullOrEmpty(announcer.HyperlinkId) && announcer.HyperlinkId.Contains("##heroid##", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(announcer.HyperlinkId)} ##heroid## not found");

            if (string.IsNullOrEmpty(announcer.AttributeId))
                AddWarning($"{nameof(announcer.AttributeId)} is empty");

            if (string.IsNullOrEmpty(announcer.CollectionCategory))
                AddWarning($"{nameof(announcer.CollectionCategory)} is empty");

            if (!announcer.ReleaseDate.HasValue)
                AddWarning($"{nameof(announcer.ReleaseDate)} is null");

            if (announcer.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(announcer.Rarity)} is unknown");

            if (string.IsNullOrEmpty(announcer.Gender))
                AddWarning($"{nameof(announcer.Gender)} is empty");

            if (string.IsNullOrEmpty(announcer.HeroId))
                AddWarning($"{nameof(announcer.HeroId)} is empty");

            if (!string.IsNullOrEmpty(announcer.HeroId) && announcer.HeroId.Contains("##heroid##", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(announcer.HeroId)} ##heroid## not found");

            if (string.IsNullOrEmpty(announcer.ImageFileName))
                AddWarning($"{nameof(announcer.ImageFileName)} is empty");

            if (!string.IsNullOrEmpty(announcer.ImageFileName) && announcer.ImageFileName.Contains("##heroid##", StringComparison.OrdinalIgnoreCase))
                AddWarning($"{nameof(announcer.ImageFileName)} ##heroid## not found");

            if (announcer.Rarity == Rarity.None || announcer.Rarity == Rarity.Unknown)
                AddWarning($"{nameof(announcer.Rarity)} is {announcer.Rarity}");
        }
    }
}
