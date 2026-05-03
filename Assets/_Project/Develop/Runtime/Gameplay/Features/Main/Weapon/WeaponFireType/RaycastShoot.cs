using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType
{
    public static class RaycastShoot
    {
        public static bool Shoot(
            Vector3 origin,
            Vector3 direction,
            float distance,
            int damage,
            GameObject owner)
        {
            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance) == false)
                return false;

            if (owner != null && hit.collider.transform.root == owner.transform.root)
                return false;

            if (hit.collider.TryGetComponent(out IDamageble damageble) == false)
                return false;

            damageble.TakeDamage(damage);
            return true;
        }
    }
}
