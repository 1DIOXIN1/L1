using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
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
    public class HubBootstrap : SceneBootstrap
    {
        [SerializeField] private PlayerSpawnPoint playerSpawnPoint;
        [SerializeField] private InteractableRegistry interactableRegistry;

        private DIContainer _container;
        private IInputService _input;
        private InteractionService _interactionService;
        private GameplayScreenPresenter _gameplayScreenPresenter;
        private bool _isRunning;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs)
        {
            _container = container;

            if (sceneArgs is not HubInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(HubInputArgs)}");

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
            _gameplayScreenPresenter.Tick();
        }

        public void LateUpdate()
        {
            if (_isRunning == false)
                return;

            _interactionService.Tick();
            _gameplayScreenPresenter.TickInteraction();
        }

        private void OnDestroy()
        {
            _isRunning = false;
            _interactionService?.Dispose();
            _interactionService = null;
            _gameplayScreenPresenter = null;
        }

        public override void Run()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _gameplayScreenPresenter = _container.Resolve<GameplayScreenPresenter>();

            CharactersFactory charactersFactory = _container.Resolve<CharactersFactory>();
            Player player = charactersFactory.CreatePlayer(playerSpawnPoint);
            PlayerCamera playerCamera = charactersFactory.PlayerCamera;

            WireInteractions(player, playerCamera);
            _gameplayScreenPresenter.AttachPlayer(player, playerCamera, _interactionService);

            _isRunning = true;
        }

        private void WireInteractions(Player player, PlayerCamera playerCamera)
        {
            ICutsceneService cutscenes = _container.Resolve<ICutsceneService>();
            cutscenes.SetPlayerBinding(player.Animator);

            InteractionConfig interactionConfig =
                _container.Resolve<ConfigsProviderService>().GetConfig<InteractionConfig>();

            _interactionService = new InteractionService(
                _input,
                interactionConfig,
                player,
                playerCamera);

            if (interactableRegistry == null)
                return;

            InteractionSetup setup = new InteractionSetup(
                cutscenes,
                player,
                _container.Resolve<StealItemService>(),
                _interactionService);

            IReadOnlyList<Interactable> interactables = interactableRegistry.Interactables;
            for (int i = 0; i < interactables.Count; i++)
                interactables[i].Construct(setup);
        }
    }
}
