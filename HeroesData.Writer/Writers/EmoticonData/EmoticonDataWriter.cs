using Heroes.Models;

namespace HeroesData.FileWriter.Writers.EmoticonData
{
    internal abstract class EmoticonDataWriter<T, TU> : WriterBase<Emoticon, T>
        where T : class
        where TU : class
    {
        public EmoticonDataWriter(FileOutputType fileOutputType)
            : base(nameof(EmoticonData), fileOutputType)
        {
        }

        protected abstract T GetHeroElement(Emoticon emoticon);
        protected abstract T GetAnimationObject(Emoticon emoticon);

        protected void AddLocalizedGameString(Emoticon emoticon)
        {
            GameStringWriter.AddEmoticonName(emoticon.Id, emoticon.Name);
            GameStringWriter.AddEmoticonDescription(emoticon.Id, GetTooltip(emoticon.Description, FileOutputOptions.DescriptionType));

            if (emoticon.LocalizedAliases != null && emoticon.LocalizedAliases.Count > 0)
                GameStringWriter.AddEmoticonAlias(emoticon.Id, string.Join(" ", emoticon.LocalizedAliases));

            if (emoticon.SearchTexts != null && emoticon.SearchTexts.Count > 0)
                GameStringWriter.AddEmoticonSearchText(emoticon.Id, string.Join(' ', emoticon.SearchTexts));
        }

        protected T HeroElement(Emoticon emoticon)
        {
            if (!string.IsNullOrEmpty(emoticon.HeroId))
            {
                return GetHeroElement(emoticon);
            }

            return null;
        }

        protected T AnimationObject(Emoticon emoticon)
        {
            if (emoticon.Image.Count.HasValue)
            {
                return GetAnimationObject(emoticon);
            }

            return null;
        }
    }
}
