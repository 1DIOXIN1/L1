using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public class ChefEnemy : EnemyBase, IEnemyAttackPresentation
    {
        [SerializeField] private ParticleSystem flameParticles;

        protected override EnemyType Type => EnemyType.Chef;

        public ParticleSystem FlameParticlesSource => flameParticles;

        protected override IEnemyAttackPresentation CreatePresentation() => this;

        public void NotifyAttackStarted() => AnimatorPresenter.PlayAttackStarted();

        public void NotifyAttackFired() => AnimatorPresenter.PlayAttackFired();

        public void NotifyAttackStopped() => AnimatorPresenter.PlayAttackStopped();
    }
}
