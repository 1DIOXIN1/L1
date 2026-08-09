using System.Collections;
using _Project.Develop.Runtime.Infrastructure;
using _Project.Develop.Runtime.Infrastructure.DI;
using _Project.Develop.Runtime.Meta.Features.Progress;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using _Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private IInputService _input;
        private bool _isRunning;

        private ResetProgressService _resetProgressService;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
            MainMenuContextRegistrations.Process(container);
        }

        public override IEnumerator Initialize()
        {
            _resetProgressService = _container.Resolve<ResetProgressService>();
            _input = _container.Resolve<IInputService>();

            yield return _container.Resolve<GameplayDataProvider>().Load();

            _input.ResetPressed += OnResetPressed;
        }

        public override void Run()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (_input is KeyboardInputService keyboardInput)
                keyboardInput.SetContext(InputContext.Menu);

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

        private void OnResetPressed()
        {
            _resetProgressService.TryReset();
        }

        private void OnDestroy()
        {
            if (_input != null)
                _input.ResetPressed -= OnResetPressed;
        }
    }
}
