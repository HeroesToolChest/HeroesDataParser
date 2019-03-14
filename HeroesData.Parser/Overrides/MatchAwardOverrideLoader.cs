using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class MatchAwardOverrideLoader : OverrideLoaderBase<MatchAwardDataOverride>, IOverrideLoader
    {
        public MatchAwardOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override string OverrideFileName
        {
            get
            {
                return $"matchaward-{base.OverrideFileName}";
            }
        }

        protected override string OverrideElementName => "CAward";

        protected override void SetOverride(XElement element)
        {
            MatchAwardDataOverride matchAwardDataOverride = new MatchAwardDataOverride();

            string cAwardId = element.Attribute("id").Value;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value;

                switch (elementName)
                {
                    case "Id":
                        if (!string.IsNullOrEmpty(valueAttribute))
                            matchAwardDataOverride.IdOverride = (true, valueAttribute);
                        break;
                }
            }

            if (!DataOverridesById.ContainsKey(cAwardId))
                DataOverridesById.Add(cAwardId, matchAwardDataOverride);
        }
    }
}
