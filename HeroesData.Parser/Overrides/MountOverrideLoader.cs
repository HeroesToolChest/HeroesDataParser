using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class MountOverrideLoader : OverrideLoaderBase<MountDataOverride>, IOverrideLoader
    {
        public MountOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
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
