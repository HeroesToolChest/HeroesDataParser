﻿using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class PortraitPackParser : ParserBase<PortraitPack, PortraitDataOverride>, IParser<PortraitPack?, PortraitPackParser>
    {
        public PortraitPackParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CPortraitPack";

        public PortraitPackParser GetInstance()
        {
            return new PortraitPackParser(XmlDataService);
        }

        public PortraitPack? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement? portraitElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (portraitElement == null)
                return null;

            PortraitPack portraitPack = new PortraitPack()
            {
                Id = id,
            };

            SetDefaultValues(portraitPack);
            SetPortraitPackData(portraitElement, portraitPack);

            if (string.IsNullOrEmpty(portraitPack.HyperlinkId))
                portraitPack.HyperlinkId = id;

            return portraitPack;
        }

        private void SetPortraitPackData(XElement portraitElement, PortraitPack portrait)
        {
            // parent lookup
            string? parentValue = portraitElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetPortraitPackData(parentElement, portrait);
            }

            foreach (XElement element in portraitElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "SORTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        portrait.SortName = text;
                }
                else if (elementName == "HYPERLINKID")
                {
                    portrait.HyperlinkId = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "NAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        portrait.Name = text;
                }
                else if (elementName == "EVENTNAME")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        portrait.EventName = text;
                    else
                        portrait.EventName = element.Attribute("value")?.Value;
                }
                else if (elementName == "RARITY")
                {
                    if (Enum.TryParse(element.Attribute("value").Value, out Rarity rarity))
                    {
                        portrait.Rarity = rarity;
                    }
                }
                else if (elementName == "PORTRAITARRAY")
                {
                    string item = element.Attribute("value")?.Value ?? string.Empty;

                    if (!string.IsNullOrEmpty(item))
                        portrait.RewardPortraitIds.Add(item);
                }
            }
        }

        private void SetDefaultValues(PortraitPack portrait)
        {
            portrait.Name = GameData.GetGameString(DefaultData.PortraitPackData?.PortraitName?.Replace(DefaultData.IdPlaceHolder, portrait.Id, StringComparison.OrdinalIgnoreCase));
            portrait.SortName = GameData.GetGameString(DefaultData.PortraitPackData?.PortraitSortName?.Replace(DefaultData.IdPlaceHolder, portrait.Id, StringComparison.OrdinalIgnoreCase));
            portrait.HyperlinkId = DefaultData.PortraitPackData?.PortraitHyperlinkId?.Replace(DefaultData.IdPlaceHolder, portrait.Id, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
        }
    }
}