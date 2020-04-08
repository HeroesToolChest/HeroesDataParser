﻿using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public class EmoticonParser : ParserBase<Emoticon, EmoticonDataOverride>, IParser<Emoticon?, EmoticonParser>
    {
        public EmoticonParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CEmoticon";

        public EmoticonParser GetInstance()
        {
            return new EmoticonParser(XmlDataService);
        }

        public Emoticon? Parse(params string[] ids)
        {
            if (ids == null || ids.Length < 1)
                return null;

            string id = ids.FirstOrDefault();

            XElement? emoticonElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (emoticonElement == null)
                return null;

            Emoticon emoticon = new Emoticon()
            {
                Id = id,
            };

            SetDefaultValues(emoticon);
            SetEmoticonData(emoticonElement, emoticon);

            if (!string.IsNullOrEmpty(emoticon.TextureSheet.Image))
                emoticon.Image.FileName = $"{Path.GetFileNameWithoutExtension(emoticon.TextureSheet.Image)}_{emoticon.Image.Index}{Path.GetExtension(emoticon.TextureSheet.Image)}";

            return emoticon;
        }

        private void SetEmoticonData(XElement emoticonElement, Emoticon emoticon)
        {
            // parent lookup
            string? parentValue = emoticonElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetEmoticonData(parentElement, emoticon);
            }
            else
            {
                string desc = GameData.GetGameString(DefaultData.EmoticonData?.EmoticonDescription?.Replace(DefaultData.IdPlaceHolder, emoticonElement.Attribute("id")?.Value, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(desc))
                    emoticon.Description = new TooltipDescription(desc);
            }

            foreach (XElement element in emoticonElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "DESCRIPTION")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        emoticon.Description = new TooltipDescription(text);
                }
                else if (elementName == "HERO")
                {
                    emoticon.HeroId = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "SKIN")
                {
                    emoticon.HeroSkinId = element.Attribute("value")?.Value;
                }
                else if (elementName == "SEARCHTEXTARRAY")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        emoticon.SearchTexts.Add(text!);
                    else
                        emoticon.SearchTexts.Add(element.Attribute("value")?.Value ?? string.Empty);
                }
                else if (elementName == "FLAGS")
                {
                    if (element.Attribute("index")?.Value == "CaseSensitive")
                    {
                        if (element.Attribute("value")?.Value == "1")
                            emoticon.IsAliasCaseSensitive = true;
                        else
                            emoticon.IsAliasCaseSensitive = false;
                    }
                    else if (element.Attribute("value")?.Value == "Hidden")
                    {
                        if (element.Attribute("value")?.Value == "1")
                            emoticon.IsHidden = true;
                        else
                            emoticon.IsHidden = false;
                    }
                }
                else if (elementName == "EXPRESSION")
                {
                    emoticon.Name = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "UNIVERSALALIASARRAY")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        emoticon.UniversalAliases.Add(text!);
                    else
                        emoticon.UniversalAliases.Add(element.Attribute("value")?.Value ?? string.Empty);
                }
                else if (elementName == "LOCALIZEDALIASARRAY")
                {
                    if (GameData.TryGetGameString(element.Attribute("value")?.Value ?? string.Empty, out string? text))
                        emoticon.LocalizedAliases.Add(text!);
                    else
                        emoticon.LocalizedAliases.Add(element.Attribute("value")?.Value ?? string.Empty);
                }
                else if (elementName == "IMAGE")
                {
                    string? textureSheet = element.Attribute("TextureSheet")?.Value;
                    string? width = element.Attribute("Width")?.Value;
                    string? index = element.Attribute("Index")?.Value;
                    string? count = element.Attribute("Count")?.Value;
                    string? durationPerFrame = element.Attribute("DurationPerFrame")?.Value;

                    if (!string.IsNullOrEmpty(textureSheet))
                    {
                        XElement? textureSheetElement = GameData.MergeXmlElements(GameData.Elements("CTextureSheet").Where(x => x.Attribute("id")?.Value == textureSheet));

                        if (textureSheetElement != null)
                            SetTextureSheetData(textureSheetElement, emoticon.TextureSheet);
                    }

                    if (!string.IsNullOrEmpty(width))
                        emoticon.Image.Width = int.Parse(width);

                    if (!string.IsNullOrEmpty(index))
                        emoticon.Image.Index = int.Parse(index);

                    if (!string.IsNullOrEmpty(count))
                        emoticon.Image.Count = int.Parse(count);

                    if (!string.IsNullOrEmpty(durationPerFrame))
                        emoticon.Image.DurationPerFrame = int.Parse(durationPerFrame);
                }
            }
        }

        private void SetTextureSheetData(XElement textureSheetElement, TextureSheet textureSheet)
        {
            // parent lookup
            string? parentValue = textureSheetElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements("CTextureSheet").Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement != null)
                    SetTextureSheetData(parentElement, textureSheet);
            }

            foreach (XElement element in textureSheetElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "IMAGE")
                {
                    textureSheet.Image = Path.GetFileName(PathHelper.GetFilePath(element.Attribute("value")?.Value ?? string.Empty)).ToLowerInvariant();
                }
                else if (elementName == "ROWS")
                {
                    if (int.TryParse(element.Attribute("value").Value, out int value))
                        textureSheet.Rows = value;
                }
                else if (elementName == "COLUMNS")
                {
                    if (int.TryParse(element.Attribute("value").Value, out int value))
                        textureSheet.Columns = value;
                }
            }
        }

        private void SetDefaultValues(Emoticon emoticon)
        {
            emoticon.Name = DefaultData.EmoticonData?.EmoticonExpression ?? string.Empty;
            emoticon.Description = new TooltipDescription(GameData.GetGameString(DefaultData.EmoticonData?.EmoticonDescription?.Replace(DefaultData.IdPlaceHolder, emoticon.Id, StringComparison.OrdinalIgnoreCase)));

            emoticon.Image.Index = 0;
            emoticon.Image.Width = 0;
            emoticon.Image.Count = null;
            emoticon.Image.DurationPerFrame = null;
        }
    }
}
