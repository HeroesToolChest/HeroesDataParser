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
            GameStringWriter.AddHeroSkinName(heroSkin.Id, heroSkin.Name);
            GameStringWriter.AddHeroSkinSortName(heroSkin.Id, heroSkin.SortName);

            if (heroSkin.InfoText != null)
                GameStringWriter.AddHeroSkinInfoText(heroSkin.Id, GetTooltip(heroSkin.InfoText, FileOutputOptions.DescriptionType));

            GameStringWriter.AddHeroSkinSearchText(heroSkin.Id, heroSkin.SearchText);
        }
    }
}
