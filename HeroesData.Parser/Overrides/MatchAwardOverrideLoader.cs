using HeroesData.Parser.Overrides.DataOverrides;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class MatchAwardOverrideLoader : OverrideLoaderBase<MatchAwardDataOverride>, IOverrideLoader
    {
        public MatchAwardOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public MatchAwardOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"matchaward-{base.OverrideFileName}";

        protected override string OverrideElementName => "CAward";

        protected override void SetOverride(XElement element)
        {
            if (element is null)
                throw new System.ArgumentNullException(nameof(element));

            MatchAwardDataOverride matchAwardDataOverride = new MatchAwardDataOverride();

            string? cAwardId = element.Attribute("id")?.Value;

            foreach (XElement dataElement in element.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string? valueAttribute = dataElement.Attribute("value")?.Value;

                if (string.IsNullOrEmpty(valueAttribute))
                    continue;

                switch (elementName)
                {
                    case "Id":
                        matchAwardDataOverride.IdOverride = (true, valueAttribute);
                        break;
                    case "MVPScreenImageFileNameOriginal":
                        matchAwardDataOverride.MVPScreenImageFileNameOriginalOverride = (true, valueAttribute);
                        break;
                    case "MVPScreenImageFileName":
                        matchAwardDataOverride.MVPScreenImageFileNameOverride = (true, valueAttribute);
                        break;
                    case "ScoreScreenImageFileNameOriginal":
                        matchAwardDataOverride.ScoreScreenImageFileNameOriginalOverride = (true, valueAttribute);
                        break;
                    case "ScoreScreenImageFileName":
                        matchAwardDataOverride.ScoreScreenImageFileNameOverride = (true, valueAttribute);
                        break;
                    case "Description":
                        matchAwardDataOverride.DescriptionOverride = (true, valueAttribute);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(cAwardId) && !DataOverridesById.ContainsKey(cAwardId))
                DataOverridesById.Add(cAwardId, matchAwardDataOverride);
        }
    }
}
