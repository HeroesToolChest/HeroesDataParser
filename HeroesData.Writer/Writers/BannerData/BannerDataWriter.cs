using Heroes.Models;

namespace HeroesData.FileWriter.Writers.BannerData
{
    internal abstract class BannerDataWriter<T, TU> : WriterBase<Banner, T>
        where T : class
        where TU : class
    {
        protected BannerDataWriter(FileOutputType fileOutputType)
            : base(nameof(BannerData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Banner banner)
        {
            GameStringWriter.AddBannerName(banner.Id, banner.Name);
            GameStringWriter.AddBannerSortName(banner.Id, banner.SortName);

            if (banner.Description != null)
                GameStringWriter.AddBannerDescription(banner.Id, GetTooltip(banner.Description, FileOutputOptions.DescriptionType));
        }
    }
}
