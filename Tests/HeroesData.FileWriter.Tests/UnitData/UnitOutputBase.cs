using Heroes.Models;
using System.Collections.Generic;

namespace HeroesData.FileWriter.Tests.UnitData
{
    public class UnitOutputBase : FileOutputTestBase<Unit>
    {
        public UnitOutputBase()
            : base(nameof(UnitData))
        {
        }

        protected override void SetTestData()
        {
            Unit unit = new Unit()
            {
                Name = "Minion Archer",
                Id = "MinionArcher",
                Description = new TooltipDescription(string.Empty),
                HyperlinkId = "MinionArcher",
                Attributes = new HashSet<string>() { "att1", "att2" },
                CUnitId = "MinionArcher",
                DamageType = "Minion",
                InnerRadius = 1.125,
                Radius = 1.111,
                Sight = 6,
                Speed = 4,
                TargetInfoPanelImageFileNames = new HashSet<string>() { "image_minion_archer.dds" },
            };
            unit.Life.LifeMax = 500;
            unit.Weapons.Add(new UnitWeapon()
            {
                Damage = 56,
                Name = "Minion Archer Weapon",
                Period = 4,
                Range = 6,
                WeaponNameId = "MinionArcherWeapon",
            });

            TestData.Add(unit);

            Unit unit2 = new Unit()
            {
                Name = "Minion Footman",
                Id = "MinionFootman",
                Description = new TooltipDescription(string.Empty),
                HyperlinkId = "MinionFootman",
                CUnitId = "MinionFootman",
                DamageType = "Minion",
                InnerRadius = 1.125,
                Radius = 1.111,
                Sight = 6,
                Speed = 4,
                TargetInfoPanelImageFileNames = new HashSet<string>() { "image_minion_archer.dds", "image_ranged.dds" },
            };
            unit2.Life.LifeMax = 900;
            unit2.Weapons.Add(new UnitWeapon()
            {
                Damage = 75,
                Name = "Minion Footman Weapon",
                Period = 6,
                Range = 7,
                WeaponNameId = "MinionFootmanWeapon",
            });
            unit2.Weapons.Add(new UnitWeapon()
            {
                Damage = 56,
                Name = "Minion Archer Weapon",
                Period = 4,
                Range = 6,
                WeaponNameId = "MinionArcherWeapon",
            });

            TestData.Add(unit2);
        }
    }
}
