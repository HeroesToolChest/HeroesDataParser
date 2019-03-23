using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.EmoticonPackData
{
    public class EmoticonPackDataOutputBase : FileOutputTestBase<EmoticonPack>
    {
        public EmoticonPackDataOutputBase()
            : base(nameof(EmoticonPackData))
        {
        }

        protected override void SetTestData()
        {
            EmoticonPack emoticonPack = new EmoticonPack()
            {
                Name = "Orphea Pack 1",
                Id = "OrpheaPack1",
                Description = new TooltipDescription("Emoticon pack for Orphea"),
                HyperlinkId = "OrpheaPack1",
                CollectionCategory = "Nexus",
                EventName = "EvilTower",
                SortName = "xxOrpheaPack1",
                ReleaseDate = new DateTime(2018, 4, 3),
            };

            emoticonPack.EmoticonIds.Add("orphea_sad");
            emoticonPack.EmoticonIds.Add("orphea_happy");

            TestData.Add(emoticonPack);

            EmoticonPack emoticonPack2 = new EmoticonPack()
            {
                Name = "Orphea Pack 2",
                Id = "OrpheaPack2",
                CollectionCategory = "Nexus",
                ReleaseDate = new DateTime(2018, 4, 3),
            };

            TestData.Add(emoticonPack2);
        }
    }
}
