using Heroes.Models;

namespace HeroesData.FileWriter.Writers.RewardPortraitData
{
    internal abstract class RewardPortraitDataWriter<T, TU> : WriterBase<RewardPortrait, T>
        where T : class
        where TU : class
    {
        protected RewardPortraitDataWriter(FileOutputType fileOutputType)
            : base(nameof(RewardPortraitData), fileOutputType)
        {
        }

        protected abstract T GetImageObject(RewardPortrait rewardPortrait);

        protected void AddLocalizedGameString(RewardPortrait rewardPortrait)
        {
            GameStringWriter.AddRewardPortraitName(rewardPortrait.Id, rewardPortrait.Name);

            if (rewardPortrait.Description != null)
                GameStringWriter.AddRewardPortraitDescription(rewardPortrait.Id, GetTooltip(rewardPortrait.Description, FileOutputOptions.DescriptionType));

            if (rewardPortrait.DescriptionUnearned != null)
                GameStringWriter.AddRewardPortraitDescriptionUnearned(rewardPortrait.Id, GetTooltip(rewardPortrait.DescriptionUnearned, FileOutputOptions.DescriptionType));
        }
    }
}
