using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

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

            if (emoticon.SearchTexts != null && emoticon.SearchTexts.Count > 0 && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("searchText", string.Join(' ', emoticon.SearchTexts));

            if (!string.IsNullOrEmpty(emoticon.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add("description", GetTooltip(emoticon.Description, FileOutputOptions.DescriptionType));

            if (emoticon.LocalizedAliases != null && emoticon.LocalizedAliases.Count > 0 && !FileOutputOptions.IsLocalizedText)
                emoticonObject.Add(new JProperty("localizedAliases", emoticon.LocalizedAliases));

            if (emoticon.UniversalAliases != null && emoticon.UniversalAliases.Count > 0 && !FileOutputOptions.IsLocalizedText)
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
                    emoticonObject.Add("image", Path.ChangeExtension(emoticon.Image.FileName, StaticImageExtension));
                else
                    emoticonObject.Add("image", Path.ChangeExtension(emoticon.Image.FileName, AnimatedImageExtension));
            }

            JProperty animation = AnimationObject(emoticon);
            if (animation != null)
                emoticonObject.Add(animation);

            return new JProperty(emoticon.Id, emoticonObject);
        }

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
            return new JProperty(
                "animation",
                new JObject(
                    new JProperty("texture", Path.ChangeExtension(emoticon.TextureSheet.Image, StaticImageExtension)),
                    new JProperty("frames", emoticon.Image.Count),
                    new JProperty("duration", emoticon.Image.DurationPerFrame),
                    new JProperty("width", emoticon.Image.Width)));
        }
    }
}
