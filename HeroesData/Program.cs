using Heroes.Models;
using HeroesData.Commands;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData
{
    public class Program
    {
        private readonly Dictionary<ExtractDataOptions, List<string>> _extractDataValues = new Dictionary<ExtractDataOptions, List<string>>();
        private readonly Dictionary<ExtractImageOptions, List<string>> _extractImageValues = new Dictionary<ExtractImageOptions, List<string>>();

        public static void Main(string[] args)
        {
            App app = new App();
            Program program = new Program();

            App.SetCurrentCulture();

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
            LocalizedTextToJsonCommand.Add(commandLineApplication).SetCommand();
            PortraitInfoCommand.Add(commandLineApplication).SetCommand();
            PortraitCacheCommand.Add(commandLineApplication).SetCommand();
            PortraitExtractCommand.Add(commandLineApplication).SetCommand();
            PortraitAutoExtractCommand.Add(commandLineApplication).SetCommand();

            CommandArgument storagePathArgument = commandLineApplication.Argument("storage-directory", "The 'Heroes of the Storm' directory or an already extracted 'mods' directory.");

            CommandOption setOutputDirectoryOption = commandLineApplication.Option("-o|--output-directory <DIRECTORYPATH>", "Sets the output directory.", CommandOptionType.SingleValue);
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

                if (extractImageFilesOption.HasValue() && !extractDataFilesOption.HasValue())
                {
                    return InvalidCommand("You need to set the -e|--extract-data option");
                }

                if (!string.IsNullOrWhiteSpace(storagePathArgument.Value))
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
                    App.OutputDirectory = Path.Combine(App.AssemblyPath, "output");

                // data file extraction
                if (extractDataFilesOption.HasValue())
                {
                    if (extractDataFilesOption.Values.Exists(x => x.Equals("ALL", StringComparison.OrdinalIgnoreCase)))
                    {
                        App.ExtractDataOption = ExtractDataOptions.All;
                    }
                    else
                    {
                        foreach (ExtractDataOptions? extractDataOption in Enum.GetValues(typeof(ExtractDataOptions)))
                        {
                            if (!extractDataOption.HasValue || extractDataOption == ExtractDataOptions.None || extractDataOption == ExtractDataOptions.All)
                                continue;

                            if (program._extractDataValues.TryGetValue(extractDataOption.Value, out List<string>? values))
                            {
                                if (extractDataFilesOption.Values.Intersect(values, StringComparer.OrdinalIgnoreCase).Any())
                                    App.ExtractDataOption |= extractDataOption.Value;
                            }
                        }
                    }

                    // none is default as defined in App
                    if (App.ExtractDataOption != ExtractDataOptions.None)
                        App.ExtractDataOption &= ~ExtractDataOptions.None;
                }
                else
                {
                    App.ExtractDataOption = ExtractDataOptions.HeroData;
                }

                // image file extraction
                if (extractImageFilesOption.HasValue() && !string.IsNullOrEmpty(storagePathArgument.Value))
                {
                    if (extractImageFilesOption.Values.Exists(x => x.Equals("ALL", StringComparison.OrdinalIgnoreCase)))
                    {
                        App.ExtractFileOption = ExtractImageOptions.All;
                    }
                    else if (extractImageFilesOption.Values.Exists(x => x.Equals("ALL-SPLIT", StringComparison.OrdinalIgnoreCase) || x.Equals("ALLSPLIT", StringComparison.OrdinalIgnoreCase)))
                    {
                        App.ExtractFileOption = ExtractImageOptions.AllSplit;
                    }
                    else
                    {
                        foreach (ExtractImageOptions? extractFileOption in Enum.GetValues(typeof(ExtractImageOptions)))
                        {
                            if (!extractFileOption.HasValue || extractFileOption == ExtractImageOptions.None || extractFileOption == ExtractImageOptions.All)
                                continue;

                            if (program._extractImageValues.TryGetValue(extractFileOption.Value, out List<string>? values))
                            {
                                if (extractImageFilesOption.Values.Intersect(values, StringComparer.OrdinalIgnoreCase).Any())
                                    App.ExtractFileOption |= extractFileOption.Value;
                            }
                        }
                    }

                    // none is default as defined in App
                    if (App.ExtractFileOption != ExtractImageOptions.None)
                        App.ExtractFileOption &= ~ExtractImageOptions.None;
                }

                if (setGameStringLocalizations.HasValue())
                {
                    IEnumerable<string> localizations = new List<string>();

                    if (setGameStringLocalizations.Values.Exists(x => x.Equals("ALL", StringComparison.OrdinalIgnoreCase)))
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

        private static int InvalidCommand(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            return 1;
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
            List<string> bundles = new List<string>()
            {
                "BUNDLES", "BUNDLE",
            };

            // data
            _extractDataValues.Add(ExtractDataOptions.HeroData, heroData);
            _extractDataValues.Add(ExtractDataOptions.Unit, unitData);
            _extractDataValues.Add(ExtractDataOptions.MatchAward, matchAwards);
            _extractDataValues.Add(ExtractDataOptions.Spray, sprays);
            _extractDataValues.Add(ExtractDataOptions.Announcer, announcers);
            _extractDataValues.Add(ExtractDataOptions.VoiceLine, voiceLines);
            _extractDataValues.Add(ExtractDataOptions.Emoticon, emoticons);
            _extractDataValues.Add(ExtractDataOptions.Bundle, bundles);
            _extractDataValues.Add(ExtractDataOptions.Boost, new List<string>()
            {
                "BOOST", "BOOSTS",
            });
            _extractDataValues.Add(ExtractDataOptions.LootChest, new List<string>()
            {
                "LOOTCHEST", "LOOTCHESTS", "LOOT", "LOOTS", "BOOTY",
            });
            _extractDataValues.Add(ExtractDataOptions.PortraitPack, new List<string>()
            {
                "PORTRAITPACKS", "PORTRAITPACK", "PORTRIATPACKS", "PORTRIATPACK", "PORTPACKS", "PORTPACK",
            });
            _extractDataValues.Add(ExtractDataOptions.RewardPortrait, new List<string>()
            {
                "REWARDPORTRAITS", "REWARDPORTRAIT", "REWARDPORTRIATS", "REWARDPORTRIAT", "REWARDPORTS", "REWARDPORT",
            });
            _extractDataValues.Add(ExtractDataOptions.HeroSkin, new List<string>()
            {
                "HEROSKINS", "HEROSKIN", "SKINS", "SKIN",
            });
            _extractDataValues.Add(ExtractDataOptions.Mount, new List<string>()
            {
                "MOUNTS", "MOUNT",
            });
            _extractDataValues.Add(ExtractDataOptions.Banner, new List<string>()
            {
                "BANNERS", "BANNER",
            });
            _extractDataValues.Add(ExtractDataOptions.EmoticonPack, new List<string>()
            {
                "EMOTICONPACKS", "EMOTICONPACK", "EMOTEPACKS", "EMOTEPACK",
            });
            _extractDataValues.Add(ExtractDataOptions.Veterancy, new List<string>()
            {
                "VETERANCY", "VET", "VETERENCY", "SCALE", "SCALING", "SCALES",
            });

            // images
            _extractImageValues.Add(ExtractImageOptions.HeroData, heroData);
            _extractImageValues.Add(ExtractImageOptions.Unit, unitData);
            _extractImageValues.Add(ExtractImageOptions.MatchAward, matchAwards);
            _extractImageValues.Add(ExtractImageOptions.Announcer, announcers);
            _extractImageValues.Add(ExtractImageOptions.Spray, sprays);
            _extractImageValues.Add(ExtractImageOptions.VoiceLine, voiceLines);
            _extractImageValues.Add(ExtractImageOptions.Emoticon, emoticons);
            _extractImageValues.Add(ExtractImageOptions.Bundle, bundles);
            _extractImageValues.Add(ExtractImageOptions.Talent, new List<string>()
            {
                "TALENTS", "TALENT", "TAL",
            });
            _extractImageValues.Add(ExtractImageOptions.Ability, new List<string>()
            {
                "ABILITIES", "ABILITY", "ABIL", "ABILITEIS", "ABILITES", "ABILITIS",
            });
            _extractImageValues.Add(ExtractImageOptions.AbilityTalent, new List<string>()
            {
                "ABILITYTALENTS", "ABILITYTALENT", "ABILTALENT", "ABILTAL",
            });
            _extractImageValues.Add(ExtractImageOptions.HeroPortrait, new List<string>()
            {
                "HEROPORTRAITS", "HEROPORTRAIT", "HP",
            });
            _extractImageValues.Add(ExtractImageOptions.HeroDataSplit, new List<string>()
            {
                "HERODATASPLIT", "HEROESSPLIT", "HEROESDATASPLIT", "HEROSPLIT", "HERODATA-SPLIT", "HEROES-SPLIT", "HEROESDATA-SPLIT", "HERO-SPLIT",
            });
        }
    }
}
