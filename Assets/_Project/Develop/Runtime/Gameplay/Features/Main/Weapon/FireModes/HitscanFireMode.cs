using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.FireModes
{
    public sealed class HitscanFireMode : IFireMode
    {
        public void Fire(WeaponFireContext context)
        {
            Vector3 origin;
            Vector3 direction;

            if (context.AimCamera != null)
            {
                Ray aimRay = context.AimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                origin = aimRay.origin;
                direction = ApplySpread(aimRay.direction, context);
            }
            else if (context.FirePoint != null)
            {
                origin = context.FirePoint.position;
                direction = ApplySpread(context.FirePoint.forward, context);
            }
            else
            {
                return;
            }

            RaycastShoot.Shoot(
                origin,
                direction,
                context.Config.Range,
                context.Config.Damage,
                context.Attacker,
                context.HitImpacts,
                out Vector3 endPoint);

            Vector3 tracerStart = context.FirePoint != null
                ? context.FirePoint.position
                : origin;
            context.Tracers.Play(tracerStart, endPoint, context.AimCamera);
        }

        private static Vector3 ApplySpread(Vector3 direction, WeaponFireContext context)
        {
            float spreadDegrees = context.CurrentSpreadDegrees;
            if (spreadDegrees <= 0.001f)
                return direction.normalized;

            float climbBias = context.Config.SpreadClimbBias;
            float sideRatio = context.Config.SpreadSideRatio;

            float climb = spreadDegrees * Mathf.Max(0f, climbBias);
            float side = spreadDegrees * Mathf.Clamp01(sideRatio);
            float pitchJitter = side * 0.45f;

            float yawDegrees = Random.Range(-side, side);
            float pitchDegrees = climb + Random.Range(-pitchJitter, pitchJitter);

            Vector3 forward = direction.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            if (right.sqrMagnitude < 0.0001f)
                right = Vector3.right;
            else
                right.Normalize();
            Vector3 up = Vector3.Cross(forward, right);

            Quaternion yaw = Quaternion.AngleAxis(yawDegrees, up);
            Quaternion pitch = Quaternion.AngleAxis(-pitchDegrees, right);
            return (yaw * pitch * forward).normalized;
        }
    }
}
