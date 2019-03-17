using Heroes.Models;
using HeroesData.Commands;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HeroesData
{
    public class Program
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Main(string[] args)
        {
            App app = new App();
            app.SetCurrentCulture();

            CommandLineApplication commandLineApplication = new CommandLineApplication(true)
            {
                Description = "Extract Heroes of the Storm game data into XML and JSON format",
            };
            commandLineApplication.HelpOption("-?|-h|--help");
            commandLineApplication.VersionOption("-v|--version", $"Heroes Data Parser ({App.Version})");

            ReadCommand.Add(commandLineApplication).SetCommand();

            CommandOption storagePathOption = commandLineApplication.Option("-s|--storage-path <FILEPATH>", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory.", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = commandLineApplication.Option("-t|--threads <NUMBER>", "Limits the maximum amount of threads to use.", CommandOptionType.SingleValue);
            CommandOption extractIconsOption = commandLineApplication.Option("-e|--extract <VALUE>", $"Extracts images, available only in -s|--storage-path mode using the Hots directory.", CommandOptionType.MultipleValue);
            CommandOption setDescriptionOption = commandLineApplication.Option("-d|--description <VALUE>", "Sets the description output type (0 - 6) - Default 0.", CommandOptionType.SingleValue);
            CommandOption setBuildOption = commandLineApplication.Option("-b|--build <number>", "Sets the override build file.", CommandOptionType.SingleValue);
            CommandOption setOutputDirectoryOption = commandLineApplication.Option("-o|--output-directory <FILEPATH>", "Sets the output directory.", CommandOptionType.SingleValue);
            CommandOption setGameStringLocalizations = commandLineApplication.Option("-l|--localization <LOCALE>", "Sets the gamestring localization(s) - Default: enUS.", CommandOptionType.MultipleValue);
            CommandOption setFileSplitOption = commandLineApplication.Option("-f|--file-split", "Splits the XML and JSON file(s) into multiple files.", CommandOptionType.NoValue);
            CommandOption xmlOutputOption = commandLineApplication.Option("--xml", "Creates xml output.", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = commandLineApplication.Option("--json", "Creates json output.", CommandOptionType.NoValue);
            CommandOption localizedTextOption = commandLineApplication.Option("--localized-text", "Extracts localized gamestrings from the XML and JSON file(s) into a text file.", CommandOptionType.NoValue);
            CommandOption validationWarningsOption = commandLineApplication.Option("--warnings", "Displays all validation warnings.", CommandOptionType.NoValue);
            CommandOption excludeAwardParseOption = commandLineApplication.Option("--exclude-awards", "Excludes match award parsing.", CommandOptionType.NoValue);
            CommandOption minifyOption = commandLineApplication.Option("--minify", "Creates .min file(s) along with current output file(s).", CommandOptionType.NoValue);

            commandLineApplication.OnExecute(() =>
            {
                App.Defaults = false;

                if (storagePathOption.HasValue())
                {
                    App.StoragePath = storagePathOption.Value();
                }

                if (setMaxDegreeParallismOption.HasValue() && int.TryParse(setMaxDegreeParallismOption.Value(), out int result))
                    App.MaxParallelism = result;

                if (setDescriptionOption.HasValue() && Enum.TryParse(setDescriptionOption.Value(), out DescriptionType resultType))
                    App.DescriptionType = resultType;

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
                        App.ExtractFileOption = ExtractFileOption.Portraits | ExtractFileOption.AbilityTalents | ExtractFileOption.MatchAwards | ExtractFileOption.Announcers | ExtractFileOption.Sprays | ExtractFileOption.VoiceLines;
                    }
                    else
                    {
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "PORTRAITS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "PORTRAIT"))
                            App.ExtractFileOption |= ExtractFileOption.Portraits;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "TALENTS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "TALENT"))
                            App.ExtractFileOption |= ExtractFileOption.Talents;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITIES") || extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITY"))
                            App.ExtractFileOption |= ExtractFileOption.Abilities;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITYTALENTS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILITYTALENT") || extractIconsOption.Values.Exists(x => x.ToUpper() == "ABILTALENT"))
                            App.ExtractFileOption |= ExtractFileOption.AbilityTalents;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "AWARDS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "MATCHAWARDS"))
                            App.ExtractFileOption |= ExtractFileOption.MatchAwards;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "ANNOUNCERS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "ANNOUNCER") || extractIconsOption.Values.Exists(x => x.ToUpper() == "ANOUNCER"))
                            App.ExtractFileOption |= ExtractFileOption.Announcers;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "SPRAYS") || extractIconsOption.Values.Exists(x => x.ToUpper() == "SPRAY"))
                            App.ExtractFileOption |= ExtractFileOption.Sprays;
                        if (extractIconsOption.Values.Exists(x => x.ToUpper() == "VOICELINES") || extractIconsOption.Values.Exists(x => x.ToUpper() == "VOICELINE") || extractIconsOption.Values.Exists(x => x.ToUpper() == "VOICE") || extractIconsOption.Values.Exists(x => x.ToUpper() == "VOICES"))
                            App.ExtractFileOption |= ExtractFileOption.VoiceLines;
                    }

                    if (App.ExtractFileOption != ExtractFileOption.None)
                        App.ExtractFileOption &= ~ExtractFileOption.None;
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
                            app.Localizations.Add(localization);
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
                    app.Localizations.Add(Localization.ENUS);
                }

                App.CreateXml = xmlOutputOption.HasValue() ? true : false;
                App.CreateJson = jsonOutputOption.HasValue() ? true : false;
                App.ShowValidationWarnings = validationWarningsOption.HasValue() ? true : false;
                App.IsFileSplit = setFileSplitOption.HasValue() ? true : false;
                App.IsLocalizedText = localizedTextOption.HasValue() ? true : false;
                App.ExcludeAwardParsing = excludeAwardParseOption.HasValue() ? true : false;
                App.CreateMinFiles = minifyOption.HasValue() ? true : false;
                app.Run();
                Console.ResetColor();

                return 0;
            });

            if (args != null && args.Length > 0)
            {
                try
                {
                    commandLineApplication.Execute(args);
                }
                catch (CommandParsingException)
                {
                    return;
                }
            }
            else // defaults
            {
                App.Defaults = true;
                app.Run();
            }

            Console.ResetColor();
        }
    }
}
