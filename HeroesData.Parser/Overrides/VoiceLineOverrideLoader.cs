using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class VoiceLineOverrideLoader : OverrideLoaderBase<VoiceLineDataOverride>, IOverrideLoader
    {
        public VoiceLineOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
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
