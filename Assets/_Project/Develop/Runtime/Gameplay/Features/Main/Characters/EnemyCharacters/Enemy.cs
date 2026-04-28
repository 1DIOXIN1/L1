namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters
{
    public sealed class Enemy : EnemyBase
    {
        private float _attackCooldown;

        protected override void Tick(float deltaTime)
        {
            if (HasTarget() == false)
                return;

            _attackCooldown -= deltaTime;

            if (DistanceToTarget() > Config.AttackDistance)
            {
                MoveTo(Target.position, deltaTime);
                return;
            }

            if (_attackCooldown > 0f)
                return;

            if (TryDamageTarget())
                _attackCooldown = Config.AttackCooldown;
        }
    }
}
