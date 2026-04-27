using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Weapon.WeaponsConfigs
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Weapon/SmgConfig", fileName = "SmgConfig")]
    public class SmgConfig : ScriptableObject, IWeaponConfig
    {
        [field: SerializeField] public SlotWeaponType slotType = SlotWeaponType.PrimarySlot;
        
        [field: SerializeField] public int Ammo { get; private set; } = 30;
        [field: SerializeField] public float ReloadSpeed { get; private set; } = 3f;
        [field: SerializeField] public int Damage { get; private set; } = 27;
        [field: SerializeField] public float ShootSpeed { get; private set; } = 30f;
        [field: SerializeField] public float BulletLifeTime { get; private set; } = 0.8f;
    }
}