using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Characters.Player
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Player", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject, ICharacterConfig
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public float Speed { get; private set; } = 4f;
    }
}
