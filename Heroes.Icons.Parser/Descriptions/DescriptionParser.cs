using Heroes.Icons.Parser.HeroData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.Descriptions
{
    public class DescriptionParser
    {
        private HeroDataLoader HeroDataLoader;
        private DescriptionLoader DescriptionLoader;
        private ScalingDataLoader ScalingDataLoader;

        private DataTable DataTable = new DataTable();

        private Dictionary<string, HashSet<string>> ElementNames = new Dictionary<string, HashSet<string>>();

        public DescriptionParser(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, ScalingDataLoader scalingDataLoader)
        {
            HeroDataLoader = heroDataLoader;
            DescriptionLoader = descriptionLoader;
            ScalingDataLoader = scalingDataLoader;

            SetElementNames();
        }

        /// <summary>
        /// The full tooltip raw description of ability/talent
        /// </summary>
        public ConcurrentDictionary<string, string> FullParsedDescriptions { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Short tooltip raw description of ability/talent
        /// </summary>
        public ConcurrentDictionary<string, string> ShortParsedDescriptions { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Hero raw description found on hero select screen
        /// </summary>
        public ConcurrentDictionary<string, string> HeroParsedDescriptions { get; set; } = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> InvalidFullDescriptions { get; set; } = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> InvalidShortDescriptions { get; set; } = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, string> InvalidHeroDescriptions { get; set; } = new ConcurrentDictionary<string, string>();

        public void Parse()
        {
            ParseDescriptionsList(DescriptionLoader.FullDescriptions, FullParsedDescriptions, InvalidFullDescriptions);
            ParseDescriptionsList(DescriptionLoader.ShortDescriptions, ShortParsedDescriptions, InvalidShortDescriptions);
            ParseDescriptionsList(DescriptionLoader.HeroDescriptions, HeroParsedDescriptions, InvalidHeroDescriptions);
        }

        private void ParseDescriptionsList(SortedDictionary<string, string> descriptions, ConcurrentDictionary<string, string> parsedDescriptions, ConcurrentDictionary<string, string> invalidDescriptions)
        {
            if (descriptions == null || descriptions.Count < 1)
                return;

            Parallel.ForEach(descriptions, description =>
            {
                string desc = description.Value;
                string name = description.Key;

                try
                {
                    if (desc.Contains("$GalaxyVar") || desc.Contains("**TEMP**"))
                    {
                        return;
                    }

                    if (desc.Contains("<d score="))
                        desc = Regex.Replace(desc, @"<d score=\w+/>", "0");

                    if (desc.Contains("<D ref="))
                        desc = desc.Replace("<D ref=", "<d ref=");

                    if (desc.Contains("<d ref= \""))
                        desc = desc.Replace("<d ref= \"", "<d ref=\"");

                    if (desc.Contains("<d ref="))
                    {
                        while (desc.Contains("[d ref="))
                        {
                            ParseDescription(name, ref desc, "(\\[d ref=\".*?/\\])", true, parsedDescriptions, invalidDescriptions);
                        }

                        ParseDescription(name, ref desc, "(<d ref=\".*?/>)", false, parsedDescriptions, invalidDescriptions);
                    }
                    else // no values to look up
                    {
                        parsedDescriptions.GetOrAdd(name, DescriptionValidation.Validate(desc));
                    }
                }
                catch (UnparseableException)
                {
                    invalidDescriptions.GetOrAdd(name, desc);
                }
                catch (Exception)
                {
                    invalidDescriptions.GetOrAdd(name, desc);
                }
            });
        }

        private void ParseDescription(string name, ref string description, string spliter, bool innerRef, ConcurrentDictionary<string, string> parsedDescriptions, ConcurrentDictionary<string, string> invalidDescriptions)
        {
            if (innerRef)
            {
                description = description.Replace("'", "\"");
            }

            string[] parts = Regex.Split(description, spliter);

            for (int i = 0; i < parts.Length; i++)
            {
                if (innerRef)
                {
                    if (!parts[i].Contains("[d ref="))
                        continue;
                }
                else
                {
                    if (!parts[i].Contains("<d ref="))
                        continue;
                }

                int? precision = null;

                string pathLookup = parts[i];

                if (pathLookup.ToLower().Contains("precision=\""))
                {
                    pathLookup = pathLookup.Replace("Precision=\"", "precision=\"");
                    pathLookup = GetPrecision(pathLookup, out precision);
                }

                if (pathLookup.ToLower().Contains("player=\"") || pathLookup.ToLower().Contains("player =\""))
                {
                    if (pathLookup.ToLower().Contains("player =\""))
                        pathLookup = pathLookup.Replace("player =\"", "player=\"");
                    else
                        pathLookup = pathLookup.Replace("Player=\"", "player=\"");

                    pathLookup = GetPlayer(pathLookup, out int? player);
                }

                pathLookup = pathLookup.Substring(8); // get path without the d ref
                pathLookup = pathLookup.Remove(pathLookup.LastIndexOf('/') - 1).TrimEnd(' ').TrimEnd('"'); // removes ending />

                // check for math operators
                string[] arithmeticPaths = pathLookup.Split(new char[] { '/', '*', '+', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < arithmeticPaths.Length; j++)
                {
                    if (!double.TryParse(arithmeticPaths[j].Trim(), out double constant))
                    {
                        if (!string.IsNullOrEmpty(arithmeticPaths[j].Trim()))
                        {
                            int position = pathLookup.IndexOf(arithmeticPaths[j]);
                            if (position >= 0)
                            {
                                string value = GetValueFromPath(arithmeticPaths[j], out double? scalingValue);

                                if (scalingValue.HasValue)
                                    pathLookup = $"{pathLookup.Substring(0, position)}{value}~~{scalingValue}~~{pathLookup.Substring(position + arithmeticPaths[j].Length)}";
                                else
                                    pathLookup = pathLookup.Substring(0, position) + value + pathLookup.Substring(position + arithmeticPaths[j].Length);
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(Regex.Replace(pathLookup, @"[/*+\-()]", string.Empty)))
                {
                    invalidDescriptions.GetOrAdd(name, description);
                    description = string.Empty;
                    return;
                }

                // extract the scaling
                string scalingText = GetScalingText(pathLookup);

                if (!string.IsNullOrEmpty(scalingText))
                {
                    pathLookup = pathLookup.Replace(scalingText, string.Empty);
                }

                double number = MathEval.CalculatePathEquation(pathLookup);

                if (precision.HasValue)
                    parts[i] = Math.Round(number, precision.Value).ToString();
                else
                    parts[i] = Math.Round(number, 0).ToString();

                if (!string.IsNullOrEmpty(scalingText))
                {
                    parts[i] += $"{scalingText}";
                }
            }

            if (innerRef)
            {
                description = string.Join(string.Empty, parts);
            }
            else
            {
                string joinDesc = string.Join(string.Empty, parts);
                Regex regex = new Regex(@"[a-z]+""[a-z]+");

                foreach (Match match in regex.Matches(joinDesc))
                {
                    joinDesc = joinDesc.Replace(match.Value, match.Value.Replace("\"", "'"));
                }

                if (!parsedDescriptions.ContainsKey(name))
                    parsedDescriptions.GetOrAdd(name, DescriptionValidation.Validate(joinDesc));
            }
        }

        private string GetPrecision(string pathPart, out int? precision)
        {
            // get the precision string first
            Regex regex = new Regex("precision=\".*?\"");
            MatchCollection precisionMatches = regex.Matches(pathPart);

            if (precisionMatches.Count != 1)
                throw new Exception("GetPrecision: invalid matches found");

            // now get the value
            regex = new Regex("\".*?\"");
            MatchCollection match = regex.Matches(precisionMatches[0].ToString());

            precision = int.Parse(match[0].ToString().Trim('"'));

            // remove precision text
            pathPart = pathPart.Replace(precisionMatches[0].ToString(), string.Empty);
            return pathPart;
        }

        private string GetPlayer(string pathPart, out int? player)
        {
            // get the player string first
            Regex regex = new Regex("player=\".*?\"");
            MatchCollection playerMatches = regex.Matches(pathPart);

            if (playerMatches.Count != 1)
                throw new Exception("GetPlayer: invalid matches found");

            // now get the value
            regex = new Regex("\".*?\"");
            MatchCollection match = regex.Matches(playerMatches[0].ToString());

            player = int.Parse(match[0].ToString().Trim('"'));

            // remove player text
            pathPart = pathPart.Replace(playerMatches[0].ToString(), string.Empty);
            return pathPart;
        }

        private string GetScalingText(string pathLookup)
        {
            // get scaling string
            Regex regex = new Regex("~~.*?~~");
            MatchCollection playerMatches = regex.Matches(pathLookup);

            if (playerMatches.Count < 1)
                return string.Empty;

            return playerMatches[0].ToString();
        }

        private string GetValueFromPath(string pathLookup, out double? scaling)
        {
            var parts = pathLookup.Trim().Split(new char[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (parts[0].Contains("$BehaviorTokenCount") || parts[0].Contains("$BehaviorStackCount"))
            {
                scaling = null;
                return "0";
            }

            return ReadData(parts, out scaling);
        }

        private string ReadData(List<string> parts, out double? scaling)
        {
            scaling = null;
            string value = ReadXDocumentData(parts, null);

            if (parts.Count == 3)
                scaling = GetScalingInfo(parts[0], parts[1], parts[2]);

            // caster life percents, CValidatorUnitCompareVital
            if (string.IsNullOrEmpty(value) && parts[1] == "CasterLifeGT75Percent")
                return "0.75";
            else if (string.IsNullOrEmpty(value) && parts[1] == "CasterLifeGT50Percent")
                return "0.50";
            else if (string.IsNullOrEmpty(value) && parts[1] == "CasterLifeGT25Percent")
                return "0.25";
            else if (string.IsNullOrEmpty(value) && parts[1] == "CasterLifeLT75Percent")
                return "0.75";
            else if (string.IsNullOrEmpty(value) && parts[1] == "CasterLifeLT50Percent")
                return "0.50";
            else if (string.IsNullOrEmpty(value) && parts[1] == "CasterLifeLT25Percent")
                return "0.25";
            else if (string.IsNullOrEmpty(value) && parts[1] == "CasterNotInCombat4")
                return "4";

            if (string.IsNullOrEmpty(value) && (parts.Last() == "AttributeFactor[Heroic]" || parts.Last() == "AttributeFactor[Structure]"))
                return "0";
            else if (string.IsNullOrEmpty(value) && (parts.Last() == "ModifyFraction" || parts.Last() == "Ratio"))
                return "1.00";
            else if (string.IsNullOrEmpty(value) && (parts.Last() == "Count" || parts.Last() == "MaxStackCount" || parts.Last() == "SpawnCount" || parts.Last() == "Scale"))
                return "1";
            else if (string.IsNullOrEmpty(value) && parts[1] == "KelThuzadMasterOfTheColdDarkModifyToken")
                return "1";
            else if (string.IsNullOrEmpty(value) && parts[1] == "TyraelElDruinsMightStalwartAngelTalentActiveBuff") // tyrael talent, unknown location
                return "20";
            else if (string.IsNullOrEmpty(value) && parts[1] == "TyraelElDruinsMightStalwartAngelTalentProcBuff") // tyrael talent, unknown location
                return "3";
            else if (string.IsNullOrEmpty(value) && parts[1] == "AlexstraszaAbundanceTimedAccumulator" && parts[2] == "MultiplierPerStep") // alextrasza ability, unknown location
                return "-0.333333333333"; // unknown value
            else if (string.IsNullOrEmpty(value) && parts[1] == "AlexstraszaAbundanceTimedAccumulator" && parts[2] == "MaxStepCount") // alextrasza ability, unknown location
                return "1"; // unknown value
            return value;
        }

        private string ReadXDocumentData(List<string> parts, XAttribute parent)
        {
            XElement currentElement = null;
            parent = null;

            if (!ElementNames.ContainsKey(parts[0]))
                return string.Empty;

            var validElementNames = ElementNames[parts[0]];
            if (validElementNames.Count < 1)
                return string.Empty;

            IEnumerable<XElement> elements = HeroDataLoader.XmlData.Descendants(validElementNames.First()).Where(x => x.Attribute("id")?.Value == parts[1]);
            for (int i = 1; i < validElementNames.Count(); i++)
            {
                elements = elements.Concat(HeroDataLoader.XmlData.Descendants(validElementNames.ElementAt(i)).Where(x => x.Attribute("id")?.Value == parts[1]));
            }

            elements = elements.Reverse();

            foreach (var element in elements)
            {
                currentElement = element;
                parent = currentElement.Attribute("parent");

                // loop through path look up
                for (int i = 2; i < parts.Count; i++)
                {
                    if (parts[i].Contains("]")) // attribute index
                    {
                        string[] elementIndexParts = parts[i].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                        var indexElements = currentElement.Descendants(elementIndexParts[0]).ToList();

                        for (int j = 0; j < indexElements.Count; j++)
                        {
                            XAttribute index = indexElements[j].Attribute("index");
                            if (index == null)
                                index = indexElements[j].Attribute("Index");

                            if ((index != null && index.Value == elementIndexParts[1]) || (index != null && int.TryParse(elementIndexParts[1], out int arrayIndex) && indexElements.Count == 1))
                            {
                                currentElement = indexElements[j];
                                break;
                            }
                            else if (index == null && int.TryParse(elementIndexParts[1], out arrayIndex)) // numerical array index
                            {
                                string tempAtt = parts.Last();
                                if (tempAtt.Contains("["))
                                    tempAtt = parts.Last().Remove(parts.Last().IndexOf('['));

                                XAttribute value = indexElements[j].Attribute(tempAtt);
                                if (value != null && j == arrayIndex)
                                {
                                    return value.Value;
                                }
                                else if (j == arrayIndex)
                                {
                                    currentElement = indexElements[j];
                                    break;
                                }
                            }
                        }
                    }
                    else if (currentElement.Attributes().Any(x => x.Name == parts[i]))
                    {
                        return currentElement.Attribute(parts[i]).Value;
                    }
                    else
                    {
                        currentElement = currentElement.Descendants(parts[i]).FirstOrDefault();
                    }

                    if (currentElement == null)
                        break;
                }

                if (currentElement == null && parent != null)
                {
                    parts[1] = parent.Value;

                    // look up the parent in the same file
                    string value = ReadXDocumentData(parts, parent);
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }

                if (currentElement != null)
                {
                    XAttribute attribute = currentElement.Attribute("value");
                    if (attribute == null)
                        attribute = currentElement.Attribute("Value");
                    if (attribute != null)
                        return attribute.Value;
                }
            }

            return string.Empty;
        }

        private double? GetScalingInfo(string catalogValue, string entryValue, string fieldValue)
        {
            if (fieldValue.EndsWith("]"))
                fieldValue = fieldValue.Substring(0, fieldValue.Length - 3);

            if (ScalingDataLoader.ScaleValueByLookupId.TryGetValue($"{catalogValue}#{entryValue}#{fieldValue}", out double scaleValue))
                return scaleValue;
            else
                return null;
        }

        private void SetElementNames()
        {
            ElementNames.Add("Behavior", new HashSet<string> { "CBehaviorBuff", "CBehaviorTokenCounter", "CBehaviorUnitTracker" });
            ElementNames.Add("Abil", new HashSet<string> { "CAbilEffectTarget", "CAbilEffectInstant", "CAbilAugment" });
            ElementNames.Add("Effect", new HashSet<string> { "CEffectDamage", "CEffectEnumArea", "CEffectCreateHealer", "CEffectModifyUnit", "CEffectCreatePersistent", "CEffectModifyPlayer",
                "CEffectModifyTokenCount", "CEffectApplyBehavior", "CEffectModifyBehaviorBuffDuration", "CEffectModifyCatalogNumeric", "CEffectLaunchMissile", "CEffectCreateUnit", "CEffectRemoveBehavior", });
            ElementNames.Add("Unit", new HashSet<string> { "CUnit" });
            ElementNames.Add("Talent", new HashSet<string> { "CTalent" });
            ElementNames.Add("Validator", new HashSet<string> { "CValidatorUnitCompareVital", "CValidatorLocationEnumArea", "CValidatorUnitCompareTokenCount", "CValidatorCompareTrackedUnitsCount" });
            ElementNames.Add("Accumulator", new HashSet<string> { "CAccumulatorToken", "CAccumulatorVitals", "CBehaviorTokenCounter", "CAccumulatorTimed", "CAccumulatorDistanceUnitTraveled" });
            ElementNames.Add("Weapon", new HashSet<string> { "CWeaponLegacy" });
            ElementNames.Add("Actor", new HashSet<string> { "CActorQuad" });
            ElementNames.Add("Armor", new HashSet<string> { "CArmor" });
        }
    }
}
