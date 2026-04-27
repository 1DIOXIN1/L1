using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon.WeaponsConfigs
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Weapon/UspConfig", fileName = "UspConfig")]
    public class UspConfig : ScriptableObject, IWeaponConfig
    {
        [field: SerializeField] public SlotWeaponType slotType = SlotWeaponType.SecondarySlot;
        [field: SerializeField] public int Ammo { get; private set; } = 12;
        [field: SerializeField] public float ReloadSpeed { get; private set;} = 5f;
        [field: SerializeField] public int Damage { get; private set;} = 10;
        [field: SerializeField] public float ShootSpeed { get; private set;} = 23f;
        [field: SerializeField] public float BulletLifeTime { get; private set; } = 0.8f;
    }
}