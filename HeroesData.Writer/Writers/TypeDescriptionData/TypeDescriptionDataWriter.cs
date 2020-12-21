using Heroes.Models;

namespace HeroesData.FileWriter.Writers.TypeDescriptionData
{
    internal abstract class TypeDescriptionDataWriter<T, TU> : WriterBase<TypeDescription, T>
        where T : class
        where TU : class
    {
        protected TypeDescriptionDataWriter(FileOutputType fileOutputType)
            : base(nameof(TypeDescriptionData), fileOutputType)
        {
        }

        protected abstract T GetImageObject(TypeDescription typeDescription);

        protected void AddLocalizedGameString(TypeDescription typeDescription)
        {
            GameStringWriter.AddTypeDescriptionName(typeDescription.Id, typeDescription.Name);
        }
    }
}
