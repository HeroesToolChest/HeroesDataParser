using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class EmoticonPackOverrideLoader : OverrideLoaderBase<EmoticonPackDataOverride>, IOverrideLoader
    {
        public EmoticonPackOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"emoticonpack-{base.OverrideFileName}";

        protected override string OverrideElementName => "CEmoticonPack";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
