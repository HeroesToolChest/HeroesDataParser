using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writers.TypeDescriptionData
{
    internal class TypeDescriptionDataJsonWriter : TypeDescriptionDataWriter<JProperty, JObject>
    {
        public TypeDescriptionDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(TypeDescription typeDescription)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(typeDescription);

            JObject typeDescriptionObject = new JObject();

            if (!string.IsNullOrEmpty(typeDescription.Name) && !FileOutputOptions.IsLocalizedText)
                typeDescriptionObject.Add("name", typeDescription.Name);

            typeDescriptionObject.Add("hyperlinkId", typeDescription.HyperlinkId);
            typeDescriptionObject.Add("iconSlot", typeDescription.IconSlot);

            JProperty? image = GetImageObject(typeDescription);
            if (image != null)
                typeDescriptionObject.Add(image);

            if (!string.IsNullOrEmpty(typeDescription.ImageFileName))
                typeDescriptionObject.Add("image", Path.ChangeExtension(typeDescription.ImageFileName.ToLowerInvariant(), StaticImageExtension));

            return new JProperty(typeDescription.Id, typeDescriptionObject);
        }

        protected override JProperty GetImageObject(TypeDescription typeDescription)
        {
            JObject textureSheetObject = new JObject(new JObject(
                    new JProperty("image", Path.ChangeExtension(typeDescription.TextureSheet.Image?.ToLowerInvariant(), StaticImageExtension))));

            if (typeDescription.TextureSheet.Columns.HasValue)
                textureSheetObject.Add(new JProperty("columns", typeDescription.TextureSheet.Columns.Value));
            if (typeDescription.TextureSheet.Rows.HasValue)
                textureSheetObject.Add(new JProperty("rows", typeDescription.TextureSheet.Rows.Value));

            return new JProperty("textureSheet", textureSheetObject);
        }
    }
}
