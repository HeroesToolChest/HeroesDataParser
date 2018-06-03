using CASCLib;
using System;
using System.Diagnostics;
using System.IO;

namespace HeroesData
{
    internal class CASCHotsStorage
    {
        private readonly string StoragePath;
        private readonly string Locale = "enUS";

        private CASCHotsStorage(string storagePath)
        {
            StoragePath = storagePath;

            Initialize();
        }

        public CASCFolder CASCFolderRoot { get; private set; }
        public CASCHandler CASCHandler { get; private set; }

        public static CASCHotsStorage Load(string storagePath)
        {
            return new CASCHotsStorage(storagePath);
        }

        private void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Loading local CASC storage...please wait");
            Console.ResetColor();

            TextWriter console = Console.Out;
            Console.SetOut(TextWriter.Null); // suppress output

            var time = new Stopwatch();

            try
            {
                time.Start();
                CASCConfig config = CASCConfig.LoadLocalStorageConfig(StoragePath);
                CASCHandler = CASCHandler.OpenStorage(config);

                LocaleFlags locale = (LocaleFlags)Enum.Parse(typeof(LocaleFlags), Locale);
                ContentFlags content = (ContentFlags)Enum.Parse(typeof(ContentFlags), "None");

                CASCHandler.Root.LoadListFile(Path.Combine(Environment.CurrentDirectory, "listfile.txt"));
                CASCFolderRoot = CASCHandler.Root.SetFlags(locale, content);

                time.Stop();
                Console.SetOut(console);
                Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
                Console.WriteLine(string.Empty);
            }
            catch (Exception ex)
            {
                Console.SetOut(console);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error while opening storage");
                Console.WriteLine(ex.Message);

                Console.ResetColor();
                Environment.Exit(1);
            }
        }
    }
}
