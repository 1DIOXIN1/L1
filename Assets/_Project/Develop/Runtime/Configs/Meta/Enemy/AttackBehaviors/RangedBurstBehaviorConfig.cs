using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/Enemy/AttackBehaviors/RangedBurst",
        fileName = "RangedBurstBehaviorConfig")]
    public class RangedBurstBehaviorConfig : EnemyAttackBehaviorConfig
    {
        [field: SerializeField] public float MinAttackDistance { get; private set; } = 5f;
        [field: SerializeField] public float MaxAttackDistance { get; private set; } = 18f;
        [field: SerializeField] public int Damage { get; private set; } = 6;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1.2f;
        [field: SerializeField] public int BurstShots { get; private set; } = 3;
        [field: SerializeField] public float BurstInterval { get; private set; } = 0.14f;
        [field: SerializeField] public float Accuracy { get; private set; } = 0.8f;
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 18f;
        [field: SerializeField] public float ProjectileLifeTime { get; private set; } = 4f;
    }
}
