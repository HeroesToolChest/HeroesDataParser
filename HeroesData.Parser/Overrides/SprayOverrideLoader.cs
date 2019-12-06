using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class SprayOverrideLoader : OverrideLoaderBase<SprayDataOverride>, IOverrideLoader
    {
        public SprayOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public SprayOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"spray-{base.OverrideFileName}";

        protected override string OverrideElementName => "CSpray";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
