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
                if (owner != null && hit.transform.root == owner.transform.root)
                    continue;

                if (hit.TryGetComponent(out IDamageble damageble) == false)
                    continue;

                damageble.TakeDamage(damage);
                return true;
            }

            return false;
        }
    }
}
