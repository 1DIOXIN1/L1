using _Project.Develop.Runtime.Gameplay.Infrastructure;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Configs/Gameplay", fileName = "GameplayPreset")]
    public class GameplayPreset :  ScriptableObject
    {
        [field: SerializeField] public GameplayType GameplayType { get; private set; } = GameplayType.Words;
        [field: SerializeField] public int LenghtSequence { get; private set;  } = 4;
    }
}