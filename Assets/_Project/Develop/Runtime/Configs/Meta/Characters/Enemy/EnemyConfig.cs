using _Project.Develop.Runtime.Configs.Meta.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Enemy", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject, ICharacterConfig
    {
        [field: SerializeField] public EnemyRole Role { get; private set; } = EnemyRole.Assault;
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public float Speed { get; private set; } = 4f;
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public float AttackDistance { get; private set; } = 1.5f;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField] public float RetreatHealthPercent { get; private set; } = 0.3f;
    }
}
