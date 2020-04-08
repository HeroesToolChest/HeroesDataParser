using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class DefaultDataButton
    {
        public const string CButtonDefaultBaseId = "StormButtonParent";

        private readonly GameData _gameData;

        public DefaultDataButton(GameData gameData)
        {
            _gameData = gameData;

            LoadCButtonDefault();
            LoadCButtonDefaultStormButtonParent();
        }

        /// <summary>
        /// Gets the default button name text. Contains ##id##.
        /// </summary>
        public string? ButtonName { get; private set; }

        /// <summary>
        /// Gets the default button tooltip text. Contains ##id##. Full text.
        /// </summary>
        public string? ButtonTooltip { get; private set; }

        /// <summary>
        /// Gets the default button simple display text. Contains ##id##. Short text.
        /// </summary>
        public string? ButtonSimpleDisplayText { get; private set; }

        /// <summary>
        /// Gets the default button tooltip vital text.
        /// </summary>
        public string? ButtonTooltipEnergyVitalName { get; private set; }

        /// <summary>
        /// Gets the default button hotkey text. Contains ##id##.
        /// </summary>
        public string? ButtonHotkey { get; private set; }

        /// <summary>
        /// Gets the default button hotkey alias text. Contains ##id##.
        /// </summary>
        public string? ButtonHotkeyAlias { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show name.
        /// </summary>
        public bool ButtonTooltipFlagShowName { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show hotkey.
        /// </summary>
        public bool ButtonTooltipFlagShowHotkey { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show usage.
        /// </summary>
        public bool ButtonTooltipFlagShowUsage { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show time.
        /// </summary>
        public bool ButtonTooltipFlagShowTime { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show cooldown.
        /// </summary>
        public bool ButtonTooltipFlagShowCooldown { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show requirements.
        /// </summary>
        public bool ButtonTooltipFlagShowRequirements { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the default button tooltip flag - show autocast.
        /// </summary>
        public bool ButtonTooltipFlagShowAutocast { get; private set; } = false;

        // <CButton default="1">
        private void LoadCButtonDefault()
        {
            CButtonElement(_gameData.Elements("CButton").Where(x => x.Attribute("default")?.Value == "1" && x.Attributes().Count() == 1));
        }

        // <CButton default="1" id="StormButtonParent">
        private void LoadCButtonDefaultStormButtonParent()
        {
            CButtonElement(_gameData.Elements("CButton").Where(x => x.Attribute("default")?.Value == "1" && x.Attribute("id")?.Value == CButtonDefaultBaseId));
        }

        private void CButtonElement(IEnumerable<XElement> cButtonElements)
        {
            foreach (XElement element in cButtonElements.Elements())
            {
                string elementName = element.Name.LocalName.ToUpperInvariant();

                if (elementName == "NAME")
                {
                    ButtonName = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIP")
                {
                    ButtonTooltip = element.Attribute("value").Value;
                }
                else if (elementName == "HOTKEY")
                {
                    ButtonHotkey = element.Attribute("value").Value;
                }
                else if (elementName == "HOTKEYALIAS")
                {
                    ButtonHotkeyAlias = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIPFLAGS")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "ShowName")
                        ButtonTooltipFlagShowName = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowHotkey")
                        ButtonTooltipFlagShowHotkey = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowUsage")
                        ButtonTooltipFlagShowUsage = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowTime")
                        ButtonTooltipFlagShowTime = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowCooldown")
                        ButtonTooltipFlagShowCooldown = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowRequirements")
                        ButtonTooltipFlagShowRequirements = element.Attribute("value").Value == "1" ? true : false;
                    else if (index == "ShowAutocast")
                        ButtonTooltipFlagShowAutocast = element.Attribute("value").Value == "1" ? true : false;
                }
                else if (elementName == "SIMPLEDISPLAYTEXT")
                {
                    ButtonSimpleDisplayText = element.Attribute("value").Value;
                }
                else if (elementName == "TOOLTIPVITALNAME")
                {
                    string index = element.Attribute("index").Value;

                    if (index == "Energy")
                        ButtonTooltipEnergyVitalName = element.Attribute("value").Value;
                }
            }
        }
    }
}
