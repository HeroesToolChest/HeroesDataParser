using Heroes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.BundleData
{
    internal class BundleDataXmlWriter : BundleDataWriter<XElement, XElement>
    {
        public BundleDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Bundles";
        }

        protected override XElement GetHeroSkinsObject(Bundle bundle)
        {
            XElement heroSkinElement = new XElement("Skins");

            foreach (string heroId in bundle.HeroIdsWithHeroSkins)
            {
                if (bundle.TryGetSkinIdsByHeroId(heroId, out IEnumerable<string>? skinIds))
                {
                    foreach (string skinId in skinIds)
                    {
                        heroSkinElement.Add(new XElement("Skin", new XAttribute("hero", heroId), skinId));
                    }
                }
            }

            return heroSkinElement;
        }

        protected override XElement MainElement(Bundle bundle)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(bundle);

            return new XElement(
                XmlConvert.EncodeName(bundle.Id),
                string.IsNullOrEmpty(bundle.Name) || FileOutputOptions.IsLocalizedText ? null! : new XAttribute("name", bundle.Name),
                string.IsNullOrEmpty(bundle.HyperlinkId) ? null! : new XAttribute("hyperlinkId", bundle.HyperlinkId),
                bundle.ReleaseDate.HasValue ? new XAttribute("releaseDate", bundle.ReleaseDate.Value.ToString("yyyy-MM-dd")) : null!,
                bundle.Franchise is not null ? new XAttribute("franchise", bundle.Franchise) : null!,
                string.IsNullOrEmpty(bundle.EventName) ? null! : new XAttribute("event", bundle.EventName),
                bundle.IsDynamicContent ? new XAttribute("isDynamicContent", true) : null!,
                string.IsNullOrEmpty(bundle.SortName) || FileOutputOptions.IsLocalizedText ? null! : new XElement("SortName", bundle.SortName),
                bundle.HeroIds.Count > 0 ? new XElement("Heroes", bundle.HeroIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => new XElement("Feature", x))) : null!,
                HeroSkins(bundle)!,
                bundle.MountIds.Count > 0 ? new XElement("Mounts", bundle.MountIds.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).Select(x => new XElement("Mount", x))) : null!,
                string.IsNullOrEmpty(bundle.ImageFileName) ? null! : new XElement("Image", Path.ChangeExtension(bundle.ImageFileName.ToLowerInvariant(), StaticImageExtension)),
                string.IsNullOrEmpty(bundle.BoostBonusId) ? null! : new XElement("BoostId", bundle.BoostBonusId),
                bundle.GoldBonus is null ? null! : new XElement("GoldBonus", bundle.GoldBonus),
                bundle.GemsBonus is null ? null! : new XElement("GemsBonus", bundle.GemsBonus));
        }
    }
}
