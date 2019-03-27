using Heroes.Models;
using HeroesData.Helpers;
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
        private readonly ParserHelper ParserHelper;

        private readonly DataTable DataTable = new DataTable();

        private Dictionary<string, HashSet<string>> ElementNames = new Dictionary<string, HashSet<string>>();

        public GameStringParser(GameData gameData)
        {
            GameData = gameData;
            ParserHelper = ParserHelper.Load();
            SetValidElementNames();
        }

        public GameStringParser(GameData gameData, int? hotsBuild)
        {
            GameData = gameData;
            HotsBuild = hotsBuild;
            ParserHelper = ParserHelper.Load(hotsBuild);
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

            if (!IsValidTooltip(key.AsSpan(), tooltip.AsSpan()))
                return false;

            if (string.IsNullOrEmpty(tooltip))
                return true;

            try
            {
                parsedTooltip = ParseTooltip(key, tooltip);

                // unable to parse correctly, returns an empty string
                if (string.IsNullOrEmpty(parsedTooltip) || parsedTooltip.Contains("<d ", StringComparison.OrdinalIgnoreCase))
                {
                    parsedTooltip = string.Empty;
                    return false;
                }

                return true;
            }
            catch (UnparseableException)
            {
                parsedTooltip = string.Empty;
                return false;
            }
            catch (Exception)
            {
                parsedTooltip = string.Empty;
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

        private string ParseTooltip(string referenceNameId, string tooltip)
        {
            tooltip = Regex.Replace(tooltip, @"<d score=.*?/>", "0");
            tooltip = Regex.Replace(tooltip, "<d\\s+ref\\s*=\\s*\"", "<d ref=\"", RegexOptions.IgnoreCase);
            tooltip = Regex.Replace(tooltip, "<d\\s+const\\s*=\\s*\"", "<d const=\"", RegexOptions.IgnoreCase);

            if (tooltip.Contains("<d ref=", StringComparison.OrdinalIgnoreCase) || tooltip.Contains("<d const=", StringComparison.OrdinalIgnoreCase))
            {
                while (tooltip.Contains("[d ref=", StringComparison.OrdinalIgnoreCase))
                {
                    tooltip = ParseGameString(referenceNameId, tooltip, "(\\[d ref=\".*?/\\])", true);
                }

                return ParseGameString(referenceNameId, tooltip, "(<d ref=\".*?/>|<d const=\".*?/>)", false);
            }
            else // no values to look up
            {
                return tooltip;
            }
        }

        private string ParseGameString(string referenceNameId, string tooltip, string splitter, bool nestedTooltip)
        {
            if (nestedTooltip)
            {
                tooltip = tooltip.Replace("'", "\"");
            }

            string[] parts = Regex.Split(tooltip, splitter);

            for (int i = 0; i < parts.Length; i++)
            {
                if (nestedTooltip)
                {
                    if (!parts[i].Contains("[d ref="))
                        continue;
                }
                else
                {
                    if (!parts[i].Contains("<d ref=") && !parts[i].Contains("<d const="))
                        continue;
                }

                string pathLookup = parts[i];

                // get and remove precision
                pathLookup = GetPrecision(pathLookup, out int? precision);

                // remove the player
                pathLookup = GetPlayer(pathLookup, out int? player);

                // perform xml data lookup to find values
                string mathPath = ParseValues(pathLookup.AsMemory());

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

                double number = HeroesMathEval.CalculatePathEquation(mathPath.Trim('/'));

                if (precision.HasValue)
                    parts[i] = Math.Round(number, precision.Value).ToString();
                else
                    parts[i] = Math.Round(number, 0).ToString();

                if (!string.IsNullOrEmpty(scalingText))
                {
                    // only add the scaling text if the next part does not start with a % sign or is time based
                    if (i + 1 < parts.Length)
                    {
                        ReadOnlySpan<char> nextPart = parts[i + 1];
                        nextPart = nextPart.Trim();

                        if (nextPart.StartsWith("</c>"))
                            nextPart = nextPart.Slice("</c>".Length).TrimStart();
                        if (nextPart.StartsWith("<n/>"))
                            nextPart = nextPart.Slice("<n/>".Length).TrimStart();
                        if (!(nextPart.StartsWith("%") || nextPart.StartsWith("0%") || IsTimeBasedScaling(pathLookup.AsMemory())))
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
            dRef = GetPrecision(dRef, out int? precision);
            dRef = GetPlayer(dRef, out int? player);

            dRef = ParseValues(dRef.AsMemory());

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

            double number = HeroesMathEval.CalculatePathEquation(dRef);

            if (precision.HasValue)
                return Math.Round(number, precision.Value);
            else
                return Math.Round(number, 0);
        }

        private string GetPrecision(string pathPart, out int? precision)
        {
            precision = null;

            // get the precision string first
            Regex regex = new Regex("precision\\s*=\\s*\".*?\"", RegexOptions.IgnoreCase);
            MatchCollection precisionMatches = regex.Matches(pathPart);

            if (precisionMatches.Count > 0)
            {
                // now get the value
                regex = new Regex("\".*?\"");
                MatchCollection match = regex.Matches(precisionMatches[0].Value);

                precision = int.Parse(match[0].Value.AsSpan().Trim('"'));

                // remove precision text
                pathPart = pathPart.Replace(precisionMatches[0].Value, string.Empty);
            }

            return pathPart;
        }

        private string GetPlayer(string pathPart, out int? player)
        {
            player = null;

            // get the player string first
            Regex regex = new Regex("player\\s*=\\s*\".*?\"", RegexOptions.IgnoreCase);
            MatchCollection playerMatches = regex.Matches(pathPart);

            if (playerMatches.Count > 0)
            {
                // now get the value
                regex = new Regex("\".*?\"");
                MatchCollection match = regex.Matches(playerMatches[0].Value);

                if (int.TryParse(match[0].Value.AsSpan().Trim('"'), out int playerNum))
                    player = playerNum;
                else
                    player = null;

                // remove player text
                pathPart = pathPart.Replace(playerMatches[0].Value, string.Empty);
            }

            return pathPart;
        }

        private string ParseValues(ReadOnlyMemory<char> pathLookup)
        {
            pathLookup = TrimDataReference(pathLookup);

            foreach (ReadOnlyMemory<char> arithmeticPath in GetPartBySeperators(pathLookup, new char[] { '/', '*', '+', '-', '(', ')' }))
            {
                ReadOnlySpan<char> path = arithmeticPath.Span;

                if (!double.TryParse(path.Trim(), out double constant))
                {
                    if (!path.Trim().IsEmpty)
                    {
                        int position = pathLookup.Span.IndexOf(arithmeticPath.Span);

                        if (position >= 0)
                        {
                            string value = GetValueFromPath(arithmeticPath.ToString(), out double? scalingValue);

                            if (scalingValue.HasValue)
                                pathLookup = $"{pathLookup.Slice(0, position)}{value}~~{scalingValue}~~{pathLookup.Slice(position + arithmeticPath.Length)}".AsMemory();
                            else
                                pathLookup = (pathLookup.Slice(0, position) + value + pathLookup.Slice(position + arithmeticPath.Length)).AsMemory();
                        }
                    }
                }
            }

            return pathLookup.ToString();
        }

        private string GetScalingText(string pathLookup)
        {
            // get scaling string
            Regex regex = new Regex("~~.*?~~");
            MatchCollection playerMatches = regex.Matches(pathLookup);

            if (playerMatches.Count < 1)
                return string.Empty;

            return playerMatches[0].Value;
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

        private string ReadData(List<string> parts, List<string> scalingParts, out double? scaling)
        {
            scaling = null;
            string value = string.Empty;

            if (parts[0].StartsWith("$"))
                value = GameData.GetValueFromAttribute(parts[0]);
            else
                value = ReadGameData(parts, null);

            if (scalingParts.Count == 3)
                scaling = GetScalingInfo(scalingParts[0], scalingParts[1], scalingParts[2]);

            if (string.IsNullOrEmpty(value))
            {
                // check each part value to see if we can find one
                foreach (var (name, partIndex, partValue) in ParserHelper.PartValueByPartName)
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

            IEnumerable<XElement> elements = GameData.Elements(validElementNames.First()).Where(x => x.Attribute("id")?.Value == parts[1]);
            for (int i = 1; i < validElementNames.Count(); i++)
            {
                elements = elements.Concat(GameData.Elements(validElementNames.ElementAt(i)).Where(x => x.Attribute("id")?.Value == parts[1]));
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
                                string lastPart = parts.Last();

                                ReadOnlySpan<char> lastPartSpan = lastPart.AsSpan();
                                int indexOfArray = lastPartSpan.IndexOf('[');

                                if (indexOfArray > -1)
                                    lastPartSpan = lastPartSpan.Slice(0, indexOfArray);

                                XAttribute value = indexElements[j].Attribute(lastPartSpan.ToString());
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
                        return GameData.GetValueFromAttribute(currentElement.Attribute(parts[i]).Value);
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
                    {
                        attribute = currentElement.Attribute("Value");
                    }
                    else if (attribute != null)
                    {
                        return GameData.GetValueFromAttribute(attribute.Value);
                    }
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

        private IEnumerable<ReadOnlyMemory<char>> GetPartBySeperators(ReadOnlyMemory<char> pathLookup, ReadOnlyMemory<char> seperators)
        {
            ReadOnlyMemory<char> part;
            int length = 0;

            for (int i = 0; i < pathLookup.Length; i++)
            {
                if (seperators.Span.IndexOf(pathLookup.Span[i]) > -1)
                {
                    part = pathLookup.Slice(i - length, length);
                    if (!part.Span.Trim().IsEmpty)
                        yield return part;

                    length = 0;
                }
                else
                {
                    length++;
                }
            }

            part = pathLookup.Slice(pathLookup.Length - length, length);
            if (!part.Span.Trim().IsEmpty)
                yield return part;
        }

        private bool IsTimeBasedScaling(ReadOnlyMemory<char> pathLookup)
        {
            List<ReadOnlyMemory<char>> arithmeticPaths = GetPartBySeperators(TrimDataReference(pathLookup), new char[] { '/', '*', '+', '-', '(', ')' }).ToList();

            for (int i = 0; i + 2 <= arithmeticPaths.Count; i++)
            {
                if (arithmeticPaths.Count >= i + 2 && arithmeticPaths[i].Span.TrimEnd().EndsWith("LifeMax") && arithmeticPaths[i + 1].Span.TrimEnd().EndsWith("LifeRegenRate"))
                    return true;
            }

            return false;
        }

        private bool IsValidTooltip(ReadOnlySpan<char> key, ReadOnlySpan<char> tooltip)
        {
            return !(key.IsEmpty || tooltip.Contains("$GalaxyVar", StringComparison.OrdinalIgnoreCase) || tooltip.Contains("**TEMP**", StringComparison.OrdinalIgnoreCase));
        }

        private ReadOnlyMemory<char> TrimDataReference(ReadOnlyMemory<char> pathLookup)
        {
            if (pathLookup.Span.StartsWith("<d ref", StringComparison.OrdinalIgnoreCase) || pathLookup.Span.StartsWith("[d ref", StringComparison.OrdinalIgnoreCase))
                pathLookup = pathLookup.Slice(8);
            else if (pathLookup.Span.StartsWith("<d const", StringComparison.OrdinalIgnoreCase))
                pathLookup = pathLookup.Slice(10);

            pathLookup = pathLookup.Slice(0, pathLookup.Span.LastIndexOf('/') - 1); // removes ending />

            int start = 0;
            int end = pathLookup.Length - 1;
            char startChar = pathLookup.Span[start];
            char endChar = pathLookup.Span[end];

            while ((start < end) && (endChar == ' ' || endChar == '"'))
            {
                if (endChar == ' ' || endChar == '"')
                {
                    end--;
                }

                endChar = pathLookup.Span[end];
            }

            return pathLookup.Slice(start, end - start + 1);
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
            ElementNames.Add("Accumulator", new HashSet<string> { "CAccumulatorToken", "CAccumulatorVitals", "CBehaviorTokenCounter", "CAccumulatorTimed", "CAccumulatorDistanceUnitTraveled", "CAccumulatorDistance", "CAccumulatorTrackedUnitCount" });
            ElementNames.Add("Weapon", new HashSet<string> { "CWeaponLegacy" });
            ElementNames.Add("Actor", new HashSet<string> { "CActorQuad", "CActorRange" });
            ElementNames.Add("Armor", new HashSet<string> { "CArmor" });
        }
    }
}
