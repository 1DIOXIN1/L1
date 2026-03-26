using System.Collections;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.UI;
using _Project.Develop.Runtime.UI.CommonViews;
using _Project.Develop.Runtime.UI.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
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
        
        private PlayerDataProvider _playerDataProvider;
        private ResetProgressService _resetProgressService;
        
        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container =  container;
            
            MainMenuContextRegistrations.Process(container);
        }
        
        public override IEnumerator Initialize()
        {
            _coroutinesPerformer = _container.Resolve<CoroutinesPerformer>();
            _playerDataProvider = _container.Resolve<PlayerDataProvider>();
            _resetProgressService = _container.Resolve<ResetProgressService>();
            _input = _container.Resolve<IInputService>();
            
            yield return _container.Resolve<GameplayDataProvider>().Load();
            
            
            _input.SelectFirstMode += OnSelectFirstMode;
            _input.SelectSecondMode += OnSelectSecondMode;
            _input.ResetPressed += OnResetPressed;
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
        
        private void OnResetPressed()
        {
            _resetProgressService.TryReset();
        }

        private void Disable()
        {
            _coroutinesPerformer.StartPerform(_playerDataProvider.Save());
            
            _input.SelectFirstMode -= OnSelectFirstMode;
            _input.SelectSecondMode -= OnSelectSecondMode;
            _input.ResetPressed -= OnResetPressed;
        }
    }
}