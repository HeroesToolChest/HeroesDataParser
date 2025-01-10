using Heroes.Models;
using Newtonsoft.Json.Linq;

namespace HeroesData.FileWriter.Writers.LootChestData
{
    internal class LootChestDataJsonWriter : LootChestDataWriter<JProperty, JObject>
    {
        public LootChestDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(LootChest lootChest)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(lootChest);

            JObject lootChestObject = new JObject();

            if (!string.IsNullOrEmpty(lootChest.Name) && !FileOutputOptions.IsLocalizedText)
                lootChestObject.Add("name", lootChest.Name);

            lootChestObject.Add("hyperlinkId", lootChest.HyperlinkId);
            lootChestObject.Add("rarity", lootChest.Rarity.ToString());

            if (!string.IsNullOrEmpty(lootChest.EventName))
                lootChestObject.Add("event", lootChest.EventName);

            lootChestObject.Add("maxRerolls", lootChest.MaxRerolls);
            lootChestObject.Add("typeDescription", lootChest.TypeDescription);

            if (!string.IsNullOrEmpty(lootChest.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                lootChestObject.Add("description", GetTooltip(lootChest.Description, FileOutputOptions.DescriptionType));

            return new JProperty(lootChest.Id, lootChestObject);
        }
    }
}
