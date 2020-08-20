using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataEmoticon
    {
        private readonly GameData _gameData;

        public DefaultDataEmoticon(GameData gameData)
        {
            _gameData = gameData;

            LoadCEmoticonDefault();
            LoadCTextureSheetDefault();
        }

        /// <summary>
        /// Gets the default emoticon localized alias array value. Contains ##id##.
        /// </summary>
        public string? EmoticonLocalizedAliasArray { get; private set; }

        /// <summary>
        /// Gets the default emoticon description. Contains ##id##.
        /// </summary>
        public string? EmoticonDescription { get; private set; }

        /// <summary>
        /// Gets the default emoticon description locked. Contains ##id##.
        /// </summary>
        public string? EmoticonDescriptionLocked { get; private set; }

        /// <summary>
        /// Gets the default emoticon expression.
        /// </summary>
        public string? EmoticonExpression { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the default emoticon status is hidden.
        /// </summary>
        public bool EmoticonIsHidden { get; private set; }

        /// <summary>
        /// Gets the default emoticon texture sheet.
        /// </summary>
        public string? EmoticonTextureSheet { get; private set; }

        /// <summary>
        /// Gets the default texture sheet image file name. Contains ##id##.
        /// </summary>
        public string? TextureSheetImage { get; private set; }

        /// <summary>
        /// Gets the default texture sheet rows.
        /// </summary>
        public int TextureSheetRows { get; private set; }

        /// <summary>
        /// Gets the default texture sheet columns.
        /// </summary>
        public int TextureSheetColumns { get; private set; }

        // <CEmoticon default="1">
        private void LoadCEmoticonDefault()
        {
            CEmoticonElement(_gameData.Elements("CEmoticon").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CTextureSheet default="1">
        private void LoadCTextureSheetDefault()
        {
            CTextureSheetElement(_gameData.Elements("CTextureSheet").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CEmoticonElement(IEnumerable<XElement> cEmoticonElements)
        {
            foreach (XElement element in cEmoticonElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "LOCALIZEDALIASARRAY")
                {
                    EmoticonLocalizedAliasArray = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTION")
                {
                    EmoticonDescription = element.Attribute("value").Value;
                }
                else if (elementName == "DESCRIPTIONLOCKED")
                {
                    EmoticonDescriptionLocked = element.Attribute("value").Value;
                }
                else if (elementName == "EXPRESSION")
                {
                    EmoticonExpression = element.Attribute("value").Value;
                }
                else if (elementName == "IMAGE")
                {
                    EmoticonTextureSheet = element.Attribute("TextureSheet").Value;
                }
                else if (elementName == "FLAGS")
                {
                    if (element.Attribute("value")?.Value == "Hidden")
                    {
                        if (element.Attribute("value")?.Value == "1")
                            EmoticonIsHidden = true;
                        else
                            EmoticonIsHidden = false;
                    }
                }
            }
        }

        private void CTextureSheetElement(IEnumerable<XElement> cTextureSheetElements)
        {
            foreach (XElement element in cTextureSheetElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "IMAGE")
                {
                    TextureSheetImage = element.Attribute("value").Value;
                }
                else if (elementName == "ROWS")
                {
                    if (int.TryParse(element.Attribute("value").Value, out int value))
                        TextureSheetRows = value;
                }
                else if (elementName == "COLUMNS")
                {
                    if (int.TryParse(element.Attribute("value").Value, out int value))
                        TextureSheetColumns = value;
                }
            }
        }
    }
}
