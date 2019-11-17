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
        public const string FailedParsed = "(╯°□°）╯︵ ┻━┻ [Failed to parse]";
        public const string ErrorTag = "##ERROR##";

        private readonly int? HotsBuild;
        private readonly GameData GameData;
        private readonly Configuration Configuration;

        public GameStringParser(GameData gameData)
        {
            Configuration = new Configuration();
            GameData = gameData;
        }

        public GameStringParser(GameData gameData, int? hotsBuild)
        {
            Configuration = new Configuration();
            GameData = gameData;
            HotsBuild = hotsBuild;
        }

        public GameStringParser(Configuration configuration, GameData gameData)
        {
            GameData = gameData;
            Configuration = configuration;
        }

        public GameStringParser(Configuration configuration, GameData gameData, int? hotsBuild)
        {
            Configuration = configuration;
            GameData = gameData;
            HotsBuild = hotsBuild;
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
                parsedTooltip = ParseTooltip(tooltip);
            }
            catch (Exception ex) when (ex is FormatException || ex is SyntaxErrorException || ex is IndexOutOfRangeException)
            {
                parsedTooltip = FailedParsed;
                return false;
            }
            catch (GameStringParseException ex)
            {
                throw new GameStringParseException($"Failed to parse \"{key}\". {ex.Message}", ex);
            }

            // unable to parse correctly, returns an empty string
            if (string.IsNullOrEmpty(parsedTooltip) || parsedTooltip.Contains("<d ", StringComparison.OrdinalIgnoreCase))
            {
                parsedTooltip = FailedParsed;
                return false;
            }

            return true;
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

        private string ParseTooltip(string tooltip)
        {
            tooltip = Regex.Replace(tooltip, @"<d score=.*?/>", "0");
            tooltip = Regex.Replace(tooltip, "<d\\s+ref\\s*=\\s*\"", "<d ref=\"", RegexOptions.IgnoreCase);
            tooltip = Regex.Replace(tooltip, "<d\\s+const\\s*=\\s*\"", "<d const=\"", RegexOptions.IgnoreCase);

            if (tooltip.Contains("<d ref=", StringComparison.OrdinalIgnoreCase) || tooltip.Contains("<d const=", StringComparison.OrdinalIgnoreCase))
            {
                while (tooltip.Contains("[d ref=", StringComparison.OrdinalIgnoreCase))
                {
                    tooltip = ParseGameString(tooltip, "(\\[d ref=\".*?/\\])", true);
                }

                return ReplaceValVariables(ParseGameString(tooltip, "(<d ref=\".*?/>|<d const=\".*?/>)", false));
            }
            else // no values to look up
            {
                return ReplaceValVariables(tooltip);
            }
        }

        private string ParseGameString(string tooltip, string splitter, bool nestedTooltip)
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
                pathLookup = GetPlayer(pathLookup, out _);

                // perform xml data lookup to find values
                string mathPath = ParseValues(pathLookup.AsMemory());

                if (string.IsNullOrEmpty(Regex.Replace(mathPath, @"[/*+\-()]", string.Empty)))
                {
                    return string.Empty;
                }

                // extract the scaling and the amount
                string scalingText = GetScalingText(mathPath, out int numOfScalingTexts);

                if (!string.IsNullOrEmpty(scalingText))
                {
                    mathPath = mathPath.Replace(scalingText, string.Empty);
                }

                // calculate the value
                double number;
                bool calculateSuccess = true;
                try
                {
                    number = HeroesMathEval.CalculatePathEquation(mathPath.Trim('/'));
                }
                catch (Exception ex) when (ex is FormatException || ex is SyntaxErrorException || ex is IndexOutOfRangeException)
                {
                    number = 0;
                    calculateSuccess = false;
                }

                if (precision.HasValue)
                    parts[i] = Math.Round(number, precision.Value).ToString();
                else
                    parts[i] = Math.Round(number, 0).ToString();

                if (!calculateSuccess)
                {
                    parts[i] += ErrorTag;
                }

                if (!string.IsNullOrEmpty(scalingText))
                {
                    // only add the scaling in certain cases
                    if (numOfScalingTexts == 1 || (numOfScalingTexts > 1 && !mathPath.Contains('/')))
                    {
                        ReadOnlySpan<char> nextPart = parts[i + 1];
                        nextPart = nextPart.TrimStart();

                        if (nextPart.StartsWith("%"))
                        {
                            parts[i] += $"%{scalingText}";
                            parts[i + 1] = parts[i + 1].ReplaceFirst("%", string.Empty);
                        }
                        else
                        {
                            parts[i] += $"{scalingText}";
                        }
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

                foreach (Match? match in Regex.Matches(joinDesc, @"[a-z]+""[a-z]+"))
                {
                    if (match == null)
                        continue;

                    joinDesc = joinDesc.Replace(match.Value, match.Value.Replace("\"", "'"));
                }

                return DescriptionValidator.Validate(joinDesc);
            }
        }

        private double? ParseDataReferenceString(string dRef)
        {
            dRef = GetPrecision(dRef, out int? precision);
            dRef = GetPlayer(dRef, out _);

            dRef = ParseValues(dRef.AsMemory());

            if (string.IsNullOrEmpty(Regex.Replace(dRef, @"[/*+\-()]", string.Empty)))
            {
                return null;
            }

            // extract the scaling
            string scalingText = GetScalingText(dRef, out _);

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

            pathPart = pathPart.Replace("preicison=", "precision=");

            // get the precision string first
            MatchCollection precisionMatches = Regex.Matches(pathPart, "precision\\s*=\\s*\".*?\"", RegexOptions.IgnoreCase);

            if (precisionMatches.Count > 0)
            {
                // now get the value
                MatchCollection match = Regex.Matches(precisionMatches[0].Value, "\".*?\"");

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
            MatchCollection playerMatches = Regex.Matches(pathPart, "player\\s*=\\s*\".*?\"", RegexOptions.IgnoreCase);

            if (playerMatches.Count > 0)
            {
                // now get the value
                MatchCollection match = Regex.Matches(playerMatches[0].Value, "\".*?\"");

                if (int.TryParse(match[0].Value.AsSpan().Trim('"'), out int playerNum))
                    player = playerNum;
                else
                    player = null;

                // remove player text
                pathPart = pathPart.Replace(playerMatches[0].Value, string.Empty);
            }

            return pathPart;
        }

        private string ReplaceValVariables(string tooltip)
        {
            MatchCollection valSMatches = Regex.Matches(tooltip, "s\\sval=\".*?\"", RegexOptions.IgnoreCase);
            MatchCollection valCMatches = Regex.Matches(tooltip, "c\\sval=\".*?\"", RegexOptions.IgnoreCase);

            foreach (Match item in valSMatches.Distinct(new MatchComparer()))
            {
                MatchCollection valueMatch = Regex.Matches(item.Value, "\".*?\"");

                string hexValue = GameData.GetStormStyleHexValueFromName(valueMatch[0].Value.Trim('"'));

                if (!string.IsNullOrEmpty(hexValue))
                {
                    tooltip = tooltip.Replace(valueMatch[0].Value, $"\"{hexValue}\" name={valueMatch[0].Value}");
                }
            }

            foreach (Match item in valCMatches.Distinct(new MatchComparer()))
            {
                MatchCollection valueMatch = Regex.Matches(item.Value, "\".*?\"");

                string hexValue = GameData.GetStormStyleHexValueFromName(valueMatch[0].Value.Trim('"'));

                if (!string.IsNullOrEmpty(hexValue))
                {
                    tooltip = tooltip.Replace(valueMatch[0].Value, $"\"{hexValue}\"");
                }
            }

            return tooltip;
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

        private string GetScalingText(string pathLookup, out int count)
        {
            count = 0;

            // get scaling string
            MatchCollection playerMatches = Regex.Matches(pathLookup, "~~.*?~~");

            if (playerMatches.Count < 1)
                return string.Empty;
            else
                count = playerMatches.Count;

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
            else if (parts.Count > 2)
                value = ReadXmlGameData(parts, null);

            if (scalingParts.Count == 3)
                scaling = GetScalingInfo(scalingParts[0], scalingParts[1], scalingParts[2]);

            if (string.IsNullOrEmpty(value))
            {
                // see if we can find a default value
                foreach ((string part, string partValue) in Configuration.GamestringDefaultValues(parts.Last()))
                {
                    if (part == "last")
                        return partValue;
                    else if (int.TryParse(part, out int partAsInt) && parts.Count() > partAsInt && parts[partAsInt] == parts.Last())
                        return partValue;
                }
            }

            return value;
        }

        private string ReadXmlGameData(List<string> parts, XAttribute? parent)
        {
            XElement? currentElement = null;
            parent = null;

            IEnumerable<string> xmlElementNames = Configuration.GamestringXmlElements(parts[0]);
            if (!xmlElementNames.Any())
            {
                IEnumerable<string>? foundElements = GameData.XmlGameData.Root.Elements().Where(x => x.Name.LocalName.StartsWith($"C{parts[0]}") && x.Attribute("id")?.Value == parts[1])?.Select(x => x.Name.LocalName);
                if (foundElements != null && foundElements.Any())
                {
                    throw new GameStringParseException($"The element type \"{parts[0]}\" was not found in the configuration file. The following elements were found for the missing type for the given id of \"{parts[1]}\": {string.Join(',', foundElements)}." +
                        $" Try adding some or all of the elements to the config.xml file XmlElementLookup section for the missing type.");
                }
            }

            IEnumerable<XElement> elements = new List<XElement>();

            foreach (string elementName in xmlElementNames)
            {
                elements = GameData.Elements(elementName).Where(x => x.Attribute("id")?.Value == parts[1]);
                if (elements.Any())
                    break;
            }

            if (!elements.Any())
            {
                IEnumerable<string>? elementDifference = GameData.XmlGameData.Root.Elements().Where(x => x.Name.LocalName.StartsWith($"C{parts[0]}") && x.Attribute("id")?.Value == parts[1])?.Select(x => x.Name.LocalName).Except(xmlElementNames);
                if (elementDifference != null && elementDifference.Any())
                    throw new GameStringParseException($"No elements were found for the type \"{parts[0]}\" given the id of \"{parts[1]}\". Try adding some or all of the following element(s) to the \"{parts[0]}\" type in the config.xml file XmlElementLookup section: {string.Join(',', elementDifference)}.");
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
                    string value = ReadXmlGameData(parts, parent);
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
    }
}
