using Heroes.Models;

namespace HeroesData.FileWriter.Writers.LootChestData
{
    internal abstract class LootChestDataWriter<T, TU> : WriterBase<LootChest, T>
        where T : class
        where TU : class
    {
        protected LootChestDataWriter(FileOutputType fileOutputType)
            : base(nameof(LootChestData), fileOutputType)
        {
        }

        protected void AddLocalizedGameString(LootChest lootChest)
        {
            GameStringWriter.AddLootChestName(lootChest.Id, lootChest.Name);

            if (lootChest.Description is not null)
                GameStringWriter.AddLootChestDescription(lootChest.Id, GetTooltip(lootChest.Description, FileOutputOptions.DescriptionType));
        }
    }
}
