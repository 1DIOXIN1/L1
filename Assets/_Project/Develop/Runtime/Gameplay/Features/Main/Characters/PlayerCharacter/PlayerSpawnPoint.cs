using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1.5f);
        }
    }
}
