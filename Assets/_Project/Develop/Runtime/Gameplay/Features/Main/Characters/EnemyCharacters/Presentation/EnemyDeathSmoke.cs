using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Presentation
{
    public static class EnemyDeathSmoke
    {
        public static void Spawn(GameObject prefab, Vector3 position)
        {
            GameObject instance = Object.Instantiate(prefab, position, prefab.transform.rotation);
            instance.GetComponent<ParticleSystem>().Play(true);
        }
    }
}
