using System;
using System.Collections;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Spawning;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.UI.Gameplay;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        [SerializeField] private PlayerSpawnPoint playerSpawnPoint;
        [SerializeField] private EnemySpawnRegistry enemySpawnRegistry;
        [SerializeField] private Bed[] beds;

        private DIContainer _container;
        private IInputService _input;
        private GameplayInputArgs _gameplayInputArgs;
        private GameplayScreenPresenter _gameplayScreenPresenter;
        private bool _isRunning;

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
            _input.SetContext(InputContext.Gameplay);

            if (_input is Controller controller)
                controller.Enable();
        }

        public void Update()
        {
            if (_isRunning == false)
                return;

            _input.Update(Time.deltaTime);
            _gameplayScreenPresenter?.Tick();
        }

        public override void Run()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _container.Resolve<ConfigsProviderService>().GetConfig<StartGameplayConfig>();

            _gameplayScreenPresenter = _container.Resolve<GameplayScreenPresenter>();

            CharactersFactory charactersFactory = _container.Resolve<CharactersFactory>();
            Player player = charactersFactory.CreatePlayer(playerSpawnPoint);
            _gameplayScreenPresenter.AttachPlayer(player);

            WireCutscenes(player);

            if (enemySpawnRegistry != null)
            {
                EnemySpawnService spawnService = _container.Resolve<EnemySpawnService>();
                spawnService.SpawnFromRegistry(enemySpawnRegistry);
            }

            _container.Resolve<GameplayCycle>().StartGame(_gameplayInputArgs);
            _isRunning = true;
        }

        private void WireCutscenes(Player player)
        {
            ICutsceneService cutscenes = _container.Resolve<ICutsceneService>();
            cutscenes.SetPlayerBinding(player.Animator);

            if (beds == null)
                return;

            for (int i = 0; i < beds.Length; i++)
            {
                if (beds[i] != null)
                    beds[i].Construct(cutscenes, player);
            }
        }
    }
}
