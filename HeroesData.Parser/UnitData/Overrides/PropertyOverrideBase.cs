using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Overrides
{
    public abstract class PropertyOverrideBase<T>
        where T : class
    {
        private readonly GameData GameData;
        private readonly GameStringParser GameStringParser;

        public PropertyOverrideBase(GameData gameData)
        {
            GameData = gameData;
            GameStringParser = new GameStringParser(GameData);
        }

        public PropertyOverrideBase(GameData gameData, int? hotsBuild)
        {
            GameData = gameData;
            HotsBuild = hotsBuild;
            GameStringParser = new GameStringParser(GameData, hotsBuild);
        }

        protected int? HotsBuild { get; }

        public void SetOverride(string elementId, XElement element, Dictionary<string, Dictionary<string, Action<T>>> propertyOverrideMethodByElementId)
        {
            var propertyOverrides = new Dictionary<string, Action<T>>();

            // loop through each override child element
            foreach (XElement property in element.Elements())
            {
                string propertyName = property.Name.LocalName;
                string propertyValue = property.Attribute("value")?.Value;

                // text will override attribute value
                if (!string.IsNullOrEmpty(property.Value))
                    propertyValue = property.Value;

                // remove existing property override - duplicates will override previous
                if (propertyOverrides.ContainsKey(propertyName))
                    propertyOverrides.Remove(propertyName);

                SetPropertyValues(propertyName, propertyValue, propertyOverrides);
            }

            if (!propertyOverrideMethodByElementId.ContainsKey(elementId) && propertyOverrides.Count > 0)
                propertyOverrideMethodByElementId.Add(elementId, propertyOverrides);
        }

        protected abstract void SetPropertyValues(string propertyName, string propertyValue, Dictionary<string, Action<T>> propertyOverrides);

        protected double GetValue(string textValue)
        {
            if (double.TryParse(textValue, out double doubleValue))
            {
                return doubleValue;
            }
            else
            {
                try
                {
                    double? value = GameStringParser.ParseDRefString(textValue);
                    if (value.HasValue)
                        return value.Value;
                    else
                        throw new NullReferenceException($"Invalid dref text: {textValue}");
                }
                catch (Exception)
                {
                    throw new Exception("Invalid value");
                }
            }
        }
    }
}
