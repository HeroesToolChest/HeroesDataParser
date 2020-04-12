using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataReward
    {
        private readonly GameData _gameData;

        public DefaultDataReward(GameData gameData)
        {
            _gameData = gameData;

            LoadCRewardDefault();
        }

        /// <summary>
        /// Gets the default reward name. Contains ##id##.
        /// </summary>
        public string RewardName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the default description text. Contains ##id##.
        /// </summary>
        public string RewardDescription { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the default description unearned text. Contains ##id##.
        /// </summary>
        public string RewardDescriptionUnearned { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the default hyperlink id. Contains ##id##.
        /// </summary>
        public string RewardHyperlinkId { get; private set; } = string.Empty;

        // <CReward default="1">
        private void LoadCRewardDefault()
        {
            CRewardElement(_gameData.Elements("CReward").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CRewardElement(IEnumerable<XElement> rewardElements)
        {
            foreach (XElement element in rewardElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    RewardName = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    RewardDescription = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTIONUNEARNED")
                {
                    RewardDescriptionUnearned = element.Attribute("value").Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    RewardHyperlinkId = element.Attribute("value").Value;
                }
            }
        }
    }
}
