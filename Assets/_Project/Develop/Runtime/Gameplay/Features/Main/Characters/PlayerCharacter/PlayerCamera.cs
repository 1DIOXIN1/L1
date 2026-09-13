using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Utilities.InputManagement;
using Cinemachine;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public sealed class PlayerCamera : MonoBehaviour
    {
        private static readonly Vector3 GameplayLocalPosition = new(0f, 0.15f, -2.5f);

        [SerializeField] private Transform lookPivot;
        [SerializeField] private Transform gameplayVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera gameplayVirtualCameraComponent;
        [SerializeField] private Camera lookCamera;
        [SerializeField] private CinemachineBrain lookBrain;

        private Player _player;
        private IInputService _input;
        private PlayerCameraController _controller;

        public Transform LookPivot => lookPivot;
        public Transform GameplayVirtualCamera => gameplayVirtualCamera;
        public CinemachineVirtualCamera GameplayVirtualCameraComponent => gameplayVirtualCameraComponent;
        public Camera LookCamera => lookCamera;
        public CinemachineBrain LookBrain => lookBrain;
        public Player Player => _player;

        public void Bind(Player player, IInputService input, PlayerConfig config)
        {
            _player = player;
            _input = input;
            _controller = new PlayerCameraController(player.transform, lookPivot, config);
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
                lookCamera.transform.SetParent(lookPivot, false);
                lookCamera.transform.localPosition = GameplayLocalPosition;
                lookCamera.transform.localRotation = Quaternion.identity;
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

            _controller.Tick(_input.LookDelta);
        }

        private void LateUpdate()
        {
            if (_controller == null || _player == null || _player.IsControlLocked)
                return;

            _controller.Apply();
        }
    }
}
