using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class BehaviorVeterancyOverrideLoader : OverrideLoaderBase<BehaviorVeterancyDataOverride>, IOverrideLoader
    {
        public BehaviorVeterancyOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public BehaviorVeterancyOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"veterancy-{base.OverrideFileName}";

        protected override string OverrideElementName => "CBehaviorVeterancy";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
