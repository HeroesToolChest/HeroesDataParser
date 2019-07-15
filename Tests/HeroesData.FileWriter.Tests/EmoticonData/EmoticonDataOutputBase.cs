using Heroes.Models;

namespace HeroesData.FileWriter.Tests.EmoticonData
{
    public class EmoticonDataOutputBase : FileOutputTestBase<Emoticon>
    {
        public EmoticonDataOutputBase()
            : base(nameof(EmoticonData))
        {
        }

        protected override void SetTestData()
        {
            Emoticon emoticon = new Emoticon()
            {
                Name = "Lunara Angry",
                Id = "lunara_angry",
                Description = new TooltipDescription("Use emoticons for lunara"),
                HyperlinkId = string.Empty,
                HeroId = "Lunara",
                HeroSkinId = "LunaraWitch",
            };
            emoticon.Image.FileName = "emoticon_image.png";
            emoticon.Image.Count = 2;
            emoticon.Image.DurationPerFrame = 1000;
            emoticon.Image.Width = 34;
            emoticon.AddLocalizedAlias(":lunaraangry:");
            emoticon.AddLocalizedAlias(":lunaangry:");
            emoticon.AddLocalizedAlias(":lunaangry:");
            emoticon.AddUniversalAlias(":(");
            emoticon.AddUniversalAlias("(:");
            emoticon.AddUniversalAlias("(:");
            emoticon.TextureSheet.Image = "emoticon_image_texture.png";

            TestData.Add(emoticon);

            Emoticon emoticon2 = new Emoticon()
            {
                Name = "Lunara Sad",
                Id = "lunara_sad",
                IsHidden = true,
            };

            TestData.Add(emoticon2);
        }
    }
}
