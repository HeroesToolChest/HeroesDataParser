using Heroes.Models;
using Heroes.Models.Veterancy;
using System.Collections.Generic;

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

            behaviorVeterancy.VeterancyLevels.Add(new VeterancyLevel()
            {
                MinimumVeterancyXP = 5,
                VeterancyModification = new VeterancyModification()
                {
                    KillXpBonus = 10,
                    DamageDealtScaledCollection = new List<VeterancyDamageDealtScaled>()
                    {
                        new VeterancyDamageDealtScaled()
                        {
                            Type = "basic",
                            Value = 0.2,
                        },
                        new VeterancyDamageDealtScaled()
                        {
                            Type = "ability",
                            Value = 0.6,
                        },
                    },
                    VitalRegenFractionCollection = new List<VeterancyVitalRegenFraction>()
                    {
                        new VeterancyVitalRegenFraction()
                        {
                            Type = "life",
                            Value = 0.02,
                        },
                    },
                },
            });

            behaviorVeterancy.VeterancyLevels.Add(new VeterancyLevel()
            {
                MinimumVeterancyXP = 9,
                VeterancyModification = new VeterancyModification()
                {
                    KillXpBonus = 0,
                    DamageDealtScaledCollection = new List<VeterancyDamageDealtScaled>()
                    {
                        new VeterancyDamageDealtScaled()
                        {
                            Type = "basic",
                            Value = 0.4,
                        },
                        new VeterancyDamageDealtScaled()
                        {
                            Type = "ability",
                            Value = 0.5,
                        },
                    },
                    VitalRegenFractionCollection = new List<VeterancyVitalRegenFraction>()
                    {
                        new VeterancyVitalRegenFraction()
                        {
                            Type = "life",
                            Value = 0.03,
                        },
                    },
                },
            });

            TestData.Add(behaviorVeterancy);
        }
    }
}
