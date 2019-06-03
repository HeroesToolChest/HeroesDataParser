using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class EmoticonOverrideLoader : OverrideLoaderBase<EmoticonDataOverride>, IOverrideLoader
    {
        public EmoticonOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        protected override string OverrideFileName => $"emoticon-{base.OverrideFileName}";

        protected override string OverrideElementName => "CEmoticon";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
