using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataRewardPortrait
    {
        private readonly GameData _gameData;

        public DefaultDataRewardPortrait(GameData gameData)
        {
            _gameData = gameData;

            LoadCRewardPortraitDefault();
        }

        /// <summary>
        /// Gets the default number of icons in the column.
        /// </summary>
        public int PortraitIconColumns { get; private set; } = 0;

        /// <summary>
        /// Gets the default number of icons in the row.
        /// </summary>
        public int PortraitIconRows { get; private set; } = 0;

        /// <summary>
        /// Gets the default icon file name.
        /// </summary>
        public string? PortraitIconFileName { get; private set; }

        /// <summary>
        /// Gets the default collection category.
        /// </summary>
        public string? PortraitCollectionCategory { get; private set; }

        /// <summary>
        /// Gets the default rarity.
        /// </summary>
        public Rarity PortraitRarity { get; private set; } = Rarity.Common;

        // <CRewardPortrait default="1">
        private void LoadCRewardPortraitDefault()
        {
            CRewardPortraitElement(_gameData.Elements("CRewardPortrait").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CRewardPortraitElement(IEnumerable<XElement> cRewardPortraitElements)
        {
            foreach (XElement element in cRewardPortraitElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "ICONCOLS")
                {
                    if (int.TryParse(element.Attribute("value")?.Value, out int result))
                        PortraitIconColumns = result;
                }
                else if (elementName == "ICONROWS")
                {
                    if (int.TryParse(element.Attribute("value")?.Value, out int result))
                        PortraitIconRows = result;
                }
                else if (elementName == "ICONFILE")
                {
                    PortraitIconFileName = _gameData.GetValueFromAttribute(element.Attribute("value").Value);
                }
                else if (elementName == "COLLECTIONCATEGORY")
                {
                    PortraitCollectionCategory = element.Attribute("value").Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity rarity))
                    {
                        PortraitRarity = rarity;
                    }
                }
            }
        }
    }
}
