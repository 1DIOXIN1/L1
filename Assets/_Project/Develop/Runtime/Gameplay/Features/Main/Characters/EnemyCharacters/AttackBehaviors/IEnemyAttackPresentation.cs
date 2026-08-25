using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    /// <summary>
    /// Per-enemy presentation for attacks (VFX, animation hooks).
    /// Same behavior logic can look different on different enemies.
    /// </summary>
    public interface IEnemyAttackPresentation
    {
        ParticleSystem FlameParticlesSource { get; }

        void NotifyAttackStarted();
        void NotifyAttackFired();
        void NotifyAttackStopped();
    }
}
