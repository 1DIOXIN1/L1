using System;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class EnemyAttackBehaviorFactory
    {
        public IEnemyAttackBehavior Create(
            EnemyAttackBehaviorConfig config,
            EnemyBase enemy,
            IEnemyAttackPresentation presentation)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));
            if (presentation == null)
                throw new ArgumentNullException(nameof(presentation));

            return config switch
            {
                RangedBurstBehaviorConfig ranged => new RangedBurstAttackBehavior(enemy, ranged, presentation),
                MeleeDashBehaviorConfig melee => new MeleeDashAttackBehavior(enemy, melee, presentation),
                FlameConeBehaviorConfig flame => new FlameConeAttackBehavior(enemy, flame, presentation),
                _ => throw new InvalidOperationException(
                    $"Unknown attack behavior config type: {config.GetType().Name}")
            };
        }
    }
}
