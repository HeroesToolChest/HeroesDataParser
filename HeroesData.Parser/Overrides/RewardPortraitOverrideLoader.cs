using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class RewardPortraitOverrideLoader : OverrideLoaderBase<RewardPortraitDataOverride>, IOverrideLoader
    {
        public RewardPortraitOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public RewardPortraitOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"rewardportrait-{base.OverrideFileName}";

        protected override string OverrideElementName => "CRewardPortrait";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
