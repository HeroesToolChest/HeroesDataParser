using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class AnnouncerOverrideLoader : OverrideLoaderBase<AnnouncerDataOverride>, IOverrideLoader
    {
        public AnnouncerOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"announcer-{base.OverrideFileName}";

        protected override string OverrideElementName => "CAnnouncerPack";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
