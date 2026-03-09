using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        public static void Process(DIContainer container, IInputSceneArgs sceneArgs)
        {
            Debug.Log("Процесс регистрации сервисов на сцене геймплея");
        }
    }
}