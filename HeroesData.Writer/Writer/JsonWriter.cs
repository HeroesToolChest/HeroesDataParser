using HeroesData.FileWriter.Settings;
using HeroesData.Parser.Models;
using HeroesData.Parser.Models.AbilityTalents;
using HeroesData.Parser.Models.AbilityTalents.Tooltip;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.FileWriter.Writer
{
    internal class JsonWriter : Writer<JProperty>
    {
        private readonly JsonFileSettings FileSettings;
        private readonly string SingleFileName = "heroesdata.json";

        private JsonWriter(JsonFileSettings fileSettings, List<Hero> heroes)
        {
            FileSettings = fileSettings;

            if (FileSettings.WriterEnabled)
            {
                if (FileSettings.FileSplit)
                    CreateMultipleFiles(heroes);
                else
                    CreateSingleFile(heroes);
            }
        }

        public static void CreateOutput(JsonFileSettings fileSettings, List<Hero> heroes)
        {
            new JsonWriter(fileSettings, heroes);
        }

        protected override void CreateMultipleFiles(List<Hero> heroes)
        {
            throw new System.NotImplementedException();
        }

        protected override void CreateSingleFile(List<Hero> heroes)
        {
            //JObject jObject = new JObject(new JProperty(RootNode, heroes.Select(hero => HeroElement(hero))));
        }

        protected override JProperty HeroElement(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty UnitElement(Unit unit)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty UnitLife(Unit unit)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty UnitEnergy(Unit unit)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty AbilitiesElement(Unit unit, bool isUnitAbilities)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty AbilityChargesElement(TooltipCharges tooltipCharges)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty AbilityCooldownElement(TooltipCooldown tooltipCooldown)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty AbilityEnergyElement(TooltipEnergy tooltipEnergy)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty AbilityLifeElement(TooltipLife tooltipLife)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty AbilityTalentInfoElement(AbilityTalentBase abilityTalentBase)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty SubAbilitiesElement(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty HeroUnitsElement(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty RatingsElement(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty TalentInfoElement(Talent talent)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty TalentsElement(Hero hero)
        {
            throw new System.NotImplementedException();
        }

        protected override JProperty WeaponsElement(Unit unit)
        {
            throw new System.NotImplementedException();
            //if (FileSettings.IncludeWeapons && hero.Weapons?.Count > 0)
            //{
            //    return new JProperty(
            //        "Weapons",
            //        hero.Weapons.Select(w => new JProperty(
            //            w.WeaponNameId,
            //            new JArray("range", w.Range),
            //            new JArray("period", w.Period),
            //            new JProperty("Damage", w.Damage, new JArray("scale", w.DamageScaling)))));
            //}
            //else
            //{
            //    return null;
            //}
        }
    }
}
