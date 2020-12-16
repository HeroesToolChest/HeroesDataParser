using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writers.EmoticonData
{
    internal class EmoticonDataJsonWriter : EmoticonDataWriter<JProperty, JObject>
    {
        public EmoticonDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Emoticon emoticon)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(emoticon);

            JObject emoticonObject = new JObject();

            if (!string.IsNullOrEmpty(emoticon.Name) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("expression", emoticon.Name);

            if (!string.IsNullOrEmpty(emoticon.HyperlinkId))
                emoticonObject.Add("hyperlinkId", emoticon.HyperlinkId);

            if (emoticon.IsAliasCaseSensitive)
                emoticonObject.Add("caseSensitive", true);

            if (emoticon.IsHidden)
                emoticonObject.Add("isHidden", true);

            if (emoticon.SearchTexts != null && emoticon.SearchTexts.Any() && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("searchText", string.Join(' ', emoticon.SearchTexts));

            if (!string.IsNullOrEmpty(emoticon.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("description", GetTooltip(emoticon.Description, FileOutputOptions.DescriptionType));

            if (!string.IsNullOrEmpty(emoticon.DescriptionLocked?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("descriptionLocked", GetTooltip(emoticon.DescriptionLocked, FileOutputOptions.DescriptionType));

            if (emoticon.LocalizedAliases != null && emoticon.LocalizedAliases.Any() && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add(new JProperty("localizedAliases", emoticon.LocalizedAliases));

            if (emoticon.UniversalAliases != null && emoticon.UniversalAliases.Any() && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add(new JProperty("aliases", emoticon.UniversalAliases));

            if (!string.IsNullOrEmpty(emoticon.HeroId))
            {
                emoticonObject.Add(new JProperty("heroId", emoticon.HeroId));

                if (!string.IsNullOrEmpty(emoticon.HeroSkinId))
                    emoticonObject.Add(new JProperty("heroSkinId", emoticon.HeroSkinId));
            }

            if (!string.IsNullOrEmpty(emoticon.Image.FileName))
            {
                if (!emoticon.Image.Count.HasValue)
                    emoticonObject.Add("image", Path.ChangeExtension(emoticon.Image.FileName.ToLowerInvariant(), StaticImageExtension));
                else
                    emoticonObject.Add("image", Path.ChangeExtension(emoticon.Image.FileName.ToLowerInvariant(), AnimatedImageExtension));
            }

            JProperty? animation = AnimationObject(emoticon);
            if (animation != null)
                emoticonObject.Add(animation);

            return new JProperty(emoticon.Id, emoticonObject);
        }

        // not used on json writer
        protected override JProperty GetHeroElement(Emoticon emoticon)
        {
            JObject jObject = new JObject
            {
                new JProperty("id", emoticon.HeroId),
            };

            if (!string.IsNullOrEmpty(emoticon.HeroSkinId))
                jObject.Add(new JProperty("skinId", emoticon.HeroSkinId));

            return new JProperty("hero", jObject);
        }

        protected override JProperty GetAnimationObject(Emoticon emoticon)
        {
            JObject animationObject = new JObject(
                    new JProperty("texture", Path.ChangeExtension(emoticon.TextureSheet.Image?.ToLowerInvariant(), StaticImageExtension)),
                    new JProperty("frames", emoticon.Image.Count),
                    new JProperty("duration", emoticon.Image.DurationPerFrame),
                    new JProperty("width", emoticon.Image.Width));

            if (emoticon.TextureSheet.Columns.HasValue)
                animationObject.Add(new JProperty("columns", emoticon.TextureSheet.Columns.Value));
            if (emoticon.TextureSheet.Rows.HasValue)
                animationObject.Add(new JProperty("rows", emoticon.TextureSheet.Rows.Value));

            return new JProperty("animation", animationObject);
        }
    }
}
