using _Project.Develop.Runtime.Gameplay.Infrastructure;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Core.Gameplay
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/StartGameplayConfig", fileName = "StartGameplayConfig")]
    public class StartGameplayConfig : ScriptableObject
    {
        [field: SerializeField] public GameplayType GameplayType { get; private set; } = GameplayType.Words;
        [field: SerializeField] public int LenghtSequence { get; private set;  } = 4;
    }
}