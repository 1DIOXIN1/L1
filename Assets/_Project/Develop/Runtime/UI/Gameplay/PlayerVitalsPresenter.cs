using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class PlayerVitalsPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly PlayerStateService _playerStateService;
        private readonly ConfigsProviderService _configsProviderService;

        private Player _player;

        public PlayerVitalsPresenter(
            GameplayScreenView view,
            PlayerStateService playerStateService,
            ConfigsProviderService configsProviderService)
        {
            _view = view;
            _playerStateService = playerStateService;
            _configsProviderService = configsProviderService;
        }

        public void Initialize()
        {
            float maxHealth = Mathf.Max(1, _playerStateService.MaxHealth);
            _view.SetHealth(_playerStateService.Health / maxHealth);
            _view.SetStamina(1f);
        }

        public void AttachPlayer(Player player)
        {
            DetachPlayer();

            _player = player;
            if (_player == null)
                return;

            _player.HealthChanged += OnHealthChanged;
            OnHealthChanged(_player.CurrentHealth, _player.MaxHealth);
        }

        public void Tick()
        {
            if (_player == null)
                return;

            float maxStamina = Mathf.Max(0.01f, _player.MaxStamina);
            _view.SetStamina(_player.Stamina / maxStamina);
        }

        public void Dispose()
        {
            DetachPlayer();
        }

        private void DetachPlayer()
        {
            if (_player == null)
                return;

            _player.HealthChanged -= OnHealthChanged;
            _player = null;
        }

        private void OnHealthChanged(int current, int max)
        {
            float maxHealth = Mathf.Max(1, max);
            _view.SetHealth(current / maxHealth);
        }
    }
}
