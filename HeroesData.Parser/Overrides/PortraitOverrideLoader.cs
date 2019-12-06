using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class PortraitOverrideLoader : OverrideLoaderBase<PortraitDataOverride>, IOverrideLoader
    {
        public PortraitOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public PortraitOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"portrait-{base.OverrideFileName}";

        protected override string OverrideElementName => "CPortraitPack";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
