namespace HeroesData.FileWriter.Writers
{
    internal interface IWritable
    {
        FileOutputOptions FileOutputOptions { get; set; }

        int? HotsBuild { get; set; }
    }
}
