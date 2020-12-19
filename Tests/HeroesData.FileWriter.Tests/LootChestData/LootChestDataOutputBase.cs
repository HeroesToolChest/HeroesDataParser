using Heroes.Models;

namespace HeroesData.FileWriter.Tests.LootChestData
{
    public class LootChestDataOutputBase : FileOutputTestBase<LootChest>
    {
        public LootChestDataOutputBase()
            : base(nameof(LootChestData))
        {
        }

        protected override void SetTestData()
        {
            LootChest lootChest = new LootChest()
            {
                Description = new TooltipDescription("legendary!!!!"),
                EventName = "event1",
                Rarity = Rarity.Legendary,
                HyperlinkId = "lootchest1",
                Id = "leglootchest1",
                Name = "Legend Chest",
                MaxRerolls = 10,
                TypeDescription = "typeDescription",
            };

            TestData.Add(lootChest);

            LootChest lootChest2 = new LootChest()
            {
                Id = "winterLoot1",
                HyperlinkId = "winterlootId1",
                MaxRerolls = 2,
                Rarity = Rarity.Rare,
                TypeDescription = "yep...",
            };

            TestData.Add(lootChest2);
        }
    }
}
