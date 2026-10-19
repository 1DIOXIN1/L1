using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType
{
    public static class FlameConeShoot
    {
        public static int Shoot(
            Vector3 origin,
            Vector3 direction,
            float range,
            float coneAngle,
            int damage,
            IDamageble attacker)
        {
            Vector3 forward = direction.normalized;
            float halfAngle = coneAngle * 0.5f;
            Collider[] hits = Physics.OverlapSphere(origin, range);
            int damagedCount = 0;

            foreach (Collider hit in hits)
            {
                Vector3 toTarget = hit.transform.position - origin;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude < 0.01f)
                    continue;

                if (toTarget.magnitude > range)
                    continue;

                if (Vector3.Angle(forward, toTarget) > halfAngle)
                    continue;

                if (DamageUtility.TryApplyDamage(hit, damage, attacker))
                    damagedCount++;
            }

            return damagedCount;
        }
    }
}
