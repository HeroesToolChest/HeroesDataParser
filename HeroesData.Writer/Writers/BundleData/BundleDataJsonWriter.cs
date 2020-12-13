using Heroes.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writers.BundleData
{
    internal class BundleDataJsonWriter : BundleDataWriter<JProperty, JObject>
    {
        public BundleDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty GetHeroSkinsObject(Bundle bundle)
        {
            JArray heroSkinsArray = new JArray();

            foreach (string heroId in bundle.HeroIdsWithHeroSkins)
            {
                if (bundle.TryGetSkinIdsByHeroId(heroId, out IEnumerable<string>? heroSkins))
                {
                    heroSkinsArray.Add(new JObject(new JProperty(heroId, heroSkins.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))));
                }
            }

            return new JProperty("skins", heroSkinsArray);
        }

        protected override JProperty MainElement(Bundle bundle)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(bundle);

            JObject bundleObject = new JObject();

            if (!string.IsNullOrEmpty(bundle.Name) && !FileOutputOptions.IsLocalizedText)
                bundleObject.Add("name", bundle.Name);

            bundleObject.Add("hyperlinkId", bundle.HyperlinkId);

            if (bundle.ReleaseDate.HasValue)
                bundleObject.Add("releaseDate", bundle.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(bundle.SortName) && !FileOutputOptions.IsLocalizedText)
                bundleObject.Add("sortName", bundle.SortName);

            if (bundle.Franchise is not null)
                bundleObject.Add("franchise", bundle.Franchise.ToString());

            if (!string.IsNullOrEmpty(bundle.EventName))
                bundleObject.Add("event", bundle.EventName);

            if (bundle.IsDynamicContent)
                bundleObject.Add("IsDynamicContent", true);

            if (bundle.HeroIds.Count > 0)
                bundleObject.Add(new JProperty("heroes", bundle.HeroIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)));

            JProperty? heroSkins = HeroSkins(bundle);
            if (heroSkins is not null)
                bundleObject.Add(heroSkins);

            if (bundle.MountIds.Count > 0)
                bundleObject.Add(new JProperty("mounts", bundle.MountIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)));

            if (!string.IsNullOrEmpty(bundle.ImageFileName))
                bundleObject.Add("image", Path.ChangeExtension(bundle.ImageFileName?.ToLowerInvariant(), StaticImageExtension));

            if (!string.IsNullOrEmpty(bundle.BoostBonusId))
                bundleObject.Add("boostId", bundle.BoostBonusId);

            if (bundle.GoldBonus is not null)
                bundleObject.Add("goldBonus", bundle.GoldBonus);

            if (bundle.GemsBonus is not null)
                bundleObject.Add("gemsBonus", bundle.GemsBonus);

            return new JProperty(bundle.Id, bundleObject);
        }
    }
}
