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
        private readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly Dictionary<ExtractDataOption, List<string>> ExtractDataValues = new Dictionary<ExtractDataOption, List<string>>();
        private readonly Dictionary<ExtractImageOption, List<string>> ExtractImageValues = new Dictionary<ExtractImageOption, List<string>>();

        public static void Main(string[] args)
        {
            App app = new App();
            Program program = new Program();

            app.SetCurrentCulture();

            program.SetExtractValues();

            CommandLineApplication commandLineApplication = new CommandLineApplication(true)
            {
                Description = "Extract Heroes of the Storm game data into XML and JSON format",
            };
            commandLineApplication.HelpOption("-?|-h|--help");
            commandLineApplication.VersionOption("-v|--version", $"Heroes Data Parser ({App.Version})");

            ListCommand.Add(commandLineApplication).SetCommand();
            ReadCommand.Add(commandLineApplication).SetCommand();
            ExtractCommand.Add(commandLineApplication).SetCommand();
            ImageCommand.Add(commandLineApplication).SetCommand();
            QuickCompareCommand.Add(commandLineApplication).SetCommand();
            V4ConvertCommand.Add(commandLineApplication).SetCommand();

            CommandArgument storagePathArgument = commandLineApplication.Argument("storage-path", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory.");

            CommandOption setOutputDirectoryOption = commandLineApplication.Option("-o|--output-directory <FILEPATH>", "Sets the output directory.", CommandOptionType.SingleValue);
            CommandOption setDescriptionOption = commandLineApplication.Option("-d|--description <VALUE>", "Sets the description output type (0 - 6) - Default: 0.", CommandOptionType.SingleValue);
            CommandOption extractDataFilesOption = commandLineApplication.Option("-e|--extract-data <VALUE>", $"Extracts data files - Default: herodata.", CommandOptionType.MultipleValue);
            CommandOption extractImageFilesOption = commandLineApplication.Option("-i|--extract-images <VALUE>", $"Extracts image files, only available using the Heroes of the Storm game directory.", CommandOptionType.MultipleValue);
            CommandOption setGameStringLocalizations = commandLineApplication.Option("-l|--localization <LOCALE>", "Sets the gamestring localization(s) - Default: enUS.", CommandOptionType.MultipleValue);
            CommandOption setBuildOption = commandLineApplication.Option("-b|--build <NUMBER>", "Sets the override build file(s).", CommandOptionType.SingleValue);
            CommandOption setMaxDegreeParallismOption = commandLineApplication.Option("-t|--threads <NUMBER>", "Limits the maximum amount of threads to use.", CommandOptionType.SingleValue);

            CommandOption xmlOutputOption = commandLineApplication.Option("--xml", "Creates xml output.", CommandOptionType.NoValue);
            CommandOption jsonOutputOption = commandLineApplication.Option("--json", "Creates json output.", CommandOptionType.NoValue);
            CommandOption setFileSplitOption = commandLineApplication.Option("--file-split", "Splits the XML and JSON file(s) into multiple files.", CommandOptionType.NoValue);
            CommandOption localizedTextOption = commandLineApplication.Option("--localized-text", "Extracts localized gamestrings from the XML and JSON file(s) into a text file.", CommandOptionType.NoValue);
            CommandOption minifyOption = commandLineApplication.Option("--minify", "Creates .min file(s) along with current output file(s).", CommandOptionType.NoValue);
            CommandOption validationWarningsOption = commandLineApplication.Option("--warnings", "Displays all validation warnings.", CommandOptionType.NoValue);

            commandLineApplication.OnExecute(() =>
            {
                App.Defaults = false;

                if (!string.IsNullOrEmpty(storagePathArgument.Value))
                {
                    App.StoragePath = storagePathArgument.Value;
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
                    App.OutputDirectory = Path.Combine(program.AssemblyPath, "output");

                // data file extraction
                if (extractDataFilesOption.HasValue())
                {
                    if (extractDataFilesOption.Values.Exists(x => x.ToUpper() == "ALL"))
                    {
                        App.ExtractDataOption = ExtractDataOption.All;
                    }
                    else
                    {
                        foreach (ExtractDataOption extractDataOption in Enum.GetValues(typeof(ExtractDataOption)))
                        {
                            if (extractDataOption == ExtractDataOption.None || extractDataOption == ExtractDataOption.All)
                                continue;

                            if (program.ExtractDataValues.TryGetValue(extractDataOption, out List<string> values))
                            {
                                if (extractDataFilesOption.Values.Intersect(values, StringComparer.OrdinalIgnoreCase).Any())
                                    App.ExtractDataOption |= extractDataOption;
                            }
                        }
                    }

                    // none is default as defined in App
                    if (App.ExtractDataOption != ExtractDataOption.None)
                        App.ExtractDataOption &= ~ExtractDataOption.None;
                }
                else
                {
                    App.ExtractDataOption = ExtractDataOption.HeroData;
                }

                // image file extraction
                if (extractImageFilesOption.HasValue() && !string.IsNullOrEmpty(storagePathArgument.Value))
                {
                    if (extractImageFilesOption.Values.Exists(x => x.ToUpper() == "ALL"))
                    {
                        App.ExtractFileOption = ExtractImageOption.All;
                    }
                    else if (extractImageFilesOption.Values.Exists(x => x.ToUpper() == "ALL-SPLIT" || x.ToUpper() == "ALLSPLIT"))
                    {
                        App.ExtractFileOption = ExtractImageOption.AllSplit;
                    }
                    else
                    {
                        foreach (ExtractImageOption extractFileOption in Enum.GetValues(typeof(ExtractImageOption)))
                        {
                            if (extractFileOption == ExtractImageOption.None || extractFileOption == ExtractImageOption.All)
                                continue;

                            if (program.ExtractImageValues.TryGetValue(extractFileOption, out List<string> values))
                            {
                                if (extractImageFilesOption.Values.Intersect(values, StringComparer.OrdinalIgnoreCase).Any())
                                    App.ExtractFileOption |= extractFileOption;
                            }
                        }
                    }

                    // none is default as defined in App
                    if (App.ExtractFileOption != ExtractImageOption.None)
                        App.ExtractFileOption &= ~ExtractImageOption.None;
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

                App.CreateXml = xmlOutputOption.HasValue();
                App.CreateJson = jsonOutputOption.HasValue();

                // if both not set, default to both true
                if (!xmlOutputOption.HasValue() && !jsonOutputOption.HasValue())
                {
                    App.CreateXml = true;
                    App.CreateJson = true;
                }

                App.ShowValidationWarnings = validationWarningsOption.HasValue();
                App.IsFileSplit = setFileSplitOption.HasValue();
                App.IsLocalizedText = localizedTextOption.HasValue();
                App.CreateMinFiles = minifyOption.HasValue();
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

        private void SetExtractValues()
        {
            // common
            List<string> heroData = new List<string>()
            {
                "HERODATA", "HEROES", "HEROESDATA", "HERO",
            };
            List<string> unitData = new List<string>()
            {
                "UNIT", "UNITS",
            };
            List<string> matchAwards = new List<string>()
            {
                "MATCHAWARDS", "MATCHAWARD", "AWARDS", "AWARD", "MATWARD",
            };
            List<string> sprays = new List<string>()
            {
                "SPRAYS", "SPRAY",
            };
            List<string> announcers = new List<string>()
            {
                "ANNOUNCERS", "ANNOUNCER", "ANOUNCER", "ANN",
            };
            List<string> voiceLines = new List<string>()
            {
                "VOICELINES", "VOICELINE", "VOICES", "VOICES",
            };
            List<string> emoticons = new List<string>()
            {
                "EMOTICONS", "EMOTICON", "EMOTES", "EMOTE",
            };

            // data
            ExtractDataValues.Add(ExtractDataOption.HeroData, heroData);
            ExtractDataValues.Add(ExtractDataOption.Unit, unitData);
            ExtractDataValues.Add(ExtractDataOption.MatchAward, matchAwards);
            ExtractDataValues.Add(ExtractDataOption.Spray, sprays);
            ExtractDataValues.Add(ExtractDataOption.Announcer, announcers);
            ExtractDataValues.Add(ExtractDataOption.VoiceLine, voiceLines);
            ExtractDataValues.Add(ExtractDataOption.Emoticon, emoticons);
            ExtractDataValues.Add(ExtractDataOption.HeroSkin, new List<string>()
            {
                "HEROSKINS", "HEROSKIN", "SKINS",
            });
            ExtractDataValues.Add(ExtractDataOption.Mount, new List<string>()
            {
                "MOUNTS", "MOUNTS",
            });
            ExtractDataValues.Add(ExtractDataOption.Banner, new List<string>()
            {
                "BANNERS", "BANNER",
            });
            ExtractDataValues.Add(ExtractDataOption.EmoticonPack, new List<string>()
            {
                "EMOTICONPACKS", "EMOTICONPACK", "EMOTEPACKS", "EMOTEPACK",
            });
            ExtractDataValues.Add(ExtractDataOption.Portrait, new List<string>()
            {
                "PORTRAITS", "PORTRAIT", "PORTRIAT", "PORT",
            });

            // images
            ExtractImageValues.Add(ExtractImageOption.HeroData, heroData);
            ExtractImageValues.Add(ExtractImageOption.Unit, unitData);
            ExtractImageValues.Add(ExtractImageOption.MatchAward, matchAwards);
            ExtractImageValues.Add(ExtractImageOption.Announcer, announcers);
            ExtractImageValues.Add(ExtractImageOption.Spray, sprays);
            ExtractImageValues.Add(ExtractImageOption.VoiceLine, voiceLines);
            ExtractImageValues.Add(ExtractImageOption.Emoticon, emoticons);
            ExtractImageValues.Add(ExtractImageOption.Talent, new List<string>()
            {
                "TALENTS", "TALENT", "TAL",
            });
            ExtractImageValues.Add(ExtractImageOption.Ability, new List<string>()
            {
                "ABILITIES", "ABILITY", "ABIL", "ABILITEIS", "ABILITES", "ABILITIS",
            });
            ExtractImageValues.Add(ExtractImageOption.AbilityTalent, new List<string>()
            {
                "ABILITYTALENTS", "ABILITYTALENT", "ABILTALENT", "ABILTAL",
            });
            ExtractImageValues.Add(ExtractImageOption.HeroPortrait, new List<string>()
            {
                "HEROPORTRAITS", "HEROPORTRAIT", "HP",
            });
            ExtractImageValues.Add(ExtractImageOption.HeroDataSplit, new List<string>()
            {
                "HERODATASPLIT", "HEROESSPLIT", "HEROESDATASPLIT", "HEROSPLIT", "HERODATA-SPLIT", "HEROES-SPLIT", "HEROESDATA-SPLIT", "HERO-SPLIT",
            });
        }
    }
}
