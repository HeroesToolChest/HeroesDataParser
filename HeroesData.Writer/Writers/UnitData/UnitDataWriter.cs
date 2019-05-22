using Heroes.Models;
using Heroes.Models.AbilityTalents;
using HeroesData.FileWriter.Writers.HeroData;
using System;

namespace HeroesData.FileWriter.Writers.UnitData
{
    internal abstract class UnitDataWriter<T, TU> : HeroDataWriter<T, TU, Unit>
        where T : class
        where TU : class
    {
        protected UnitDataWriter(FileOutputType fileOutputType)
            : base(nameof(UnitData), fileOutputType)
        {
        }

        protected override void AddLocalizedGameString(Unit unit)
        {
            base.AddLocalizedGameString(unit);

            GameStringWriter.AddUnitDamageType(unit.Id, unit.DamageType);
        }

        protected override void AddLocalizedGameString(AbilityTalentBase abilityTalentBase)
        {
            base.AddLocalizedGameString(abilityTalentBase);
        }

        protected override T GetPortraitObject(Hero hero)
        {
            throw new NotImplementedException();
        }

        protected override T GetRatingsObject(Hero hero)
        {
            throw new NotImplementedException();
        }

        //protected override T GetSubAbilitiesObject(ILookup<string, Ability> linkedAbilities)
        //{
        //    throw new NotImplementedException();
        //}

        protected override T GetTalentsObject(Hero hero)
        {
            throw new NotImplementedException();
        }

        protected override TU TalentInfoElement(Talent talent)
        {
            throw new NotImplementedException();
        }
    }
}
