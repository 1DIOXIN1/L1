using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public sealed class ProjectileFireMode : IFireMode
    {
        public void Fire(WeaponFireContext context)
        {
            if (context.BulletPrefab == null || context.FirePoint == null || context.Config == null)
                return;

            Vector3 direction = ResolveAimDirection(context);

            GameObject projectileObject = Object.Instantiate(
                context.BulletPrefab,
                context.FirePoint.position,
                Quaternion.LookRotation(direction));

            if (projectileObject.TryGetComponent(out ProjectileShoot projectile) == false)
            {
                Object.Destroy(projectileObject);
                return;
            }

            projectile.Initialize(
                direction,
                context.Config.ProjectileSpeed,
                context.Config.Damage,
                context.Config.BulletLifeTime,
                context.Owner);
        }

        private static Vector3 ResolveAimDirection(WeaponFireContext context)
        {
            if (context.AimCamera == null)
                return context.FirePoint.forward;

            Ray aimRay = context.AimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 aimPoint = aimRay.GetPoint(context.Config.Range);
            if (Physics.Raycast(aimRay, out RaycastHit hit, context.Config.Range, ~0, QueryTriggerInteraction.Ignore))
                aimPoint = hit.point;

            Vector3 direction = aimPoint - context.FirePoint.position;
            if (direction.sqrMagnitude < 0.0001f)
                return context.AimCamera.transform.forward;

            return direction.normalized;
        }
    }
}
