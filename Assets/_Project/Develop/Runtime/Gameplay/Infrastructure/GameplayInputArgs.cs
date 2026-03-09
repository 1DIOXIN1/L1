using _Project.Develop.Runtime.Utilities.SceneManagement;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayInputArgs : IInputSceneArgs
    {
        public GameplayInputArgs(GameplayType gameplayType)
        {
            GameplayType =  gameplayType;
        }
        
        public GameplayType GameplayType { get; }
    }
}