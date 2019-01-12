using Heroes.Models;
using HeroesData.Commands;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HeroesData
{
    internal class Program
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static void Main(string[] args)
        {
            App.SetCurrentCulture();

            CommandLineApplication app = new CommandLineApplication(true)
            {
                Description = "Extract Heroes of the Storm game data into XML and JSON format",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("-v|--version", $"Heroes Data Parser ({App.Version})");

            ReadCommand.Add(app).SetCommand();

            CommandOption storagePathOption = app.Option("-s|--storage-path <FILEPATH>", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory.", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = app.Option("-t|--threads <NUMBER>", "Limits the maximum amount of threads to use.", CommandOptionType.SingleValue);
            CommandOption extractIconsOption = app.Option("-e|--extract <VALUE>", $"Extracts images, available only in -s|--storage-path mode using the Hots directory.", CommandOptionType.MultipleValue);
            CommandOption setDescriptionOption = app.Option("-d|--description <VALUE>", "Set the description output type (0 - 6) - Default 0.", CommandOptionType.SingleValue);
            CommandOption setBuildOption = app.Option("-b|--build <number>", "Set the override build file.", CommandOptionType.SingleValue);
            CommandOption setOutputDirectoryOption = app.Option("-o|--output-directory <FILEPATH>", "Set the output directory.", CommandOptionType.SingleValue);
            CommandOption setGameStringLocalizations = app.Option("-l|--localization <LOCALE>", "Set the gamestring localization(s) - Default: enUS.", CommandOptionType.MultipleValue);
            CommandOption setFileSplitOption = app.Option("-f|--file-split", "Split the XML and JSON file(s) into multiple files.", CommandOptionType.NoValue);
            CommandOption xmlOutputOption = app.Option("--xml", "Create xml output.", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = app.Option("--json", "Create json output.", CommandOptionType.NoValue);
            CommandOption localizedTextOption = app.Option("--localized-text", "Extract localized gamestrings from the XML and JSON file(s) into a text file.", CommandOptionType.NoValue);
            CommandOption heroWarningsOption = app.Option("--hero-warnings", "Display all hero warnings.", CommandOptionType.NoValue);
            CommandOption excludeAwardParseOption = app.Option("--exclude-awards", "Exclude match award parsing.", CommandOptionType.NoValue);
            CommandOption minifyOption = app.Option("--minify", "Create .min file(s) along with current output file(s).", CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                App.Defaults = false;

                if (storagePathOption.HasValue())
                {
                    App.StoragePath = storagePathOption.Value();
                }

                if (setMaxDegreeParallismOption.HasValue() && int.TryParse(setMaxDegreeParallismOption.Value(), out int result))
                    App.MaxParallelism = result;

                if (setDescriptionOption.HasValue() && int.TryParse(setDescriptionOption.Value(), out result))
                    App.DescriptionType = result;

                if (setBuildOption.HasValue() && int.TryParse(setBuildOption.Value(), out result))
                    App.OverrideBuild = result;

                if (setOutputDirectoryOption.HasValue())
                    App.OutputDirectory = setOutputDirectoryOption.Value();
                else
                    App.OutputDirectory = Path.Combine(AssemblyPath, "output");

                if (extractIconsOption.HasValue() && storagePathOption.HasValue())
                {
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ALL"))
                    {
                        App.ExtractImagePortraits = true;
                        App.ExtractImageAbilityTalents = true;
                        App.ExtractMatchAwards = true;
                    }

                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "PORTRAITS"))
                        App.ExtractImagePortraits = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "TALENTS"))
                        App.ExtractImageTalents = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITIES"))
                        App.ExtractImageAbilities = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITYTALENTS"))
                        App.ExtractImageAbilityTalents = true;
                    if (extractIconsOption.Values.Exists(x => x.ToUpper() == "AWARDS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "MATCHAWARDS"))
                        App.ExtractMatchAwards = true;
                }

                if (setGameStringLocalizations.HasValue())
                {
                    IEnumerable<string> localizations = new List<string>();

                    if (setGameStringLocalizations.Values.Exists(x => x.ToUpper() == "ALL"))
                        localizations = Enum.GetNames(typeof(Localization));
                    else
                        localizations = setGameStringLocalizations.Values;

                    foreach (string locale in localizations)
                    {
                        if (Enum.TryParse(locale, true, out Localization localization))
                        {
                            App.Localizations.Add(localization);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unknown localization - {locale}");
                        }
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    App.Localizations.Add(Localization.ENUS);
                }

                App.CreateXml = xmlOutputOption.HasValue() ? true : false;
                App.CreateJson = jsonOutputOption.HasValue() ? true : false;
                App.ShowHeroWarnings = heroWarningsOption.HasValue() ? true : false;
                App.IsFileSplit = setFileSplitOption.HasValue() ? true : false;
                App.IsLocalizedText = localizedTextOption.HasValue() ? true : false;
                App.ExcludeAwardParsing = excludeAwardParseOption.HasValue() ? true : false;
                App.CreateMinFiles = minifyOption.HasValue() ? true : false;
                App.Run();
                Console.ResetColor();

                return 0;
            });

            if (args.Length > 0)
            {
                try
                {
                    app.Execute(args);
                }
                catch (CommandParsingException)
                {
                    return;
                }
            }
            else // defaults
            {
                App.Defaults = true;
                App.Run();
            }

            Console.ResetColor();

            Environment.Exit(0);
        }
    }
}
