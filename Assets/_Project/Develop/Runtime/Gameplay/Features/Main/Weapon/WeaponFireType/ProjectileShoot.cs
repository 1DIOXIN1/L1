using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType
{
    public class ProjectileShoot : MonoBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;
        private int _damage;
        private GameObject _owner;
        private HitImpactService _hitImpacts;
        private bool _consumed;

        public void Initialize(
            Vector3 direction,
            float speed,
            int damage,
            float lifeTime,
            GameObject owner,
            HitImpactService hitImpacts = null)
        {
            _damage = damage;
            _owner = owner;
            _hitImpacts = hitImpacts;
            _consumed = false;

            rigidbody.useGravity = false;
            rigidbody.velocity = direction.normalized * speed;

            Destroy(gameObject, lifeTime);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (_consumed || other == null)
                return;

            if (_owner != null && other.transform.root == _owner.transform.root)
                return;

            _consumed = true;

            Vector3 point = other.ClosestPoint(transform.position);
            Vector3 toProjectile = transform.position - point;
            Vector3 normal = toProjectile.sqrMagnitude > 0.0001f
                ? toProjectile.normalized
                : -transform.forward;

            _hitImpacts.Play(point, normal, other);
            DamageUtility.TryApplyDamage(other, _damage, _owner);
            Destroy(gameObject);
        }
    }
}
