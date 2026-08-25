using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/Enemy/AttackBehaviors/FlameCone",
        fileName = "FlameConeBehaviorConfig")]
    public class FlameConeBehaviorConfig : EnemyAttackBehaviorConfig
    {
        [field: SerializeField] public float MinAttackDistance { get; private set; } = 1f;
        [field: SerializeField] public float MaxAttackDistance { get; private set; } = 8f;
        [field: SerializeField] public float FlameRange { get; private set; } = 7f;
        [field: SerializeField] public float FlameConeAngle { get; private set; } = 70f;
        [field: SerializeField] public int FlameDamagePerSecond { get; private set; } = 5;
        [field: SerializeField] public float FlameWarmupTime { get; private set; } = 0.35f;
    }
}
