using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon
{
    public interface IWeaponConfig
    {
        public int Ammo { get; }
        public float ReloadSpeed { get; }
        public int Damage { get; }
        public float ShootSpeed { get; }
    }
}