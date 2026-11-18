using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon
{
    [Flags]
    public enum WeaponAttachments { None = 0, Magazine = 1, Suppressor = 2 }

    public enum SupplyType { Medkit, EmpGrenade, EnergyDrink }

    [Serializable]
    public sealed class WorkbenchWeapon
    {
        public WeaponConfig weapon;
        public string title;
        public string category;
        public Sprite icon;
        [Min(1)] public int weaponPrice;
        [Min(1)] public int magazinePrice;
        [Min(1)] public int extraRounds;
        [Min(1)] public int suppressorPrice;
        [Range(0.1f, 1f)] public float spreadMultiplier;
        [Min(1)] public int ammoPack;
        [Min(1)] public int ammoPrice;

        public WeaponType Type => weapon.Type;

        public int MagazineSize(WeaponAttachments attachments) =>
            weapon.MagazineSize + ((attachments & WeaponAttachments.Magazine) != 0 ? extraRounds : 0);

        public float SpreadScale(WeaponAttachments attachments) =>
            (attachments & WeaponAttachments.Suppressor) != 0 ? spreadMultiplier : 1f;
    }

    [Serializable]
    public sealed class WorkbenchSupply
    {
        public SupplyType type;
        public string title;
        public Sprite icon;
        [Min(1)] public int price;
    }

    [CreateAssetMenu(menuName = "Configs/Meta/Workbench", fileName = "WorkbenchConfig")]
    public sealed class WorkbenchConfig : ScriptableObject
    {
        [SerializeField] private WorkbenchWeapon[] weapons;
        [SerializeField] private WorkbenchSupply[] supplies;
        [field: SerializeField, Min(0)] public int MissionReward { get; private set; }

        public IReadOnlyList<WorkbenchWeapon> Weapons => weapons;
        public IReadOnlyList<WorkbenchSupply> Supplies => supplies;
        public WorkbenchWeapon Find(WeaponType type) => Array.Find(weapons, entry => entry.Type == type);
    }
}
