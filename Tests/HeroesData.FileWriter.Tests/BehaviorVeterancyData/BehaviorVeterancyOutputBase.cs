using Heroes.Models;
using Heroes.Models.Veterancy;

namespace HeroesData.FileWriter.Tests.BehaviorVeterancyData
{
    public class BehaviorVeterancyOutputBase : FileOutputTestBase<BehaviorVeterancy>
    {
        public BehaviorVeterancyOutputBase()
            : base(nameof(BehaviorVeterancyData))
        {
        }

        protected override void SetTestData()
        {
            BehaviorVeterancy behaviorVeterancy = new BehaviorVeterancy()
            {
                CombineModifications = true,
                CombineXP = true,
                Id = "Vet1",
            };

            var veterancyModification = new VeterancyModification
            {
                KillXpBonus = 10,
            };
            veterancyModification.DamageDealtScaledCollection.Add(
                new VeterancyDamageDealtScaled()
                {
                    Type = "basic",
                    Value = 0.2,
                });
            veterancyModification.DamageDealtScaledCollection.Add(
                new VeterancyDamageDealtScaled()
                {
                    Type = "ability",
                    Value = 0.6,
                });
            veterancyModification.VitalRegenFractionCollection.Add(
                new VeterancyVitalRegenFraction()
                {
                    Type = "life",
                    Value = 0.02,
                });

            behaviorVeterancy.VeterancyLevels.Add(new VeterancyLevel()
            {
                MinimumVeterancyXP = 5,
                VeterancyModification = veterancyModification,
            });

            var veterancyModification2 = new VeterancyModification()
            {
                KillXpBonus = 0,
            };
            veterancyModification2.DamageDealtScaledCollection.Add(
                new VeterancyDamageDealtScaled()
                {
                    Type = "basic",
                    Value = 0.4,
                });

            veterancyModification2.DamageDealtScaledCollection.Add(
                new VeterancyDamageDealtScaled()
                {
                    Type = "ability",
                    Value = 0.5,
                });

            veterancyModification2.VitalRegenFractionCollection.Add(
                new VeterancyVitalRegenFraction()
                {
                    Type = "life",
                    Value = 0.03,
                });

            behaviorVeterancy.VeterancyLevels.Add(new VeterancyLevel()
            {
                MinimumVeterancyXP = 9,
                VeterancyModification = veterancyModification2,
            });

            TestData.Add(behaviorVeterancy);
        }
    }
}
