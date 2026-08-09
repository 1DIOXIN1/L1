using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType
{
    public static class OverlapShoot
    {
        public static bool Shoot(
            Vector3 origin,
            float radius,
            int damage,
            GameObject owner)
        {
            Collider[] hits = Physics.OverlapSphere(origin, radius);

            foreach (Collider hit in hits)
            {
                if (DamageUtility.TryApplyDamage(hit, damage, owner))
                    return true;
            }

            return false;
        }
    }
}
