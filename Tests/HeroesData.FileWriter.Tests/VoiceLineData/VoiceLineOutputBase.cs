using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.VoiceLineData
{
    public class VoiceLineOutputBase : FileOutputTestBase<VoiceLine>
    {
        public VoiceLineOutputBase()
            : base(nameof(VoiceLineData))
        {
        }

        protected override void SetTestData()
        {
            VoiceLine voiceLine = new VoiceLine()
            {
                Name = "Common1",
                Id = "Common1",
                AttributeId = "AB01",
                SortName = string.Empty,
                Description = new TooltipDescription(string.Empty),
                HyperlinkId = "Common1Id",
                ReleaseDate = new DateTime(2016, 5, 21),
                Rarity = Rarity.Epic,
            };

            TestData.Add(voiceLine);

            VoiceLine voiceLine2 = new VoiceLine()
            {
                Name = "Common2",
                Id = "Common2",
                AttributeId = "AB02",
                SortName = "xxCommon2",
                Description = new TooltipDescription("some voice talking"),
                HyperlinkId = "Common2Id",
                ReleaseDate = new DateTime(2014, 5, 21),
            };

            TestData.Add(voiceLine2);
        }
    }
}
