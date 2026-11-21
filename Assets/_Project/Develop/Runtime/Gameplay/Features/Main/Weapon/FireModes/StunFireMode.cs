using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public sealed class StunFireMode : IFireMode
    {
        private readonly DamageableRegistry _targets;

        public StunFireMode(DamageableRegistry targets) => _targets = targets;

        public void Fire(WeaponFireContext context)
        {
            Ray ray = context.AimCamera != null
                ? context.AimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f))
                : new Ray(context.FirePoint.position, context.FirePoint.forward);

            float range = context.Config.Range;
            Vector3 end = ray.GetPoint(range);
            if (Physics.Raycast(ray, out RaycastHit hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                end = hit.point;
                if (_targets.TryGet(hit.collider, out IDamageble target) && target is EnemyBase enemy)
                    enemy.ApplyStun(context.Config.StunDuration);
            }

            context.Tracers.Play(context.FirePoint.position, end, context.AimCamera);
        }
    }
}
