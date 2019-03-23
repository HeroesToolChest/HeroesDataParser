using Heroes.Models;
using HeroesData.Commands;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HeroesData
{
    public class Program
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly Dictionary<ExtractFileOption, List<string>> ExtractValues = new Dictionary<ExtractFileOption, List<string>>();

        public static void Main(string[] args)
        {
            App app = new App();
            app.SetCurrentCulture();

            SetExtractValues();

            CommandLineApplication commandLineApplication = new CommandLineApplication(true)
            {
                Description = "Extract Heroes of the Storm game data into XML and JSON format",
            };
            commandLineApplication.HelpOption("-?|-h|--help");
            commandLineApplication.VersionOption("-v|--version", $"Heroes Data Parser ({App.Version})");

            ReadCommand.Add(commandLineApplication).SetCommand();

            CommandOption storagePathOption = commandLineApplication.Option("-s|--storage-path <FILEPATH>", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory.", CommandOptionType.SingleValue);
            CommandOption setOutputDirectoryOption = commandLineApplication.Option("-o|--output-directory <FILEPATH>", "Sets the output directory.", CommandOptionType.SingleValue);
            CommandOption setDescriptionOption = commandLineApplication.Option("-d|--description <VALUE>", "Sets the description output type (0 - 6) - Default 0.", CommandOptionType.SingleValue);
            CommandOption extractIconsOption = commandLineApplication.Option("-e|--extract <VALUE>", $"Extracts images, available only in -s|--storage-path mode using the Hots directory.", CommandOptionType.MultipleValue);
            CommandOption setGameStringLocalizations = commandLineApplication.Option("-l|--localization <LOCALE>", "Sets the gamestring localization(s) - Default: enUS.", CommandOptionType.MultipleValue);
            CommandOption setBuildOption = commandLineApplication.Option("-b|--build <number>", "Sets the override build file.", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = commandLineApplication.Option("-t|--threads <NUMBER>", "Limits the maximum amount of threads to use.", CommandOptionType.SingleValue);

            CommandOption xmlOutputOption = commandLineApplication.Option("--xml", "Creates xml output.", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = commandLineApplication.Option("--json", "Creates json output.", CommandOptionType.NoValue);
            CommandOption setFileSplitOption = commandLineApplication.Option("-f|--file-split", "Splits the XML and JSON file(s) into multiple files.", CommandOptionType.NoValue);
            CommandOption localizedTextOption = commandLineApplication.Option("--localized-text", "Extracts localized gamestrings from the XML and JSON file(s) into a text file.", CommandOptionType.NoValue);
            CommandOption minifyOption = commandLineApplication.Option("--minify", "Creates .min file(s) along with current output file(s).", CommandOptionType.NoValue);
            CommandOption validationWarningsOption = commandLineApplication.Option("--warnings", "Displays all validation warnings.", CommandOptionType.NoValue);
            CommandOption excludeAwardParseOption = commandLineApplication.Option("--exclude-awards", "Excludes match award parsing.", CommandOptionType.NoValue);

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
                        App.ExtractFileOption = ExtractFileOption.All;
                    }
                    else
                    {
                        foreach (ExtractFileOption extractFileOption in Enum.GetValues(typeof(ExtractFileOption)))
                        {
                            if (extractFileOption == ExtractFileOption.None || extractFileOption == ExtractFileOption.All)
                                continue;

                            if (ExtractValues.TryGetValue(extractFileOption, out List<string> values))
                            {
                                if (extractIconsOption.Values.Intersect(values, StringComparer.OrdinalIgnoreCase).Any())
                                    App.ExtractFileOption |= extractFileOption;
                            }
                        }
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

        private static void SetExtractValues()
        {
            ExtractValues.Add(ExtractFileOption.Portraits, new List<string>()
            {
                "PORTRAITS", "PORTRAIT", "PORTRIAT", "PORT",
            });

            ExtractValues.Add(ExtractFileOption.Talents, new List<string>()
            {
                "TALENTS", "TALENT", "TAL",
            });

            ExtractValues.Add(ExtractFileOption.Abilities, new List<string>()
            {
                "ABILITIES", "ABILITY", "ABIL", "ABILITEIS", "ABILITES", "ABILITIS",
            });

            ExtractValues.Add(ExtractFileOption.AbilityTalents, new List<string>()
            {
                "ABILITYTALENTS", "ABILITYTALENT", "ABILTALENT", "ABILTAL",
            });

            ExtractValues.Add(ExtractFileOption.MatchAwards, new List<string>()
            {
                "AWARDS", "MATCHAWARDS", "AWARD", "MATCHAWARD", "MATWARD",
            });

            ExtractValues.Add(ExtractFileOption.Announcers, new List<string>()
            {
                "ANNOUNCERS", "ANNOUNCER", "ANOUNCER", "ANN",
            });

            ExtractValues.Add(ExtractFileOption.Sprays, new List<string>()
            {
                "SPRAYS", "SPRAY",
            });

            ExtractValues.Add(ExtractFileOption.VoiceLines, new List<string>()
            {
                "VOICELINES", "VOICELINE", "VOICE", "VOICES",
            });

            ExtractValues.Add(ExtractFileOption.Emoticons, new List<string>()
            {
                "EMOTICONS", "EMOTICON", "EMOTES", "EMOTE",
            });
        }
    }
}
