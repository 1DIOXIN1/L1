using System.Collections;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private CoroutinesPerformer _coroutinesPerformer;
        private IInputService _input;
        private bool _isRunning = false;
        
        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container =  container;
            
            MainMenuContextRegistrations.Process(container);
        }
        
        public override IEnumerator Initialize()
        {
            _coroutinesPerformer = _container.Resolve<CoroutinesPerformer>();
            
            _input = _container.Resolve<IInputService>();
            
            _input.SelectFirstMode += OnSelectFirstMode;
            _input.SelectSecondMode += OnSelectSecondMode;
            
            yield break;
        }

        public override void Run()
        {
            if (_input is Controller controller)
                controller.Enable();

            _isRunning = true;
        }

        private void Update()
        {
            if (_isRunning == false)
                return;
            
            _input.Update(Time.deltaTime);
        }

        private void OnSelectFirstMode()
        {
            Disable();
            
            _coroutinesPerformer.StartPerform(_container.Resolve<SceneSwitcherService>()
                .ProcessSwitchTo(Scenes.GamePlay, new GameplayInputArgs(GameplayType.Numbers)));
        }
        
        private void OnSelectSecondMode()
        {
            Disable();
            
            _coroutinesPerformer.StartPerform(_container.Resolve<SceneSwitcherService>()
                .ProcessSwitchTo(Scenes.GamePlay, new GameplayInputArgs(GameplayType.Words)));
        }

        private void Disable()
        {
            _input.SelectFirstMode -= OnSelectFirstMode;
            _input.SelectSecondMode -= OnSelectSecondMode;
        }
    }
}