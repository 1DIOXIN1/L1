using _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetEffect;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget.GadgetsType
{
    public class Grenade : IGadget
    {
        private readonly GrenadeConfig _config;
        private readonly Transform _usePoint;
        private readonly GameObject _owner;

        private float _lastUseTime = -Mathf.Infinity;

        public Grenade(
            GrenadeConfig config,
            Transform usePoint,
            GameObject owner)
        {
            _config = config;
            _usePoint = usePoint;
            _owner = owner;

            Amount = config.StartAmount;
            Cooldown = config.Cooldown;
        }

        public int Amount { get; private set; }
        public float Cooldown { get; private set; }
        public bool CanUse => Amount > 0 && Time.time >= _lastUseTime + Cooldown;

        public void Use()
        {
            if (!CanUse)
                return;

            Amount--;
            _lastUseTime = Time.time;

            GameObject grenadeObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            grenadeObject.name = nameof(Grenade);
            grenadeObject.transform.position = _usePoint.position;
            grenadeObject.transform.localScale = Vector3.one * _config.VisualScale;

            Rigidbody rigidbody = grenadeObject.AddComponent<Rigidbody>();
            rigidbody.velocity = _usePoint.forward * _config.ThrowForce + Vector3.up * _config.UpwardForce;

            GrenadeProjectile projectile = grenadeObject.AddComponent<GrenadeProjectile>();
            projectile.Initialize(
                _owner,
                _config.Damage,
                _config.ExplosionRadius,
                _config.ExplosionForce,
                _config.FuseTime);
        }
    }
}
