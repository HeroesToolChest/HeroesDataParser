using Heroes.Models;

namespace HeroesData.FileWriter.Writers.HeroSkinData
{
    internal abstract class HeroSkinDataWriter<T, TU> : WriterBase<HeroSkin, T>
        where T : class
        where TU : class
    {
        protected HeroSkinDataWriter(FileOutputType fileOutputType)
            : base(nameof(HeroSkinData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(HeroSkin heroSkin)
        {
            GameStringWriter.AddHeroSkinName(heroSkin.ShortName, heroSkin.Name);
            GameStringWriter.AddHeroSkinSortName(heroSkin.ShortName, heroSkin.SortName);
            GameStringWriter.AddHeroSkinInfo(heroSkin.ShortName, GetTooltip(heroSkin.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddHeroSkinSearchText(heroSkin.ShortName, heroSkin.SearchText);
        }
    }
}
