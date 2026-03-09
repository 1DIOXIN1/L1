using System;
using System.Collections;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        
        private GameplayInputArgs _gameplayInputArgs;
        
        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container =  container;

            if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)}");

            _gameplayInputArgs = gameplayInputArgs;
            
            GameplayContextRegistrations.Process(container, _gameplayInputArgs);
        }
        
        public override IEnumerator Initialize()
        {
            Debug.Log(_gameplayInputArgs.GameplayType);

            yield break;
        }

        public override void Run()
        {
            
        }
    }
}