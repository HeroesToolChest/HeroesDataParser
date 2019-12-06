using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class VoiceLineOverrideLoader : OverrideLoaderBase<VoiceLineDataOverride>, IOverrideLoader
    {
        public VoiceLineOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public VoiceLineOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"voiceline-{base.OverrideFileName}";

        protected override string OverrideElementName => "CVoiceLine";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
