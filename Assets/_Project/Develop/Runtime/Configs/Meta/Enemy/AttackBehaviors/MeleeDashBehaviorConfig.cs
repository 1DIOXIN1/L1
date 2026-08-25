using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/Enemy/AttackBehaviors/MeleeDash",
        fileName = "MeleeDashBehaviorConfig")]
    public class MeleeDashBehaviorConfig : EnemyAttackBehaviorConfig
    {
        [field: SerializeField] public float AttackRange { get; private set; } = 2.5f;
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField] public float DashTriggerDistance { get; private set; } = 6f;
        [field: SerializeField] public float DashSpeed { get; private set; } = 9f;
        [field: SerializeField] public float DashCooldown { get; private set; } = 3f;
        [field: SerializeField] public float PostAttackPause { get; private set; } = 0.8f;
    }
}
