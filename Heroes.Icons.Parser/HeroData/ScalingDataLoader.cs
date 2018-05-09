using System.Collections.Generic;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.HeroData
{
    public class ScalingDataLoader
    {
        private HeroDataLoader HeroDataLoader;

        public ScalingDataLoader(HeroDataLoader heroDataLoader)
        {
            HeroDataLoader = heroDataLoader;
        }

        /// <summary>
        /// Gets the scaling value by the lookup id: catalog#entry#field
        /// </summary>
        public Dictionary<string, double> ScaleValueByLookupId { get; set; } = new Dictionary<string, double>();

        public void Load()
        {
            ParseLevelScalingArrays();
        }

        private void ParseLevelScalingArrays()
        {
            IEnumerable<XElement> levelScalingArrays = HeroDataLoader.XmlData.Root.Descendants("LevelScalingArray");

            foreach (XElement scalingArray in levelScalingArrays)
            {
                foreach (XElement modification in scalingArray.Elements("Modifications"))
                {
                    string catalog = modification.Element("Catalog")?.Attribute("value")?.Value;
                    string entry = modification.Element("Entry")?.Attribute("value")?.Value;
                    string field = modification.Element("Field")?.Attribute("value")?.Value;
                    string value = modification.Element("Value")?.Attribute("value")?.Value;

                    if (string.IsNullOrEmpty(value))
                        continue;

                    string id = $"{catalog}#{entry}#{field}";

                    if (ScaleValueByLookupId.ContainsKey(id))
                        ScaleValueByLookupId[id] = double.Parse(value); // replace
                    else
                        ScaleValueByLookupId.Add(id, double.Parse(value));
                }
            }
        }
    }
}
