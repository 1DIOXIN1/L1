using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Player
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public float Health { get; private set; } = 100f;
        [field: SerializeField] public float Speed { get; private set; } = 4f;
    }
}