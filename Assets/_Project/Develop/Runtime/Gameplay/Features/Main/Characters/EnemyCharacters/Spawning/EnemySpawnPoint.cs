using System.Linq;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private EnemyType enemyType = EnemyType.Ranger;
        [SerializeField] private Transform[] patrolPoints;

        public EnemyType EnemyType => enemyType;
        public Transform[] PatrolPoints => patrolPoints;
        public Vector3 SpawnPosition => transform.position;

        public bool HasValidPatrolPoints()
        {
            return patrolPoints != null && patrolPoints.Any(point => point != null);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            if (patrolPoints == null)
                return;

            Gizmos.color = Color.cyan;
            foreach (Transform point in patrolPoints)
            {
                if (point == null)
                    continue;

                Gizmos.DrawWireSphere(point.position, 0.35f);
                Gizmos.DrawLine(transform.position, point.position);
            }
        }
    }
}
