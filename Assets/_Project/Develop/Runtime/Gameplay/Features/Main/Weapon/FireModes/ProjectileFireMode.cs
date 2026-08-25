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

            GameObject projectileObject = Object.Instantiate(
                context.BulletPrefab,
                context.FirePoint.position,
                Quaternion.identity);

            if (projectileObject.TryGetComponent(out ProjectileShoot projectile) == false)
            {
                Object.Destroy(projectileObject);
                return;
            }

            projectile.Initialize(
                context.FirePoint.forward,
                context.Config.ProjectileSpeed,
                context.Config.Damage,
                context.Config.BulletLifeTime,
                context.Owner);
        }
    }
}
