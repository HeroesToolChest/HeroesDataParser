using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class BundleOverrideLoader : OverrideLoaderBase<BundleDataOverride>, IOverrideLoader
    {
        public BundleOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public BundleOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"bundle-{base.OverrideFileName}";

        protected override string OverrideElementName => "CBundle";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
