using Heroes.Models;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HeroesData.FileWriter.Writers.AnnouncerData
{
    internal class AnnouncerDataJsonWriter : AnnouncerDataWriter<JProperty, JObject>
    {
        public AnnouncerDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(Announcer announcer)
        {
            if (FileOutputOptions.IsLocalizedText)
                AddLocalizedGameString(announcer);

            JObject announcerObject = new JObject();

            if (!string.IsNullOrEmpty(announcer.Name) && !FileOutputOptions.IsLocalizedText)
                announcerObject.Add("name", announcer.Name);

            if (!string.IsNullOrEmpty(announcer.SortName) && !FileOutputOptions.IsLocalizedText)
                announcerObject.Add("sortName", announcer.SortName);

            announcerObject.Add("hyperlinkId", announcer.HyperlinkId);
            announcerObject.Add("attributeId", announcer.AttributeId);
            announcerObject.Add("rarity", announcer.Rarity.ToString());

            if (announcer.ReleaseDate.HasValue)
                announcerObject.Add("releaseDate", announcer.ReleaseDate.Value.ToString("yyyy-MM-dd"));

            announcerObject.Add("category", announcer.CollectionCategory);

            if (!string.IsNullOrEmpty(announcer.Gender))
                announcerObject.Add("gender", announcer.Gender);

            announcerObject.Add("heroId", announcer.HeroId);

            if (!string.IsNullOrEmpty(announcer.ImageFileName))
                announcerObject.Add("image", Path.ChangeExtension(announcer.ImageFileName.ToLowerInvariant(), StaticImageExtension));


            if (!string.IsNullOrEmpty(announcer.Description?.RawDescription) && !FileOutputOptions.IsLocalizedText)
                announcerObject.Add("description", GetTooltip(announcer.Description, FileOutputOptions.DescriptionType));

            return new JProperty(announcer.Id, announcerObject);
        }
    }
}
