using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;
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
            IDamageble attacker,
            HitImpactService hitImpacts,
            out Vector3 endPoint)
        {
            Vector3 normalized = direction.normalized;
            if (Physics.Raycast(
                    origin,
                    normalized,
                    out RaycastHit hit,
                    distance,
                    ~0,
                    QueryTriggerInteraction.Ignore))
            {
                endPoint = hit.point;
                hitImpacts.Play(hit);
                DamageUtility.TryApplyDamage(hit.collider, damage, attacker);
                return true;
            }

            endPoint = origin + normalized * distance;
            return false;
        }
    }
}
