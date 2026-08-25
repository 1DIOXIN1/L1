using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public sealed class HitscanFireMode : IFireMode
    {
        public void Fire(WeaponFireContext context)
        {
            if (context.FirePoint == null || context.Config == null)
                return;

            RaycastShoot.Shoot(
                context.FirePoint.position,
                context.FirePoint.forward,
                context.Config.Range,
                context.Config.Damage,
                context.Owner);
        }
    }
}
