using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class EmoticonOverrideLoader : OverrideLoaderBase<EmoticonDataOverride>, IOverrideLoader
    {
        public EmoticonOverrideLoader(GameData gameData, int? hotsBuild)
            : base(gameData, hotsBuild)
        {
        }

        protected override string OverrideFileName
        {
            get
            {
                return $"emoticon-{base.OverrideFileName}";
            }
        }

        protected override string OverrideElementName => "CEmoticon";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
