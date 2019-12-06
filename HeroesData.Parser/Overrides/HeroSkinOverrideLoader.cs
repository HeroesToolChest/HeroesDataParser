using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class HeroSkinOverrideLoader : OverrideLoaderBase<HeroSkinDataOverride>, IOverrideLoader
    {
        public HeroSkinOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public HeroSkinOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
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
