using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.Meta.Features.Player;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameMode
    {
        public event Action<MissionResult> MissionEnded;

        private readonly EnemyAIService _enemyAIService;
        private readonly List<IMissionObjective> _objectives = new();

        private Player _player;
        private WeaponInventory _weaponInventory;
        private bool _isFinished;

        public GameMode(EnemyAIService enemyAIService)
        {
            _enemyAIService = enemyAIService;
        }

        public void RegisterPlayer(Player player, WeaponInventory weaponInventory)
        {
            _player = player;
            _weaponInventory = weaponInventory;
        }

        public void CapturePlayerState(PlayerStateService playerStateService)
        {
            if (_player == null || _weaponInventory == null)
                return;

            playerStateService.CaptureFrom(_player, _weaponInventory);
        }

        public void TriggerDefeat()
        {
            Complete(MissionEndReason.PlayerDied);
        }

        public void Start()
        {
            _isFinished = false;
            StopObjectives();

            _enemyAIService.MarkSpawnComplete();

            var clearEnemies = new ClearAllEnemiesObjective(_enemyAIService);
            clearEnemies.Completed += OnObjectiveCompleted;
            _objectives.Add(clearEnemies);

            foreach (IMissionObjective objective in _objectives)
                objective.Start();
        }

        private void OnObjectiveCompleted()
        {
            if (_isFinished)
                return;

            for (int i = 0; i < _objectives.Count; i++)
            {
                if (_objectives[i].IsComplete == false)
                    return;
            }

            Complete(MissionEndReason.ObjectivesComplete);
        }

        private void Complete(MissionEndReason reason)
        {
            if (_isFinished)
                return;

            _isFinished = true;
            StopObjectives();
            _player?.SetControlMode(PlayerControlMode.Locked);

            MissionEnded?.Invoke(new MissionResult(reason));
        }

        private void StopObjectives()
        {
            foreach (IMissionObjective objective in _objectives)
            {
                objective.Completed -= OnObjectiveCompleted;
                objective.Stop();
            }

            _objectives.Clear();
        }

        // Sequence minigame rules (legacy, disabled for shooter sortie loop):
        //
        // private void OnRightSequence()
        // {
        //     _walletService.Add(CurrencyTypes.Gold, _configsProviderService.GetConfig<StartWalletConfig>().ValueToAdd);
        //     Complete(MissionEndReason.ObjectivesComplete);
        // }
        //
        // private void OnWrongSequence()
        // {
        //     var valueToSpend = _configsProviderService.GetConfig<StartWalletConfig>().ValueToSpend;
        //
        //     if (_walletService.Enough(CurrencyTypes.Gold, valueToSpend))
        //         _walletService.Spend(CurrencyTypes.Gold, valueToSpend);
        //
        //     Complete(MissionEndReason.PlayerDied);
        // }
    }
}
