using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class TypeDescriptionOverrideLoader : OverrideLoaderBase<TypeDescriptionDataOverride>, IOverrideLoader
    {
        public TypeDescriptionOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public TypeDescriptionOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"typedescription-{base.OverrideFileName}";

        protected override string OverrideElementName => "CTypeDescription";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
