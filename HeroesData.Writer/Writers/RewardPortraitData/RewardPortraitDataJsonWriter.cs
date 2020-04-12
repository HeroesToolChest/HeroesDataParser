using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writers.RewardPortraitData
{
    internal class RewardPortraitDataJsonWriter : RewardPortraitDataWriter<JProperty, JObject>
    {
        public RewardPortraitDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(RewardPortrait rewardPortrait)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(rewardPortrait);

            JObject portraitObject = new JObject();

            if (!string.IsNullOrEmpty(rewardPortrait.Name) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("name", rewardPortrait.Name);

            portraitObject.Add("hyperlinkId", rewardPortrait.HyperlinkId);
            portraitObject.Add("rarity", rewardPortrait.Rarity.ToString());

            if (!string.IsNullOrEmpty(rewardPortrait.CollectionCategory))
                portraitObject.Add("category", rewardPortrait.CollectionCategory);

            if (!string.IsNullOrEmpty(rewardPortrait.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("description", GetTooltip(rewardPortrait.Description, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(rewardPortrait.DescriptionUnearned?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                portraitObject.Add("descriptionUnearned", GetTooltip(rewardPortrait.DescriptionUnearned, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(rewardPortrait.HeroId))
                portraitObject.Add(new JProperty("heroId", rewardPortrait.HeroId));

            if (!string.IsNullOrEmpty(rewardPortrait.PortraitPackId))
                portraitObject.Add(new JProperty("portraitPackId", rewardPortrait.PortraitPackId));

            portraitObject.Add(new JProperty("iconSlot", rewardPortrait.IconSlot));

            JProperty? image = GetImageObject(rewardPortrait);
            if (image != null)
                portraitObject.Add(image);

            return new JProperty(rewardPortrait.Id, portraitObject);
        }

        protected override JProperty GetImageObject(RewardPortrait rewardPortrait)
        {
            return new JProperty(
                "textureSheet",
                new JObject(
                    new JProperty("image", Path.ChangeExtension(rewardPortrait.TextureSheet.Image.ToLowerInvariant(), StaticImageExtension)),
                    new JProperty("columns", rewardPortrait.TextureSheet.Columns),
                    new JProperty("rows", rewardPortrait.TextureSheet.Rows)));
        }
    }
}
