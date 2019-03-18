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
            emoticon.LocalizedAliases.Add(":lunaraangry:");
            emoticon.LocalizedAliases.Add(":lunaangry:");
            emoticon.UniversalAliases.Add(":(");
            emoticon.UniversalAliases.Add("(:");

            TestData.Add(emoticon);

            Emoticon emoticon2 = new Emoticon()
            {
                Name = "Lunara Sad",
                Id = "lunara_sad",
            };

            TestData.Add(emoticon2);
        }
    }
}
