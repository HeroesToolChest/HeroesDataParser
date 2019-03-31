using Heroes.Models;

namespace HeroesData.FileWriter.Writers.SprayData
{
    internal abstract class SprayDataWriter<T, TU> : WriterBase<Spray, T>
        where T : class
        where TU : class
    {
        public SprayDataWriter(FileOutputType fileOutputType)
            : base(nameof(SprayData), fileOutputType)
        {
        }

        protected abstract T GetAnimationObject(Spray spray);

        protected void AddLocalizedGameString(Spray spray)
        {
            GameStringWriter.AddSprayName(spray.Id, spray.Name);
            GameStringWriter.AddSpraySortName(spray.Id, spray.SortName);
            GameStringWriter.AddSprayDescription(spray.Id, GetTooltip(spray.Description, FileOutputOptions.DescriptionType));
            GameStringWriter.AddSpraySearchText(spray.Id, spray.SearchText);
        }

        protected T AnimationObject(Spray spray)
        {
            if (spray.AnimationCount > 0)
            {
                return GetAnimationObject(spray);
            }

            return null;
        }
    }
}
