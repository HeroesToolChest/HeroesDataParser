using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class BoostOverrideLoader : OverrideLoaderBase<BoostDataOverride>, IOverrideLoader
    {
        public BoostOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public BoostOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"boost-{base.OverrideFileName}";

        protected override string OverrideElementName => "CBoost";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
