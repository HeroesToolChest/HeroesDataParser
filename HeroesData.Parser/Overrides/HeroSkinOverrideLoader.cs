using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class HeroSkinOverrideLoader : OverrideLoaderBase<HeroSkinDataOverride>, IOverrideLoader
    {
        public HeroSkinOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"heroskin-{base.OverrideFileName}";

        protected override string OverrideElementName => "CSkin";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
