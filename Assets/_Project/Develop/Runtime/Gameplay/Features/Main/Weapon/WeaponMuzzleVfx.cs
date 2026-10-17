using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class WeaponMuzzleVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private ParticleSystem muzzleSmoke;
        [SerializeField] private ParticleSystem casingEject;
        [SerializeField] private MuzzleFlashPulse flashPulse;

        public ParticleSystem MuzzleFlash => muzzleFlash;
        public ParticleSystem MuzzleSmoke => muzzleSmoke;
        public ParticleSystem CasingEject => casingEject;
        public MuzzleFlashPulse FlashPulse => flashPulse;
    }
}
