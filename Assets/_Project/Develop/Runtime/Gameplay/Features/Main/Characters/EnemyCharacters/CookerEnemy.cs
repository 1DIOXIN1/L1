using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class CookerEnemy : EnemyBase, IEnemyAttackPresentation
    {
        [SerializeField] private ParticleSystem flameParticles;

        protected override EnemyType Type => EnemyType.Cooker;

        public ParticleSystem FlameParticlesSource => flameParticles;

        protected override IEnemyAttackPresentation CreatePresentation() => this;

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
