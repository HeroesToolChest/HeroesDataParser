using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class MountOverrideLoader : OverrideLoaderBase<MountDataOverride>, IOverrideLoader
    {
        public MountOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public MountOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"mount-{base.OverrideFileName}";

        protected override string OverrideElementName => "CMount";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
