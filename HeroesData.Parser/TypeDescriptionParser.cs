using Heroes.Models;
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
    public class TypeDescriptionParser : ParserBase<TypeDescription, TypeDescriptionDataOverride>, IParser<TypeDescription?, TypeDescriptionParser>
    {
        public TypeDescriptionParser(IXmlDataService xmlDataService)
            : base(xmlDataService)
        {
        }

        protected override string ElementType => "CTypeDescription";

        public TypeDescriptionParser GetInstance()
        {
            return new TypeDescriptionParser(XmlDataService);
        }

        public TypeDescription? Parse(params string[] ids)
        {
            if (ids.Length < 1)
                return null;

            string id = ids.First();

            XElement? typeDescriptionElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == id));
            if (typeDescriptionElement == null)
                return null;

            TypeDescription typeDescription = new TypeDescription()
            {
                Id = id,
            };

            SetDefaultValues(typeDescription);
            SetTypeDescriptionData(typeDescriptionElement, typeDescription);

            if (string.IsNullOrEmpty(typeDescription.HyperlinkId))
                typeDescription.HyperlinkId = id;

            if (!string.IsNullOrEmpty(typeDescription.TextureSheet.Image))
                typeDescription.ImageFileName = $"storm_{nameof(typeDescription).ToLowerInvariant()}_{typeDescription.Id.ToLowerInvariant()}.dds";

            return typeDescription;
        }

        protected override bool ValidItem(XElement element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));

            return true;
        }

        private void SetTypeDescriptionData(XElement typeDescriptionElement, TypeDescription typeDescription)
        {
            // parent lookup
            string? parentValue = typeDescriptionElement.Attribute("parent")?.Value;
            if (!string.IsNullOrEmpty(parentValue))
            {
                XElement? parentElement = GameData.MergeXmlElements(GameData.Elements(ElementType).Where(x => x.Attribute("id")?.Value == parentValue));
                if (parentElement is not null)
                    SetTypeDescriptionData(parentElement, typeDescription);
            }

            foreach (XElement element in typeDescriptionElement.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    typeDescription.Name = element.Attribute("value")?.Value;
                }
                else if (elementName == "HYPERLINKID")
                {
                    typeDescription.HyperlinkId = element.Attribute("value")?.Value.Replace(DefaultData.IdPlaceHolder, typeDescription.Id, StringComparison.OrdinalIgnoreCase);
                }
                else if (elementName == "REWARDICON")
                {
                    string? textureSheetValue = element.Attribute("TextureSheet")?.Value;
                    string? indexValue = element.Attribute("Index")?.Value;

                    if (textureSheetValue is not null)
                    {
                        XElement? textureSheetElement = GameData.MergeXmlElements(GameData.Elements("CTextureSheet").Where(x => x.Attribute("id")?.Value == textureSheetValue));
                        if (textureSheetElement is not null)
                        {
                            foreach (XElement sheetElement in textureSheetElement.Elements())
                            {
                                string itemName = sheetElement.Name.LocalName.ToUpperInvariant();

                                if (itemName == "IMAGE")
                                {
                                    typeDescription.TextureSheet.Image = Path.GetFileName(PathHelper.GetFilePath(sheetElement.Attribute("value")?.Value))?.ToLowerInvariant();
                                }
                                else if (itemName == "ROWS")
                                {
                                    if (int.TryParse(sheetElement.Attribute("value")?.Value, out int value))
                                        typeDescription.TextureSheet.Rows = value;
                                }
                                else if (itemName == "COLUMNS")
                                {
                                    if (int.TryParse(sheetElement.Attribute("value")?.Value, out int value))
                                        typeDescription.TextureSheet.Columns = value;
                                }
                            }
                        }
                    }

                    if (indexValue is not null)
                    {
                        if (int.TryParse(indexValue, out int value))
                            typeDescription.IconSlot = value;
                    }
                }
                else if (elementName == "LARGEICON")
                {
                }
            }
        }

        private void SetDefaultValues(TypeDescription typeDescription)
        {
            typeDescription.Name = GameData.GetGameString(DefaultData.TypeDescriptionData!.TypeDescriptionName.Replace(DefaultData.IdPlaceHolder, typeDescription.Id, StringComparison.OrdinalIgnoreCase));
            typeDescription.TextureSheet = DefaultData.TypeDescriptionData!.TextureSheet;
        }
    }
}
