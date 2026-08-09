namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public interface IEnemyState
    {
        EnemyStateId Id { get; }

        void Enter(EnemyContext context);

        void Exit(EnemyContext context);

        void Tick(EnemyContext context, float deltaTime);
    }
}
