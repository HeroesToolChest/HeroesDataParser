using HeroesData.Loader.XmlGameData;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public static class XmlParse
    {
        /// <summary>
        /// Returns the value of the given elements from the value attribute.
        /// </summary>
        /// <param name="id">The element id. Used for exception handling.</param>
        /// <param name="element">The element to be parsed.</param>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public static double GetDoubleValue(string id, XElement element, GameData gameData)
        {
            if (element == null || gameData == null)
                throw new ArgumentNullException();

            if (double.TryParse(gameData.GetValueFromAttribute(element.Attribute("value").Value), out double value))
                return value;
            else
                throw new FormatException($"Invalid value: {id} {element.Attribute("value").Value}");
        }
    }
}
