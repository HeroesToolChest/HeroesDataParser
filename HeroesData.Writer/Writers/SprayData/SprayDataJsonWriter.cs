using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writers.SprayData
{
    internal class SprayDataJsonWriter : SprayDataWriter<JProperty, JObject>
    {
        public SprayDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Spray spray)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(spray);

            JObject sprayObject = new JObject();

            if (!string.IsNullOrEmpty(spray.Name) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("name", spray.Name);

            sprayObject.Add("hyperlinkId", spray.HyperlinkId);
            sprayObject.Add("attributeId", spray.AttributeId);
            sprayObject.Add("rarity", spray.Rarity.ToString());
            sprayObject.Add("category", spray.CollectionCategory);

            if (!string.IsNullOrEmpty(spray.EventName))
                sprayObject.Add("event", spray.EventName);

            if (spray.ReleaseDate.HasValue)
                sprayObject.Add("releaseDate", spray.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(spray.SortName) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("sortName", spray.SortName);

            if (!string.IsNullOrEmpty(spray.SearchText) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("searchText", spray.SearchText);

            if (!string.IsNullOrEmpty(spray.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                sprayObject.Add("description", GetTooltip(spray.Description, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(spray.ImageFileName))
            {
                if (spray.AnimationCount < 1)
                    sprayObject.Add("image", Path.ChangeExtension(spray.ImageFileName, StaticImageExtension));
                else
                    sprayObject.Add("image", Path.ChangeExtension(spray.ImageFileName, AnimatedImageExtension));
            }

            JProperty animation = AnimationObject(spray);
            if (animation != null)
                sprayObject.Add(animation);

            return new JProperty(spray.Id, sprayObject);
        }

        protected override JProperty GetAnimationObject(Spray spray)
        {
            return new JProperty(
                "animation",
                new JObject(
                    new JProperty("texture", Path.ChangeExtension(spray.ImageFileName, StaticImageExtension)),
                    new JProperty("frames", spray.AnimationCount),
                    new JProperty("duration", spray.AnimationDuration)));
        }
    }
}
