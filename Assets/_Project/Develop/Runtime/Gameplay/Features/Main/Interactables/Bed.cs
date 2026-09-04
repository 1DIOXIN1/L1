using System.Threading.Tasks;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class Bed : Interactable
    {
        [SerializeField] private CutsceneConfig cutscene;
        [SerializeField] private Transform sleepPoint;

        private ICutsceneService _cutscenes;
        private Player _player;
        private bool _used;

        public override void Construct(InteractionSetup setup)
        {
            _cutscenes = setup.Cutscenes;
            _player = setup.Player;

            if (cutscene != null)
                _cutscenes.Register(cutscene);
        }

        public override bool CanInteract()
        {
            return _used == false
                   && _cutscenes != null
                   && _player != null
                   && cutscene != null
                   && _cutscenes.IsPlaying == false;
        }

        public override void Interact()
        {
            if (CanInteract() == false)
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
                await _cutscenes.Play(cutscene.Id, point);
            }
            finally
            {
                if (characterController != null)
                    characterController.enabled = wasEnabled;

                if (_player != null)
                    _player.SetControlMode(previousMode);
            }
        }
    }
}
