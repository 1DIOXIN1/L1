using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public sealed class HitscanFireMode : IFireMode
    {
        public void Fire(WeaponFireContext context)
        {
            if (context.FirePoint == null || context.Config == null)
                return;

            Vector3 origin = context.FirePoint.position;
            Vector3 direction = ResolveAimDirection(context, origin);

            RaycastShoot.Shoot(
                origin,
                direction,
                context.Config.Range,
                context.Config.Damage,
                context.Owner);
        }

        private static Vector3 ResolveAimDirection(WeaponFireContext context, Vector3 origin)
        {
            if (context.AimCamera == null)
                return context.FirePoint.forward;

            Ray aimRay = context.AimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 aimPoint = aimRay.GetPoint(context.Config.Range);
            if (Physics.Raycast(aimRay, out RaycastHit hit, context.Config.Range, ~0, QueryTriggerInteraction.Ignore))
                aimPoint = hit.point;

            Vector3 direction = aimPoint - origin;
            if (direction.sqrMagnitude < 0.0001f)
                return context.AimCamera.transform.forward;

            return direction.normalized;
        }
    }
}
