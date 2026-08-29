using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public interface IWeapon
    {
        event Action AmmoChanged;

        WeaponType Type { get; }
        int Ammo { get; }
        int ReserveAmmo { get; }
        int MagazineSize { get; }
        int ReserveCapacity { get; }
        bool CanShoot { get; }
        bool IsReloading { get; }
        bool IsAutomatic { get; }
        Sprite HudIconActive { get; }
        Sprite HudIconReloading { get; }
        GameObject ViewPrefab { get; }
        Vector3 ViewLocalPosition { get; }
        Vector3 ViewLocalEulerAngles { get; }
        Vector3 ViewLocalScale { get; }

        void Tick(float deltaTime);
        void Shoot();
        void Reload();
        void SetAmmo(int ammo);
        void SetReserveAmmo(int reserveAmmo);
    }
}
