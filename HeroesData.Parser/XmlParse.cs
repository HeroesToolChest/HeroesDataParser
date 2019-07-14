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
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (gameData == null)
                throw new ArgumentNullException(nameof(gameData));

            if (double.TryParse(gameData.GetValueFromAttribute(element.Attribute("value").Value), out double value))
                return value;
            else
                throw new FormatException($"Invalid value: {id} {element.Attribute("value").Value}");
        }

        /// <summary>
        /// Returns the value of the given elements from the value attribute.
        /// </summary>
        /// <param name="id">The element id. Used for exception handling.</param>
        /// <param name="element">The element to be parsed.</param>
        /// <param name="gameData"></param>
        /// <returns></returns>
        public static int GetIntValue(string id, XElement element, GameData gameData)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (gameData == null)
                throw new ArgumentNullException(nameof(gameData));

            if (int.TryParse(gameData.GetValueFromAttribute(element.Attribute("value").Value), out int value))
                return value;
            else
                throw new FormatException($"Invalid value: {id} {element.Attribute("value").Value}");
        }
    }
}
