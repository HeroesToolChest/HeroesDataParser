using Heroes.Models;
using System;

namespace HeroesData.FileWriter.Tests.AnnouncerData
{
    public class AnnouncerDataOutputBase : FileOutputTestBase<Announcer>
    {
        public AnnouncerDataOutputBase()
            : base(nameof(AnnouncerData))
        {
        }

        protected override void SetTestData()
        {
            Announcer announcer = new Announcer()
            {
                Name = "Orphea",
                Id = "OrpheaA",
                AttributeId = "OE3",
                SortName = "qqOrpheaA",
                Rarity = Rarity.Rare,
                Description = new TooltipDescription("Orphea Announcer"),
                HyperlinkId = "OrpheaAId",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "Realm",
                Gender = "Female",
                HeroId = "Orphea",
                ImageFileName = "announcer_orphea.dds",
            };

            TestData.Add(announcer);

            Announcer announcer2 = new Announcer()
            {
                Name = "Murky",
                Id = "MurkyA",
                AttributeId = "ME3",
                SortName = "qqMurkyA",
                Rarity = Rarity.Rare,
                HyperlinkId = "MurkyAId",
                ReleaseDate = new DateTime(2016, 5, 21),
                CollectionCategory = "Warcraft",
                Gender = "Male",
                HeroId = "Murkey",
                ImageFileName = "announcer_murkey.dds",
            };

            TestData.Add(announcer2);
        }
    }
}
