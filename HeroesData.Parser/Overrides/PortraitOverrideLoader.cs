using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class PortraitOverrideLoader : OverrideLoaderBase<PortraitDataOverride>, IOverrideLoader
    {
        public PortraitOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
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
