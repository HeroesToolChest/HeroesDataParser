using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HeroesData.Parser.GameStrings
{
    public class GameStringParser
    {
        private readonly int? HotsBuild;
        private readonly GameData GameData;
        private readonly GameStringValues GameStringValues;

        private readonly DataTable DataTable = new DataTable();

        private Dictionary<string, HashSet<string>> ElementNames = new Dictionary<string, HashSet<string>>();

        public GameStringParser(GameData gameData)
        {
            GameData = gameData;
            GameStringValues = GameStringValues.Load();
            SetValidElementNames();
        }

        public GameStringParser(GameData gameData, int? hotsBuild)
        {
            GameData = gameData;
            HotsBuild = hotsBuild;
            GameStringValues = GameStringValues.Load(hotsBuild);
            SetValidElementNames();
        }

        /// <summary>
        /// Gets a parsed tooltip from a given tooltip.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="tooltip">The tooltip (value) to be parsed.</param>
        /// <param name="parsedTooltip">The parsed tooltip.</param>
        /// <returns></returns>
        public bool TryParseRawTooltip(string key, string tooltip, out string parsedTooltip)
        {
            parsedTooltip = string.Empty;
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(tooltip))
                return false;

            try
            {
                parsedTooltip = ParseTooltipGameStringData(key, tooltip);
                return true;
            }
            catch (UnparseableException)
            {
                parsedTooltip = null;
                return false;
            }
            catch (Exception)
            {
                parsedTooltip = null;
                return false;
            }
        }

        /// <summary>
        /// Parse a dref string and return the calculated value.
        /// </summary>
        /// <param name="dRef">The dref string.</param>
        /// <returns></returns>
        public double? ParseDRefString(string dRef)
        {
            return ParseDataReferenceString(dRef);
        }

        private string ParseTooltipGameStringData(string referenceNameId, string tooltip)
        {
            if (tooltip.Contains("$GalaxyVar") || tooltip.Contains("**TEMP**"))
            {
                return string.Empty;
            }

            if (tooltip.Contains("<d score="))
                tooltip = Regex.Replace(tooltip, @"<d score=.*?/>", "0");

            if (tooltip.Contains("<D ref="))
                tooltip = tooltip.Replace("<D ref=", "<d ref=");

            if (tooltip.Contains("<d ref= \""))
                tooltip = tooltip.Replace("<d ref= \"", "<d ref=\"");

            if (tooltip.Contains("<d ref="))
            {
                while (tooltip.Contains("[d ref="))
                {
                    tooltip = ParseGameString(referenceNameId, tooltip, "(\\[d ref=\".*?/\\])", true);
                }

                return ParseGameString(referenceNameId, tooltip, "(<d ref=\".*?/>)", false);
            }
            else // no values to look up
            {
                return tooltip;
            }
        }

        private string ParseGameString(string referenceNameId, string tooltip, string spliter, bool nestedTooltip)
        {
            if (nestedTooltip)
            {
                tooltip = tooltip.Replace("'", "\"");
            }

            string[] parts = Regex.Split(tooltip, spliter);

            for (int i = 0; i < parts.Length; i++)
            {
                if (nestedTooltip)
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

                // get precision
                if (pathLookup.ToLower().Contains("precision=\""))
                {
                    pathLookup = pathLookup.Replace("Precision=\"", "precision=\"");
                    pathLookup = GetPrecision(pathLookup, out precision);
                }

                // remove the player
                if (pathLookup.ToLower().Contains("player=\"") || pathLookup.ToLower().Contains("player =\""))
                {
                    if (pathLookup.ToLower().Contains("player =\""))
                        pathLookup = pathLookup.Replace("player =\"", "player=\"");
                    else
                        pathLookup = pathLookup.Replace("Player=\"", "player=\"");

                    pathLookup = GetPlayer(pathLookup, out int? player);
                }

                // perform xml data lookup to find values
                string mathPath = ParseValues(pathLookup);

                if (string.IsNullOrEmpty(Regex.Replace(mathPath, @"[/*+\-()]", string.Empty)))
                {
                    return string.Empty;
                }

                // extract the scaling
                string scalingText = GetScalingText(mathPath);

                if (!string.IsNullOrEmpty(scalingText))
                {
                    mathPath = mathPath.Replace(scalingText, string.Empty);
                }

                double number = MathEval.CalculatePathEquation(mathPath.Trim('/'));

                if (precision.HasValue)
                    parts[i] = Math.Round(number, precision.Value).ToString();
                else
                    parts[i] = Math.Round(number, 0).ToString();

                if (!string.IsNullOrEmpty(scalingText))
                {
                    // only add the scaling text if the next part does not start with a % sign or is time based
                    if (i + 1 < parts.Length)
                    {
                        string nextPart = parts[i + 1].Trim();
                        if (nextPart.StartsWith("</c>"))
                            nextPart = nextPart.Substring("</c>".Length).TrimStart();
                        if (nextPart.StartsWith("<n/>"))
                            nextPart = nextPart.Substring("<n/>".Length).TrimStart();

                        if (!(nextPart.StartsWith("%") || nextPart.StartsWith("0%") || CheckForTimeBasedScaling(pathLookup)))
                            parts[i] += $"{scalingText}";
                    }
                    else
                    {
                        parts[i] += $"{scalingText}";
                    }
                }
            }

            if (nestedTooltip)
            {
                return string.Join(string.Empty, parts);
            }
            else
            {
                string joinDesc = string.Join(string.Empty, parts);
                Regex regex = new Regex(@"[a-z]+""[a-z]+");

                foreach (Match match in regex.Matches(joinDesc))
                {
                    joinDesc = joinDesc.Replace(match.Value, match.Value.Replace("\"", "'"));
                }

                return DescriptionValidator.Validate(joinDesc);
            }
        }

        private double? ParseDataReferenceString(string dRef)
        {
            int? precision = null;

            if (dRef.ToLower().Contains("precision=\""))
            {
                dRef = GetPrecision(dRef, out precision);
            }

            dRef = ParseValues(dRef);

            if (string.IsNullOrEmpty(Regex.Replace(dRef, @"[/*+\-()]", string.Empty)))
            {
                return null;
            }

            // extract the scaling
            string scalingText = GetScalingText(dRef);

            if (!string.IsNullOrEmpty(scalingText))
            {
                dRef = dRef.Replace(scalingText, string.Empty);
            }

            double number = MathEval.CalculatePathEquation(dRef);

            if (precision.HasValue)
                return Math.Round(number, precision.Value);
            else
                return Math.Round(number, 0);
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

        private string ParseValues(string pathLookup)
        {
            string[] arithmeticPaths = GetArithmeticPaths(ref pathLookup);

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

            return pathLookup;
        }

        private string[] GetArithmeticPaths(ref string pathLookup)
        {
            pathLookup = pathLookup.Substring(8); // get path without the d ref
            pathLookup = pathLookup.Remove(pathLookup.LastIndexOf('/') - 1).TrimEnd(' ').TrimEnd('"'); // removes ending />

            // check for math operators
            return pathLookup.Split(new char[] { '/', '*', '+', '-', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
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
            var scalingParts = pathLookup.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (parts[0].Contains("$BehaviorTokenCount") || parts[0].Contains("$BehaviorStackCount"))
            {
                scaling = null;
                return "0";
            }

            return ReadData(parts, scalingParts, out scaling);
        }

        private bool CheckForTimeBasedScaling(string pathLookup)
        {
            string[] arithmeticPaths = GetArithmeticPaths(ref pathLookup);

            for (int i = 0; i + 2 <= arithmeticPaths.Length; i++)
            {
                if (arithmeticPaths.Length >= i + 2 && arithmeticPaths[i].TrimEnd().EndsWith("LifeMax") && arithmeticPaths[i + 1].TrimEnd().EndsWith("LifeRegenRate"))
                    return true;
            }

            return false;
        }

        private string ReadData(List<string> parts, List<string> scalingParts, out double? scaling)
        {
            scaling = null;
            string value = ReadGameData(parts, null);

            if (scalingParts.Count == 3)
                scaling = GetScalingInfo(scalingParts[0], scalingParts[1], scalingParts[2]);

            if (string.IsNullOrEmpty(value))
            {
                // check each part value to see if we can find one
                foreach (var (name, partIndex, partValue) in GameStringValues.PartValueByPartName)
                {
                    if (partIndex == "last" && parts.Last() == name)
                    {
                        return partValue;
                    }
                    else if (int.TryParse(partIndex, out int partIndexInt))
                    {
                        if (parts.Count > partIndexInt && parts[partIndexInt] == name)
                            return partValue;
                    }
                }
            }

            return value;
        }

        private string ReadGameData(List<string> parts, XAttribute parent)
        {
            XElement currentElement = null;
            parent = null;

            if (!ElementNames.ContainsKey(parts[0]))
                return string.Empty;

            var validElementNames = ElementNames[parts[0]];
            if (validElementNames.Count < 1)
                return string.Empty;

            IEnumerable<XElement> elements = GameData.XmlGameData.Root.Elements(validElementNames.First()).Where(x => x.Attribute("id")?.Value == parts[1]);
            for (int i = 1; i < validElementNames.Count(); i++)
            {
                elements = elements.Concat(GameData.XmlGameData.Root.Elements(validElementNames.ElementAt(i)).Where(x => x.Attribute("id")?.Value == parts[1]));
            }

            elements = elements.Reverse();

            foreach (XElement element in elements)
            {
                currentElement = element;
                parent = currentElement.Attribute("parent");

                // loop through path look up
                for (int i = 2; i < parts.Count; i++)
                {
                    if (parts[i].Contains("]")) // attribute index
                    {
                        string[] elementIndexParts = parts[i].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                        var indexElements = currentElement.Elements(elementIndexParts[0]).ToList();

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
                        currentElement = currentElement.Elements(parts[i]).FirstOrDefault();
                    }

                    if (currentElement == null)
                        break;
                }

                if (currentElement == null && parent != null)
                {
                    parts[1] = parent.Value;

                    // look up the parent in the same file
                    string value = ReadGameData(parts, parent);
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
            double? scaleValue = null;

            // try lookup without indexing first
            if (fieldValue.Contains("]"))
            {
                scaleValue = GameData.GetScaleValue((catalogValue, entryValue, Regex.Replace(fieldValue, @"\[.*?\]", string.Empty)));
            }

            if (!scaleValue.HasValue)
                scaleValue = GameData.GetScaleValue((catalogValue, entryValue, fieldValue));

            return scaleValue;
        }

        private void SetValidElementNames()
        {
            ElementNames.Add("Behavior", new HashSet<string> { "CBehaviorBuff", "CBehaviorTokenCounter", "CBehaviorUnitTracker" });
            ElementNames.Add("Abil", new HashSet<string> { "CAbilEffectTarget", "CAbilEffectInstant", "CAbilAugment", "CAbilBehavior" });
            ElementNames.Add("Effect", new HashSet<string> { "CEffectDamage", "CEffectEnumArea", "CEffectCreateHealer", "CEffectModifyUnit", "CEffectCreatePersistent", "CEffectModifyPlayer",
                "CEffectModifyTokenCount", "CEffectApplyBehavior", "CEffectModifyBehaviorBuffDuration", "CEffectModifyCatalogNumeric", "CEffectLaunchMissile", "CEffectCreateUnit", "CEffectRemoveBehavior", });
            ElementNames.Add("Unit", new HashSet<string> { "CUnit" });
            ElementNames.Add("Talent", new HashSet<string> { "CTalent" });
            ElementNames.Add("Validator", new HashSet<string> { "CValidatorUnitCompareVital", "CValidatorLocationEnumArea", "CValidatorUnitCompareTokenCount", "CValidatorCompareTrackedUnitsCount", "CValidatorUnitCompareDamageTakenTime",
                "CValidatorUnitCompareBehaviorCount", "CValidatorUnitCompareBehaviorDuration", });
            ElementNames.Add("Accumulator", new HashSet<string> { "CAccumulatorToken", "CAccumulatorVitals", "CBehaviorTokenCounter", "CAccumulatorTimed", "CAccumulatorDistanceUnitTraveled" });
            ElementNames.Add("Weapon", new HashSet<string> { "CWeaponLegacy" });
            ElementNames.Add("Actor", new HashSet<string> { "CActorQuad", "CActorRange" });
            ElementNames.Add("Armor", new HashSet<string> { "CArmor" });
        }
    }
}
