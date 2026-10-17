using System;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class EnemyAttackBehaviorFactory
    {
        private readonly HitImpactService _hitImpacts;

        public EnemyAttackBehaviorFactory(HitImpactService hitImpacts)
        {
            _hitImpacts = hitImpacts;
        }

        public IEnemyAttackBehavior Create(
            EnemyAttackBehaviorConfig config,
            EnemyBase enemy,
            IEnemyAttackPresentation presentation)
        {
            return config switch
            {
                RangedBurstBehaviorConfig ranged => new RangedBurstAttackBehavior(
                    enemy,
                    ranged,
                    presentation,
                    _hitImpacts),
                MeleeDashBehaviorConfig melee => new MeleeDashAttackBehavior(enemy, melee, presentation),
                FlameConeBehaviorConfig flame => new FlameConeAttackBehavior(enemy, flame, presentation),
                _ => throw new InvalidOperationException(
                    $"Unknown attack behavior config type: {config.GetType().Name}")
            };
        }
    }
}
