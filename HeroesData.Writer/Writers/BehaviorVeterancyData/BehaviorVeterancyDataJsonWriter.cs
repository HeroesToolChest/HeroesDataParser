using Heroes.Models.Veterancy;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HeroesData.FileWriter.Writers.BehaviorVeterancyData
{
    internal class BehaviorVeterancyDataJsonWriter : BehaviorVeterancyDataWriter<JProperty, JObject>
    {
        public BehaviorVeterancyDataJsonWriter()
            : base(FileOutputType.Json)
        {
        }

        protected override JProperty MainElement(BehaviorVeterancy behaviorVeterancy)
        {
            JObject veterancyObject = new JObject
            {
                { "combineModifications", behaviorVeterancy.CombineModifications },
                { "combineXP", behaviorVeterancy.CombineXP },
            };

            veterancyObject.Add(GetVeterancyLevels(behaviorVeterancy.VeterancyLevels));

            return new JProperty(behaviorVeterancy.Id, veterancyObject);
        }

        protected override JProperty GetVeterancyLevels(IEnumerable<VeterancyLevel> veterancyLevels)
        {
            JArray veterancyLevelArray = new JArray();

            foreach (VeterancyLevel veterancyLevel in veterancyLevels)
            {
                JObject veterancyLevelObject = new JObject();
                JObject modificationObject = new JObject();

                if (veterancyLevel.VeterancyModification?.KillXpBonus > 0)
                    modificationObject.Add(new JProperty("killXPBonus", veterancyLevel.VeterancyModification.KillXpBonus));

                if (veterancyLevel.VeterancyModification?.DamageDealtScaledCollection.Count > 0)
                    modificationObject.Add(GetDamageDealtScaledObject(veterancyLevel));

                if (veterancyLevel.VeterancyModification?.DamageDealtFractionCollection.Count > 0)
                    modificationObject.Add(GetDamageDealtFractionObject(veterancyLevel));

                if (veterancyLevel.VeterancyModification?.VitalMaxCollection.Count > 0)
                    modificationObject.Add(GetVitalMaxCollectionObject(veterancyLevel));

                if (veterancyLevel.VeterancyModification?.VitalMaxFractionCollection.Count > 0)
                    modificationObject.Add(GetVitalMaxFractionCollectionObject(veterancyLevel));

                if (veterancyLevel.VeterancyModification?.VitalRegenCollection.Count > 0)
                    modificationObject.Add(GetVitalRegenObject(veterancyLevel));

                if (veterancyLevel.VeterancyModification?.VitalRegenFractionCollection.Count > 0)
                    modificationObject.Add(GetVitalRegenFractionObject(veterancyLevel));

                veterancyLevelObject.Add("minVeterancyXP", veterancyLevel.MinimumVeterancyXP);

                if (modificationObject.HasValues)
                    veterancyLevelObject.Add(new JProperty("modifications", modificationObject));

                veterancyLevelArray.Add(veterancyLevelObject);
            }

            return new JProperty("veterancyLevels", veterancyLevelArray);
        }

        protected override JProperty GetDamageDealtScaledObject(VeterancyLevel veterancyLevel)
        {
            JObject jObject = new JObject();

            foreach (VeterancyDamageDealtScaled veterancyProperty in veterancyLevel.VeterancyModification.DamageDealtScaledCollection)
                jObject.Add(new JProperty(veterancyProperty.Type, veterancyProperty.Value));

            return new JProperty("damageDealtScaled", jObject);
        }

        protected override JProperty GetDamageDealtFractionObject(VeterancyLevel veterancyLevel)
        {
            JObject jObject = new JObject();

            foreach (VeterancyDamageDealtFraction veterancyProperty in veterancyLevel.VeterancyModification.DamageDealtFractionCollection)
                jObject.Add(new JProperty(veterancyProperty.Type, veterancyProperty.Value));

            return new JProperty("damageDealtFraction", jObject);
        }

        protected override JProperty GetVitalMaxCollectionObject(VeterancyLevel veterancyLevel)
        {
            JObject jObject = new JObject();

            foreach (VeterancyVitalMax veterancyProperty in veterancyLevel.VeterancyModification.VitalMaxCollection)
                jObject.Add(new JProperty(veterancyProperty.Type, veterancyProperty.Value));

            return new JProperty("vitalMax", jObject);
        }

        protected override JProperty GetVitalMaxFractionCollectionObject(VeterancyLevel veterancyLevel)
        {
            JObject jObject = new JObject();

            foreach (VeterancyVitalMaxFraction veterancyProperty in veterancyLevel.VeterancyModification.VitalMaxFractionCollection)
                jObject.Add(new JProperty(veterancyProperty.Type, veterancyProperty.Value));

            return new JProperty("vitalMaxFraction", jObject);
        }

        protected override JProperty GetVitalRegenObject(VeterancyLevel veterancyLevel)
        {
            JObject jObject = new JObject();

            foreach (VeterancyVitalRegen veterancyProperty in veterancyLevel.VeterancyModification.VitalRegenCollection)
                jObject.Add(new JProperty(veterancyProperty.Type, veterancyProperty.Value));

            return new JProperty("vitalRegen", jObject);
        }

        protected override JProperty GetVitalRegenFractionObject(VeterancyLevel veterancyLevel)
        {
            JObject jObject = new JObject();

            foreach (VeterancyVitalRegenFraction veterancyProperty in veterancyLevel.VeterancyModification.VitalRegenFractionCollection)
                jObject.Add(new JProperty(veterancyProperty.Type, veterancyProperty.Value));

            return new JProperty("vitalRegenFraction", jObject);
        }
    }
}
