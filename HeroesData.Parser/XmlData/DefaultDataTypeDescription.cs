using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataTypeDescription
    {
        private readonly GameData _gameData;

        public DefaultDataTypeDescription(GameData gameData)
        {
            _gameData = gameData;

            LoadCTypeDescriptionDefault();
        }

        /// <summary>
        /// Gets the default name text. Contains ##id##.
        /// </summary>
        public string TypeDescriptionName { get; private set; } = string.Empty;

        public TextureSheet TextureSheet { get; private set; } = new TextureSheet();

        // <CTypeDescription default="1">
        private void LoadCTypeDescriptionDefault()
        {
            CTypeDescriptionElement(_gameData.Elements("CTypeDescription").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        private void CTypeDescriptionElement(IEnumerable<XElement> cTypeDescriptionElements)
        {
            foreach (XElement element in cTypeDescriptionElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    TypeDescriptionName = element.Attribute("value")?.Value ?? string.Empty;
                }
                else if (elementName == "REWARDICON")
                {
                    string? textureSheetValue = element.Attribute("TextureSheet")?.Value;
                    if (textureSheetValue is not null)
                    {
                        XElement? textureSheetElement = GameData.MergeXmlElements(_gameData.Elements("CTextureSheet").Where(x => x.Attribute("id")?.Value == textureSheetValue));
                        if (textureSheetElement is not null)
                        {
                            foreach (XElement sheetElement in textureSheetElement.Elements())
                            {
                                string itemName = sheetElement.Name.LocalName.ToUpperInvariant();

                                if (itemName == "IMAGE")
                                {
                                    TextureSheet.Image = Path.GetFileName(PathHelper.GetFilePath(sheetElement.Attribute("value")?.Value))?.ToLowerInvariant();
                                }
                                else if (itemName == "ROWS")
                                {
                                    if (int.TryParse(sheetElement.Attribute("value")?.Value, out int value))
                                        TextureSheet.Rows = value;
                                }
                                else if (itemName == "COLUMNS")
                                {
                                    if (int.TryParse(sheetElement.Attribute("value")?.Value, out int value))
                                        TextureSheet.Columns = value;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
