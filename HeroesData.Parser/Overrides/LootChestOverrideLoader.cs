using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public class LootChestOverrideLoader : OverrideLoaderBase<LootChestDataOverride>, IOverrideLoader
    {
        public LootChestOverrideLoader(int? hotsBuild)
            : base(hotsBuild)
        {
        }

        public LootChestOverrideLoader(string appPath, int? hotsBuild)
            : base(appPath, hotsBuild)
        {
        }

        protected override string OverrideFileName => $"lootchest-{base.OverrideFileName}";

        protected override string OverrideElementName => "CLootChest";

        protected override void SetOverride(XElement element)
        {
            throw new NotImplementedException();
        }
    }
}
