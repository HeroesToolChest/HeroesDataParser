using Heroes.Models;
using HeroesData.Parser;
namespace HeroesData.ExtractorData
{
    public class DataLootChest : DataExtractorBase<LootChest?, LootChestParser>, IData
    {
        public DataLootChest(LootChestParser parser)
            : base(parser)
        {
        }

        public override string Name => "lootchests";

        protected override void Validation(LootChest? data)
        {
            if (data is null)
                return;

            if (string.IsNullOrEmpty(data.Id))
                AddWarning($"{nameof(data.Id)} is empty");

            if (string.IsNullOrEmpty(data.HyperlinkId))
                AddWarning($"{nameof(data.HyperlinkId)} is empty");
        }
    }
}
