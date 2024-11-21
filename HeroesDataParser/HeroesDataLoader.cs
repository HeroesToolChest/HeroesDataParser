using CASCLib;

namespace HeroesDataParser;

public static class HeroesDataLoader
{
    public static async Task<HeroesXmlLoader> Load()
    {
        HeroesXmlLoader? heroesXmlLoader = null;
        using BackgroundWorkerEx backgroundWorkerEx = new();
        backgroundWorkerEx.DoWork += (_, e) =>
        {
            heroesXmlLoader = HeroesXmlLoader.LoadWithCASC("E:\\Games\\Heroes of the Storm", backgroundWorkerEx);
            //heroesXmlLoader2 = HeroesXmlLoader.LoadAsOnlineCASC(backgroundWorkerEx);
            //heroesXmlLoader2 = HeroesXmlLoader.LoadWithFile("F:\\heroes\\heroes_91418\\mods_all_91418", backgroundWorkerEx);
            //heroesXmlLoader2 = HeroesXmlLoader.LoadWithFile("F:\\heroes\\heroes_92264\\mods_92264", backgroundWorkerEx);

            heroesXmlLoader
                .LoadStormMods()
                //.LoadMapMod("Volskaya Foundry")
                .LoadGameStrings();
        };
        backgroundWorkerEx.ProgressChanged += (_, e) =>
        {
            Task.Run(() =>
            {
                Console.WriteLine($"{e.ProgressPercentage} - {e.UserState}");
            });
        };
        backgroundWorkerEx.RunWorkerCompleted += (_, e) =>
        {
            Console.WriteLine("done");

        };

        backgroundWorkerEx.RunWorkerAsync();


        while (backgroundWorkerEx.IsBusy)
        {
            await Task.Delay(1000);
        }

        //heroesXmlLoader.LoadMapMod("Volskaya Foundry");

        //var a = heroesXmlLoader.HeroesData.GetLoadedStormBattleGroundMap();
        return heroesXmlLoader;
        //IHeroesDataService heroesDataService = new HeroesDataService()
        //{
        //    HeroesData = heroesData,
        //};
    }
}
