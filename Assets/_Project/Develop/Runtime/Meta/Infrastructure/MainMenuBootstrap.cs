using System.Collections;
using _Project.Develop.Runtime.Gameplay.Infrastructure;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using _Project.Develop.Runtime.Utilities;
using _Project.Develop.Runtime.Utilities.DataManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap, IDataReader<GameplayData>
    {
        private DIContainer _container;
        private CoroutinesPerformer _coroutinesPerformer;
        private IInputService _input;
        private bool _isRunning = false;
        
        private PlayerDataProvider _playerDataProvider;
        private GameplayDataProvider _gameplayDataProvider;
        private WalletService _walletService;
        private ResetProgressService _resetProgressService;

        private int _countWins;
        private int _countLoss;
        
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
            _walletService = _container.Resolve<WalletService>();
            _input = _container.Resolve<IInputService>();
            
            _container.Resolve<GameplayDataProvider>().RegisterReader(this);
            
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
            
            //Костыль для проверки
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log($"Wins: {_countWins}, Losses: {_countLoss}");
                Debug.Log("Gold now:" + _walletService.GetCurrency(CurrencyTypes.Gold).Value);
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

        public void ReadFrom(GameplayData data)
        {
            _countWins = data.CountWins;
            _countLoss = data.CountLoss;
        }
    }
}