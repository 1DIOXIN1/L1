using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Utilities.InputManagement;
using Cinemachine;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public sealed class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform lookPivot;
        [SerializeField] private Transform gameplayVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera gameplayVirtualCameraComponent;
        [SerializeField] private Camera lookCamera;
        [SerializeField] private CinemachineBrain lookBrain;

        private Player _player;
        private IInputService _input;
        private PlayerConfig _config;
        private PlayerCameraController _controller;
        private Vector3 _offsetVelocity;
        private float _currentFov;

        public Transform LookPivot => lookPivot;
        public Transform GameplayVirtualCamera => gameplayVirtualCamera;
        public CinemachineVirtualCamera GameplayVirtualCameraComponent => gameplayVirtualCameraComponent;
        public Camera LookCamera => lookCamera;
        public CinemachineBrain LookBrain => lookBrain;
        public Player Player => _player;
        public bool IsAiming { get; private set; }

        public void Initialize(Player player, IInputService input, PlayerConfig config)
        {
            _player = player;
            _input = input;
            _config = config;
            _controller = new PlayerCameraController(player.transform, lookPivot, config);
            _currentFov = config.CameraDefaultFov;
            ConfigureForGameplay();
        }

        public void SyncFromTransforms()
        {
            _controller?.SyncFromBody();
        }

        public void ConfigureForGameplay()
        {
            if (lookPivot == null)
                return;

            if (lookBrain != null)
                lookBrain.enabled = false;

            if (lookCamera != null)
            {
                lookCamera.enabled = true;
                lookCamera.transform.SetParent(lookPivot, false);
                lookCamera.transform.localPosition = _config != null
                    ? _config.CameraDefaultOffset
                    : new Vector3(0.35f, 0.2f, -2.5f);
                lookCamera.transform.localRotation = Quaternion.identity;
                lookCamera.fieldOfView = _currentFov > 0f ? _currentFov : 60f;
            }

            if (gameplayVirtualCameraComponent != null)
                gameplayVirtualCameraComponent.enabled = false;

            if (gameplayVirtualCamera != null)
                gameplayVirtualCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_controller == null || _input == null || _player == null || _player.IsControlLocked)
                return;

            IsAiming = _input.IsAimHeld;
            _controller.Tick(
                _input.LookDelta,
                IsAiming,
                _input.IsShootHeld,
                _player.PlanarMoveDirection,
                Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (_controller == null || _player == null || _player.IsControlLocked)
                return;

            _controller.Apply();
            UpdateCameraRig(Time.deltaTime);
        }

        private void UpdateCameraRig(float deltaTime)
        {
            if (lookCamera == null || _config == null)
                return;

            Vector3 targetOffset = IsAiming ? _config.CameraAimOffset : _config.CameraDefaultOffset;
            lookCamera.transform.localPosition = Vector3.SmoothDamp(
                lookCamera.transform.localPosition,
                targetOffset,
                ref _offsetVelocity,
                _config.CameraOffsetSmoothTime);

            float targetFov = IsAiming ? _config.CameraAimFov : _config.CameraDefaultFov;
            _currentFov = Mathf.Lerp(_currentFov, targetFov, 1f - Mathf.Exp(-_config.CameraFovLerpSpeed * deltaTime));
            lookCamera.fieldOfView = _currentFov;
        }
    }
}
