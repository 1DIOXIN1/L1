using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public sealed class EnemyAttackAnimEvents : MonoBehaviour
    {
        [SerializeField] private EnemyBase enemy;

        public void OnAttackFire()
        {
            enemy.NotifyAttackAnimFire();
        }
    }
}
