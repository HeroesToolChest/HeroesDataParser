using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
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
            throw new NotImplementedException();
        }
    }
}
