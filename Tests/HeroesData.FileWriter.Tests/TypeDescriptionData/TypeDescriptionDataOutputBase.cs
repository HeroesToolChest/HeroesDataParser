using Heroes.Models;

namespace HeroesData.FileWriter.Tests.TypeDescriptionData
{
    public class TypeDescriptionDataOutputBase : FileOutputTestBase<TypeDescription>
    {
        public TypeDescriptionDataOutputBase()
            : base(nameof(TypeDescriptionData))
        {
        }

        protected override void SetTestData()
        {
            TypeDescription typeDescription = new TypeDescription()
            {
                Id = "typedescription",
                Name = "type desc",
                HyperlinkId = "typedescription11",
                IconSlot = 5,
                TextureSheet = new TextureSheet()
                {
                    Image = "some_image.png",
                    Rows = 6,
                    Columns = 23,
                },
                ImageFileName = "typedescription.png",
            };

            TestData.Add(typeDescription);

            TypeDescription typeDescription2 = new TypeDescription()
            {
                Id = "typedescription22",
                HyperlinkId = "typedescription22",
                IconSlot = 5,
                TextureSheet = new TextureSheet()
                {
                    Image = "some_image333.png",
                    Rows = 6,
                    Columns = 23,
                },
            };

            TestData.Add(typeDescription2);
        }
    }
}
