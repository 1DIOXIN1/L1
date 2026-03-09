using _Project.Develop.Runtime.Gameplay.Infrastructure;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/GameplayPreset", fileName = "NumbersPreset")]
    public class GameplayNumbersPreset : ScriptableObject
    {
        [field: SerializeField] public GameplayType GameplayType { get; } = GameplayType.Numbers;
    }
}
