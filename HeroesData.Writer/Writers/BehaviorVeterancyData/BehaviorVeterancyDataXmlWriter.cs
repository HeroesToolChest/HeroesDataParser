using Heroes.Models.Veterancy;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.FileWriter.Writers.BehaviorVeterancyData
{
    internal class BehaviorVeterancyDataXmlWriter : BehaviorVeterancyDataWriter<XElement, XElement>
    {
        public BehaviorVeterancyDataXmlWriter()
            : base(FileOutputType.Xml)
        {
            RootNodeName = "Veterancies";
        }

        protected override XElement MainElement(BehaviorVeterancy behaviorVeterancy)
        {
            return new XElement(
                behaviorVeterancy.Id,
                new XAttribute("combineModifications", behaviorVeterancy.CombineModifications),
                new XAttribute("combineXP", behaviorVeterancy.CombineXP),
                GetVeterancyLevels(behaviorVeterancy.VeterancyLevels));
        }

        protected override XElement GetVeterancyLevels(IEnumerable<VeterancyLevel> veterancyLevels)
        {
            XElement veterancyLevelsElement = new XElement("VeterancyLevels");

            foreach (VeterancyLevel veterancyLevel in veterancyLevels)
            {
                XElement veterancyLevelElement =
                    new XElement(
                        "VeterancyLevel",
                        new XAttribute("minVeterancyXP", veterancyLevel.MinimumVeterancyXP),
                        new XElement(
                            "Modifications",
                            veterancyLevel.VeterancyModification?.KillXpBonus > 0 ? new XAttribute("killXPBonus", veterancyLevel.VeterancyModification.KillXpBonus) : null,
                            veterancyLevel.VeterancyModification?.DamageDealtScaledCollection.Count > 0 ? GetDamageDealtScaledObject(veterancyLevel) : null,
                            veterancyLevel.VeterancyModification?.DamageDealtFractionCollection.Count > 0 ? GetDamageDealtFractionObject(veterancyLevel) : null,
                            veterancyLevel.VeterancyModification?.VitalMaxCollection.Count > 0 ? GetVitalMaxCollectionObject(veterancyLevel) : null,
                            veterancyLevel.VeterancyModification?.VitalMaxFractionCollection.Count > 0 ? GetVitalMaxFractionCollectionObject(veterancyLevel) : null,
                            veterancyLevel.VeterancyModification?.VitalRegenCollection.Count > 0 ? GetVitalRegenObject(veterancyLevel) : null,
                            veterancyLevel.VeterancyModification?.VitalRegenFractionCollection.Count > 0 ? GetVitalRegenFractionObject(veterancyLevel) : null));

                veterancyLevelsElement.Add(veterancyLevelElement);
            }

            return veterancyLevelsElement;
        }

        protected override XElement GetDamageDealtFractionObject(VeterancyLevel veterancyLevel)
        {
            return new XElement(
                "DamageDealtFraction",
                veterancyLevel.VeterancyModification.DamageDealtFractionCollection.Select(collection => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collection.Type), collection.Value)));
        }

        protected override XElement GetDamageDealtScaledObject(VeterancyLevel veterancyLevel)
        {
            return new XElement(
                "DamageDealtScaled",
                veterancyLevel.VeterancyModification.DamageDealtScaledCollection.Select(collection => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collection.Type), collection.Value)));
        }

        protected override XElement GetVitalMaxCollectionObject(VeterancyLevel veterancyLevel)
        {
            return new XElement(
                "VitalMax",
                veterancyLevel.VeterancyModification.VitalMaxCollection.Select(collection => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collection.Type), collection.Value)));
        }

        protected override XElement GetVitalMaxFractionCollectionObject(VeterancyLevel veterancyLevel)
        {
            return new XElement(
                "VitalMaxFraction",
                veterancyLevel.VeterancyModification.VitalMaxFractionCollection.Select(collection => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collection.Type), collection.Value)));
        }

        protected override XElement GetVitalRegenFractionObject(VeterancyLevel veterancyLevel)
        {
            return new XElement(
                "VitalRegenFraction",
                veterancyLevel.VeterancyModification.VitalRegenFractionCollection.Select(collection => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collection.Type), collection.Value)));
        }

        protected override XElement GetVitalRegenObject(VeterancyLevel veterancyLevel)
        {
            return new XElement(
                "VitalRegen",
                veterancyLevel.VeterancyModification.VitalRegenCollection.Select(collection => new XElement(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(collection.Type), collection.Value)));
        }
    }
}
