using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    /// <summary>
    /// Default presentation hooks for enemies without custom VFX.
    /// Replace/extend per enemy for unique animations and effects.
    /// </summary>
    public sealed class DefaultEnemyAttackPresentation : IEnemyAttackPresentation
    {
        public ParticleSystem FlameParticlesSource => null;

        public void NotifyAttackStarted()
        {
        }

        public void NotifyAttackFired()
        {
        }

        public void NotifyAttackStopped()
        {
        }
    }
}
