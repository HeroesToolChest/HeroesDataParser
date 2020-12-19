using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class LootChestParser : ParserBase<LootChest, LootChestDataOverride>, IParser<LootChest?, LootChestParser>
    {
        public LootChestParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CLootChest";

        public LootChestParser GetInstance()
        {
            return new LootChestParser(XmlDataService);
        }

        public LootChest? Parse(params string[] ids)
        {
            if (ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? lootChestElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (lootChestElement == null)
                return null;

            LootChest lootChest = new LootChest()
            {
                Id = id,
            };

            SetDefaultValues(lootChest);
            SetLootChestData(lootChestElement, lootChest);

            if (string.IsNullOrEmpty(lootChest.HyperlinkId))
                lootChest.HyperlinkId = id;

            return lootChest;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return true;
        }

        private void SetLootChestData(XElement lootChestElement, LootChest lootChest)
        {
            // parent lookup
            string? parentValue = lootChestElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue && x.Attribute("parent")?.Value != parentValue));
                if (parentElement is not null)
                    SetLootChestData(parentElement, lootChest);
            }

            foreach (XElement element in lootChestElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "HYPERLINKID")
                {
                    lootChest.HyperlinkId = element.Attribute("value")?.Value.Replace(DefaultData.IdPlaceHolder, lootChest.Id, StringComparison.OrdinalIgnoreCase);
                }
                else if (elementName == "NAME")
                {
                    string? nameValue = element.Attribute("value")?.Value.Replace(DefaultData.IdPlaceHolder, lootChest.Id, StringComparison.OrdinalIgnoreCase);
                    if (nameValue is not null)
                    {
                        if (GameData.TryGetGameString(nameValue, out string? text))
                            lootChest.Name = text;
                    }
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value")?.Value, out Rarity rarity))
                    {
                        lootChest.Rarity = rarity;
                    }
                }
                else if (elementName == "DESCRIPTION")
                {
                    string? descriptionValue = element.Attribute("value")?.Value;
                    if (descriptionValue is not null)
                    {
                        lootChest.Description = new TooltipDescription(GameData.GetGameString(descriptionValue));
                    }
                }
                else if (elementName == "EVENTNAME")
                {
                    lootChest.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "MAXREROLLS")
                {
                    lootChest.MaxRerolls = XmlParse.GetIntValue(lootChest.Id, element, GameData);
                }
                else if (elementName == "TYPEDESCRIPTION")
                {
                    lootChest.TypeDescription = element.Attribute("value")?.Value.Replace(DefaultData.IdPlaceHolder, lootChest.Id, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        private void SetDefaultValues(LootChest lootChest)
        {
            lootChest.Description = new TooltipDescription(GameData.GetGameString(DefaultData.LootChestData?.ToolChestDescription.Replace(DefaultData.IdPlaceHolder, lootChest.Id, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
