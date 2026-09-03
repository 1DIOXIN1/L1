using System.Threading.Tasks;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    [RequireComponent(typeof(Collider))]
    public sealed class Bed : MonoBehaviour
    {
        [SerializeField] private CutsceneConfig cutscene;
        [SerializeField] private Transform sleepPoint;

        private ICutsceneService _cutscenes;
        private Player _player;
        private bool _used;

        public void Construct(ICutsceneService cutscenes, Player player)
        {
            _cutscenes = cutscenes;
            _player = player;

            if (cutscene != null)
                _cutscenes.Register(cutscene);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_used || _cutscenes == null || _player == null || cutscene == null)
                return;

            if (_cutscenes.IsPlaying)
                return;

            if (IsPlayerCollider(other) == false)
                return;

            _used = true;
            _ = PlayAsync();
        }

        private async Task PlayAsync()
        {
            PlayerControlMode previousMode = _player.ControlMode;
            _player.SetControlMode(PlayerControlMode.Locked);

            CharacterController characterController = _player.CharacterController;
            bool wasEnabled = characterController != null && characterController.enabled;
            if (characterController != null)
                characterController.enabled = false;

            try
            {
                Transform point = sleepPoint != null ? sleepPoint : transform;
                _player.transform.SetPositionAndRotation(point.position, point.rotation);
                await _cutscenes.Play(cutscene.Id);
            }
            finally
            {
                if (characterController != null)
                    characterController.enabled = wasEnabled;

                if (_player != null)
                    _player.SetControlMode(previousMode);
            }
        }

        private bool IsPlayerCollider(Collider other)
        {
            Transform root = _player.transform;
            return other.transform == root || other.transform.IsChildOf(root);
        }
    }
}
