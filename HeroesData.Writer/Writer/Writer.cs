using HeroesData.Parser.Models;
using HeroesData.Parser.Models.AbilityTalents;
using HeroesData.Parser.Models.AbilityTalents.Tooltip;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeroesData.FileWriter.Writer
{
    internal abstract class Writer<T>
    {
        protected Writer()
        {
            Directory.CreateDirectory(OutputFolder);
        }

        protected string OutputFolder => "Output";
        protected string RootNode => "Heroes";

        protected string StripInvalidChars(string text)
        {
            return new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }

        protected abstract void CreateMultipleFiles(List<Hero> heroes);
        protected abstract void CreateSingleFile(List<Hero> heroes);
        protected abstract T HeroElement(Hero hero);
        protected abstract T UnitElement(Unit unit);
        protected abstract T UnitLife(Unit unit);
        protected abstract T UnitEnergy(Unit unit);
        protected abstract T RatingsElement(Hero hero);
        protected abstract T WeaponsElement(Unit unit);
        protected abstract T AbilitiesElement(Unit unit, bool isUnitAbilities);
        protected abstract T SubAbilitiesElement(Hero hero);
        protected abstract T TalentsElement(Hero hero);
        protected abstract T HeroUnitsElement(Hero hero);
        protected abstract T AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase);
        protected abstract T TalentInfoElement(Talent talent);
        protected abstract T AbilityLifeElement(TooltipLife tooltipLife);
        protected abstract T AbilityEnergyElement(TooltipEnergy tooltipEnergy);
        protected abstract T AbilityCooldownElement(TooltipCooldown tooltipCooldown);
        protected abstract T AbilityChargesElement(TooltipCharges tooltipCharges);

        protected string GetTooltip(TooltipDescription tooltipDescription, int setting)
        {
            if (setting == 0)
                return tooltipDescription.RawDescription;
            else if (setting == 1)
                return tooltipDescription.PlainText;
            else if (setting == 2)
                return tooltipDescription.PlainTextWithNewlines;
            else if (setting == 3)
                return tooltipDescription.PlainTextWithScaling;
            else if (setting == 4)
                return tooltipDescription.PlainTextWithScalingWithNewlines;
            else if (setting == 6)
                return tooltipDescription.ColoredTextWithScaling;
            else
                return tooltipDescription.ColoredText;
        }
    }
}
