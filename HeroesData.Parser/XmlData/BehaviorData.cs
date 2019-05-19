using HeroesData.Loader.XmlGameData;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class BehaviorData
    {
        private readonly GameData GameData;

        public BehaviorData(GameData gameData)
        {
            GameData = gameData;
        }

        public string GetScalingBehaviorLink(XElement behaviorArrayElement)
        {
            string behaviorLink = behaviorArrayElement.Attribute("Link")?.Value;
            if (string.IsNullOrEmpty(behaviorLink))
                return string.Empty;

            XElement behaviorVeterancyElement = GameData.MergeXmlElements(GameData.Elements("CBehaviorVeterancy").Where(x => x.Attribute("id")?.Value == behaviorLink));
            if (behaviorVeterancyElement != null)
                return behaviorLink;

            return string.Empty;
        }
    }
}
