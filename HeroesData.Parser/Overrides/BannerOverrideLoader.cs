using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class BannerOverrideLoader : OverrideLoaderBase<BannerDataOverride>, IOverrideLoader
    {
        public BannerOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"banner-{base.OverrideFileName}";

        protected override string OverrideElementName => "CBanner";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
