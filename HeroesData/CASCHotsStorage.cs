using CASCLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData
{
    internal class CASCHotsStorage
    {
        private readonly string _storagePath;

        private CASCHotsStorage(string storagePath)
        {
            _storagePath = storagePath;

            Initialize();
        }

        public CASCFolder? CASCFolderRoot { get; private set; }
        public CASCHandler? CASCHandler { get; private set; }

        public static CASCHotsStorage Load(string storagePath)
        {
            return new CASCHotsStorage(storagePath);
        }

        private static void DrawProgressBar(float percent, int barSize, char progressCharacter)
        {
            Console.CursorVisible = false;
            string p1 = string.Empty;
            string p2 = string.Empty;

            int chars = (int)Math.Round(percent / (1.0f / barSize));

            for (int i = 0; i < chars; i++)
                p1 += progressCharacter;
            for (int i = 0; i < barSize - chars; i++)
                p2 += progressCharacter;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{p1}");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"{p2}");

            Console.ResetColor();

            percent *= 100;
            Console.Write($" {percent:0}%");

            if (percent < 100)
                Console.Write("\r");
        }

        private static void DrawProgressBar(int complete, int maxVal, int barSize, char progressCharacter)
        {
            float percent = (float)complete / maxVal;
            DrawProgressBar(percent, barSize, progressCharacter);
        }

        private void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Loading local CASC storage...please wait");
            Console.ResetColor();

            TextWriter console = Console.Out;

            Stopwatch time = new Stopwatch();

            using ManualResetEvent resetEvent = new ManualResetEvent(false);
            using BackgroundWorkerEx backgroundWorker = new BackgroundWorkerEx();

            backgroundWorker.DoWork += (_, e) =>
            {
                Console.SetOut(TextWriter.Null); // suppress output
                CASCConfig config = CASCConfig.LoadLocalStorageConfig(_storagePath);
                CASCHandler = CASCHandler.OpenStorage(config, backgroundWorker);

                LocaleFlags locale = LocaleFlags.All;

                Console.SetOut(console); // enable output
                CASCHandler.Root.LoadListFile(Path.Combine(Environment.CurrentDirectory, "listfile.txt"), backgroundWorker);

                Console.SetOut(TextWriter.Null); // suppress output
                CASCFolderRoot = CASCHandler.Root.SetFlags(locale);
            };

            backgroundWorker.ProgressChanged += (_, e) =>
            {
                // main thread is blocked, so push it on another thread
                Task.Run(() => { DrawProgressBar(e.ProgressPercentage, 100, 72, '#'); });
            };

            backgroundWorker.RunWorkerCompleted += (_, e) =>
            {
                time.Stop();
                Console.SetOut(console); // enable output
                Console.Write("\r");
                DrawProgressBar(100, 100, 72, '#');

                Console.WriteLine();
                Console.WriteLine($"Finished in {time.Elapsed.TotalSeconds:0.####} seconds");
                Console.WriteLine();

                resetEvent.Set();
            };

            try
            {
                // use backgroundworker for progress reporting since it is provided by CASCLib
                // the main thread is blocked until the background process is completed
                time.Start();
                backgroundWorker.RunWorkerAsync();
                resetEvent.WaitOne();
            }
            catch (Exception ex)
            {
                resetEvent.Set();
                Console.SetOut(console);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Error while opening storage");
                Console.WriteLine(ex.Message);

                Console.ResetColor();
                Environment.Exit(1);
            }
        }
    }
}
