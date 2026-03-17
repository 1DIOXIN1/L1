using System.Collections;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.DataManagement.Serializers;
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
        private WalletService _walletService;
        
        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container =  container;
            
            MainMenuContextRegistrations.Process(container);
        }
        
        public override IEnumerator Initialize()
        {
            _coroutinesPerformer = _container.Resolve<CoroutinesPerformer>();
            _playerDataProvider = _container.Resolve<PlayerDataProvider>();
            
            _input = _container.Resolve<IInputService>();
            
            _input.SelectFirstMode += OnSelectFirstMode;
            _input.SelectSecondMode += OnSelectSecondMode;
            
            _walletService = _container.Resolve<WalletService>();
            
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

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _walletService.Add(CurrencyTypes.Gold, 10);
                Debug.Log("Gold now:" + _walletService.GetCurrency(CurrencyTypes.Gold).Value);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (_walletService.Enough(CurrencyTypes.Gold, 10))
                {
                    _walletService.Spend(CurrencyTypes.Gold, 10);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _coroutinesPerformer.StartPerform(_playerDataProvider.Save());
                Debug.Log("Сохранение");
            }
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