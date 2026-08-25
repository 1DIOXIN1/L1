namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.AttackBehaviors
{
    public interface IEnemyAttackBehavior
    {
        void Tick(float deltaTime);
        void Reset();
    }
}
