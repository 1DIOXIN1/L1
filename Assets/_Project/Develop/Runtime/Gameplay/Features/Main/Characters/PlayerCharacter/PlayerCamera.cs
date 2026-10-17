using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
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
        private PlayerWeaponVisualFollow _weaponVisualFollow;
        private Vector3 _offsetVelocity;
        private float _currentFov;
        private Vector3 _shakePosition;
        private Vector3 _shakeEuler;

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
            _shakePosition = Vector3.zero;
            _shakeEuler = Vector3.zero;
            ConfigureForGameplay();
        }

        public void SyncFromTransforms()
        {
            _controller?.SyncFromBody();
        }

        public void SetWeaponVisualFollow(PlayerWeaponVisualFollow weaponVisualFollow)
        {
            _weaponVisualFollow = weaponVisualFollow;
        }

        public void AddFireShake()
        {
            float strength = IsAiming ? _config.CameraAimFireShakeMultiplier : 1f;
            _shakePosition += new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 1f),
                Random.Range(-0.35f, 0.15f)) * (_config.CameraFireShakePosition * strength);

            _shakeEuler += new Vector3(
                -_config.CameraFireShakeRotation,
                Random.Range(-_config.CameraFireShakeRotation, _config.CameraFireShakeRotation) * 0.65f,
                Random.Range(-_config.CameraFireShakeRotation, _config.CameraFireShakeRotation) * 0.35f) * strength;
        }

        public void ConfigureForGameplay()
        {
            if (lookBrain != null)
                lookBrain.enabled = false;

            if (lookCamera != null)
            {
                lookCamera.enabled = true;
                lookCamera.transform.SetParent(lookPivot, false);
                lookCamera.transform.localPosition = _config.CameraDefaultOffset;
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
            if (_controller == null || _player == null || _player.IsControlLocked)
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

            float deltaTime = Time.deltaTime;
            _controller.Apply();
            UpdateCameraRig(deltaTime);
            _weaponVisualFollow.Tick(deltaTime);
        }

        private void UpdateCameraRig(float deltaTime)
        {
            if (lookCamera == null)
                return;

            float shakeT = 1f - Mathf.Exp(-Mathf.Max(0.01f, _config.CameraFireShakeReturnSpeed) * deltaTime);
            _shakePosition = Vector3.Lerp(_shakePosition, Vector3.zero, shakeT);
            _shakeEuler = Vector3.Lerp(_shakeEuler, Vector3.zero, shakeT);

            Vector3 targetOffset = IsAiming ? _config.CameraAimOffset : _config.CameraDefaultOffset;
            lookCamera.transform.localPosition = Vector3.SmoothDamp(
                lookCamera.transform.localPosition,
                targetOffset + _shakePosition,
                ref _offsetVelocity,
                _config.CameraOffsetSmoothTime);
            lookCamera.transform.localRotation = Quaternion.Euler(_shakeEuler);

            float targetFov = IsAiming ? _config.CameraAimFov : _config.CameraDefaultFov;
            _currentFov = Mathf.Lerp(_currentFov, targetFov, 1f - Mathf.Exp(-_config.CameraFovLerpSpeed * deltaTime));
            lookCamera.fieldOfView = _currentFov;
        }
    }
}
