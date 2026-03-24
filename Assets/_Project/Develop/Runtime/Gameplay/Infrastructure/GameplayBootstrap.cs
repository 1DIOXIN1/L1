using System;
using System.Collections;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private IInputService _input;
        private GameplayInputArgs _gameplayInputArgs;
        private bool _isRunning = false;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs)
        {
            _container = container;

            if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)}");

            _gameplayInputArgs = gameplayInputArgs;
            
            GameplayContextRegistrations.Process(container);
        }

        public override IEnumerator Initialize()
        {
            yield return _container.Resolve<GameplayDataProvider>().Load();
            
            _input = _container.Resolve<IInputService>();
            
            if (_input is Controller controller)
                controller.Enable();
        }

        public void Update()
        {
            if (_isRunning == false)
                return;

            _input.Update(Time.deltaTime);
        }

        public override void Run()
        {
            var generator = _container.Resolve<GameplaySequenceGeneratorService>();
            var config = _container.Resolve<ConfigsProviderService>().GetConfig<StartGameplayConfig>();
            var checker = _container.Resolve<SequenceChecker>();

            checker.StartCheck(generator.Generate(config.LenghtSequence, _gameplayInputArgs.GameplayType), _input);

            _container.Resolve<GameplayCycle>().StartGame(_gameplayInputArgs);

            _isRunning = true;
        }
    }
}