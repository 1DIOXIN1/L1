using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public interface IEnemyAttackPresentation
    {
        ParticleSystem FlameParticlesSource { get; }

        void NotifyAttackStarted();
        void NotifyAttackFired();
        void NotifyAttackStopped();
    }
}
