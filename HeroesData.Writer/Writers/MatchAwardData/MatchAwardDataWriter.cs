using Heroes.Models;

namespace HeroesData.FileWriter.Writers.MatchAwardData
{
    internal abstract class MatchAwardDataWriter<T, TU> : WriterBase<MatchAward, T>
        where T : class
        where TU : class
    {
        protected MatchAwardDataWriter(FileOutputType fileOutputType)
            : base(nameof(MatchAwardData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(MatchAward matchAward)
        {
            GameStringWriter.AddMatchAwardName(matchAward.ShortName, matchAward.Name);
            GameStringWriter.AddMatchAwardDescription(matchAward.ShortName, GetTooltip(matchAward.Description, FileOutputOptions.DescriptionType));
        }
    }
}
