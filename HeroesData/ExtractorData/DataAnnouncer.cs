using Heroes.Models;
using HeroesData.Parser;

namespace HeroesData.ExtractorData
{
    public class DataAnnouncer : DataExtractorBase<Announcer, AnnouncerParser>, IData
    {
        public DataAnnouncer(AnnouncerParser parser)
            : base(parser)
        {
        }

        public override string Name => "announcers";

        protected override void Validation(Announcer announcer)
        {
            if (string.IsNullOrEmpty(announcer.Name))
                AddWarning($"{nameof(announcer.Name)} is empty");

            if (string.IsNullOrEmpty(announcer.Id))
                AddWarning($"{nameof(announcer.Id)} is empty");

            if (string.IsNullOrEmpty(announcer.HyperlinkId))
                AddWarning($"{nameof(announcer.HyperlinkId)} is empty");

            if (!string.IsNullOrEmpty(announcer.HyperlinkId) && announcer.HyperlinkId.Contains("##heroid##"))
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

            if (string.IsNullOrEmpty(announcer.Hero))
                AddWarning($"{nameof(announcer.Hero)} is empty");

            if (!string.IsNullOrEmpty(announcer.Hero) && announcer.Hero.Contains("##heroid##"))
                AddWarning($"{nameof(announcer.Hero)} ##heroid## not found");

            if (string.IsNullOrEmpty(announcer.ImageFileName))
                AddWarning($"{nameof(announcer.ImageFileName)} is empty");

            if (!string.IsNullOrEmpty(announcer.ImageFileName) && announcer.ImageFileName.Contains("##heroid##"))
                AddWarning($"{nameof(announcer.ImageFileName)} ##heroid## not found");
        }
    }
}
