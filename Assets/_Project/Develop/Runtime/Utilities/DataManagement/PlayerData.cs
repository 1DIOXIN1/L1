using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Meta.Features.Wallet;

namespace _Project.Develop.Runtime.Utilities.DataManagement
{
    public class PlayerData : ISaveData
    {
        public Dictionary<CurrencyTypes, int> WalletData;
        public int Health;
        public SlotWeaponType SelectedWeaponSlot;
        public Dictionary<WeaponType, int> AmmoByWeapon;
        public Dictionary<WeaponType, int> ReserveAmmoByWeapon;
    }
}
