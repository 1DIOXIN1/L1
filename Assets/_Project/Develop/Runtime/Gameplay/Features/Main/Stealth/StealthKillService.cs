using System.Collections;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.UI.Gameplay;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Stealth
{
    public sealed class StealthKillService
    {
        private readonly StealthKillConfig _config;
        private readonly StealthKillPresentationFactory _presentationFactory;
        private readonly CoroutinesPerformer _coroutines;
        private readonly GameplayScreenPresenter _gameplayScreen;
        private readonly CharactersFactory _charactersFactory;

        private Coroutine _routine;
        private IStealthKillPresentation _presentation;
        private Player _activePlayer;
        private EnemyBase _activeEnemy;
        private PlayerControlMode _previousMode;
        private int _healthAtStart;
        private bool _isActive;

        public StealthKillService(
            StealthKillConfig config,
            StealthKillPresentationFactory presentationFactory,
            CoroutinesPerformer coroutines,
            GameplayScreenPresenter gameplayScreen,
            CharactersFactory charactersFactory)
        {
            _config = config;
            _presentationFactory = presentationFactory;
            _coroutines = coroutines;
            _gameplayScreen = gameplayScreen;
            _charactersFactory = charactersFactory;
        }

        public bool IsActive => _isActive;

        public bool HasPresentation(EnemyBase enemy)
        {
            StealthKillPresentationConfig presentation = enemy?.Context?.Preset?.StealthKillPresentation;
            return presentation != null && presentation.HasTimeline;
        }

        public bool CanStart(Player player, EnemyBase enemy)
        {
            if (_isActive || player.IsControlLocked || enemy.IsAlive == false)
                return false;

            if (HasPresentation(enemy) == false)
                return false;

            if (enemy.Context.Awareness.Phase != DetectionPhase.Calm)
                return false;

            Vector3 toPlayer = player.transform.position - enemy.transform.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > _config.MaxDistanceSqr || toPlayer.sqrMagnitude < 0.0001f)
                return false;

            float angleFromBack = Vector3.Angle(-enemy.transform.forward, toPlayer);
            return angleFromBack <= _config.BehindAngle;
        }

        public void TryStart(Player player, EnemyBase enemy)
        {
            if (CanStart(player, enemy) == false)
                return;

            _routine = _coroutines.StartPerform(KillRoutine(player, enemy));
        }

        public void Cancel()
        {
            if (_isActive == false)
                return;

            _presentation?.Cancel();

            if (_routine != null)
            {
                _coroutines.StopPerform(_routine);
                _routine = null;
            }

            Finish(interrupted: true);
        }

        private IEnumerator KillRoutine(Player player, EnemyBase enemy)
        {
            _isActive = true;
            _activePlayer = player;
            _activeEnemy = enemy;
            _previousMode = player.ControlMode;
            _healthAtStart = player.CurrentHealth;

            player.SetControlMode(PlayerControlMode.Locked);
            player.HealthChanged += OnPlayerHealthChanged;
            enemy.SetGameplaySuspended(true);
            _gameplayScreen?.SetDetectionIconsVisible(false);
            AlignParticipants(player, enemy);

            _presentation = _presentationFactory.Create(
                enemy.Context.Preset.StealthKillPresentation,
                _config.CameraReturnBlendDuration);

            if (_presentation == null)
            {
                Finish(interrupted: true);
                yield break;
            }

            yield return _presentation.Play(player, enemy, _charactersFactory.PlayerCamera);

            if (_activeEnemy.IsAlive == false)
            {
                Finish(interrupted: true);
                yield break;
            }

            _activeEnemy.ExecuteStealthKill();
            Finish(interrupted: false);
        }

        private void OnPlayerHealthChanged(int current, int max)
        {
            if (_isActive && current < _healthAtStart)
                Cancel();
        }

        private void AlignParticipants(Player player, EnemyBase enemy)
        {
            Transform enemyTransform = enemy.transform;
            Vector3 standPosition = enemyTransform.position - enemyTransform.forward * _config.PlayerStandDistance;
            standPosition.y = player.transform.position.y;

            CharacterController characterController = player.CharacterController;
            bool wasEnabled = characterController.enabled;
            characterController.enabled = false;

            player.transform.SetPositionAndRotation(
                standPosition,
                Quaternion.LookRotation(enemyTransform.forward, Vector3.up));

            characterController.enabled = wasEnabled;
        }

        private void Finish(bool interrupted)
        {
            if (_isActive == false)
                return;

            _isActive = false;
            _routine = null;
            _presentation = null;

            _activePlayer.HealthChanged -= OnPlayerHealthChanged;
            _activePlayer.SetControlMode(_previousMode);
            _gameplayScreen?.SetDetectionIconsVisible(true);

            if (interrupted && _activeEnemy.IsAlive)
                _activeEnemy.SetGameplaySuspended(false);

            _activePlayer = null;
            _activeEnemy = null;
        }
    }
}
