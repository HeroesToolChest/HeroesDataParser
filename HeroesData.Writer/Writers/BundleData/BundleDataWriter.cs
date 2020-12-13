using Heroes.Models;

namespace HeroesData.FileWriter.Writers.BundleData
{
    internal abstract class BundleDataWriter<T, TU> : WriterBase<Bundle, T>
        where T : class
        where TU : class
    {
        protected BundleDataWriter(FileOutputType fileOutputType)
            : base(nameof(BundleData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(Bundle bundle)
        {
            GameStringWriter.AddBundleName(bundle.Id, bundle.Name);
            GameStringWriter.AddBundleSortName(bundle.Id, bundle.SortName);
        }

        protected abstract T GetHeroSkinsObject(Bundle bundle);

        protected virtual T? HeroSkins(Bundle bundle)
        {
            if (bundle.HeroSkinsCount > 0)
            {
                return GetHeroSkinsObject(bundle);
            }

            return null;
        }
    }
}
