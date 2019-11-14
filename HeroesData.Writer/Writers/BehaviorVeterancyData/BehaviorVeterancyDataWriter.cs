using Heroes.Models;
using Heroes.Models.Veterancy;
using System.Collections.Generic;

namespace HeroesData.FileWriter.Writers.BehaviorVeterancyData
{
    internal abstract class BehaviorVeterancyDataWriter<T, TU> : WriterBase<BehaviorVeterancy, T>
        where T : class
        where TU : class
    {
        public BehaviorVeterancyDataWriter(FileOutputType fileOutputType)
            : base(nameof(BehaviorVeterancyData), fileOutputType)
        {
        }

        protected abstract T GetVeterancyLevels(IEnumerable<VeterancyLevel> veterancyLevels);
        protected abstract T GetDamageDealtScaledObject(VeterancyLevel veterancyLevel);
        protected abstract T GetDamageDealtFractionObject(VeterancyLevel veterancyLevel);
        protected abstract T GetVitalMaxCollectionObject(VeterancyLevel veterancyLevel);
        protected abstract T GetVitalMaxFractionCollectionObject(VeterancyLevel veterancyLevel);
        protected abstract T GetVitalRegenObject(VeterancyLevel veterancyLevel);
        protected abstract T GetVitalRegenFractionObject(VeterancyLevel veterancyLevel);
    }
}
