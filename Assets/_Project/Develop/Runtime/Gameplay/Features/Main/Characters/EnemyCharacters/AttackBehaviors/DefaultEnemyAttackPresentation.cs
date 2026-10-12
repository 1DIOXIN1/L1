using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Presentation;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public sealed class DefaultEnemyAttackPresentation : IEnemyAttackPresentation
    {
        private readonly EnemyAnimatorPresenter _animatorPresenter;

        public DefaultEnemyAttackPresentation(EnemyAnimatorPresenter animatorPresenter)
        {
            _animatorPresenter = animatorPresenter;
        }

        public ParticleSystem FlameParticlesSource => null;

        public void NotifyAttackStarted() => _animatorPresenter.PlayAttackStarted();

        public void NotifyAttackFired() => _animatorPresenter.PlayAttackFired();

        public void NotifyAttackStopped() => _animatorPresenter.PlayAttackStopped();
    }
}
