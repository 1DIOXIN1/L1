using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponFireType
{
    public class ProjectileShoot : MonoBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;
        private float _speed;
        private int _damage;
        private GameObject _owner;

        public void Initialize(
            Vector3 direction,
            float speed,
            int damage,
            float lifeTime,
            GameObject owner)
        {
            _speed = speed;
            _damage = damage;
            _owner = owner;
            
            rigidbody.useGravity = false;
            rigidbody.velocity = direction.normalized * speed;
            
            Destroy(gameObject, lifeTime);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (DamageUtility.TryApplyDamage(other, _damage, _owner) == false)
                return;

            Destroy(gameObject);
        }
    }
}
