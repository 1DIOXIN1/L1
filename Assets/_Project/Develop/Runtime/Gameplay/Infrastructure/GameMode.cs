using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Infrastructure.Mission;
using _Project.Develop.Runtime.Meta.Features.Missions;
using _Project.Develop.Runtime.Meta.Features.Player;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameMode
    {
        public event Action<MissionResult> MissionEnded;

        private readonly EnemyAIService _enemyAIService;
        private readonly MissionService _missionService;
        private readonly MissionObjectiveFactory _objectiveFactory;
        private readonly MissionQuestTracker _questTracker;
        private readonly List<IMissionObjective> _optionalObjectives = new();
        private readonly List<Action> _optionalCompletedHandlers = new();

        private Player _player;
        private WeaponInventory _weaponInventory;
        private SequenceObjective _primarySequence;
        private string _missionId;
        private bool _isFinished;

        public GameMode(
            EnemyAIService enemyAIService,
            MissionService missionService,
            MissionObjectiveFactory objectiveFactory,
            MissionQuestTracker questTracker)
        {
            _enemyAIService = enemyAIService;
            _missionService = missionService;
            _objectiveFactory = objectiveFactory;
            _questTracker = questTracker;
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

        public void Start(string missionId)
        {
            if (string.IsNullOrEmpty(missionId))
                throw new ArgumentException("Mission id is required.", nameof(missionId));

            if (_missionService.TryGetMission(missionId, out MissionDefinition mission) == false)
                throw new InvalidOperationException($"Mission '{missionId}' is not configured.");

            _missionId = missionId;
            _isFinished = false;
            StopObjectives();

            _enemyAIService.MarkSpawnComplete();
            _questTracker.BeginMission(mission);

            _primarySequence = _objectiveFactory.CreatePrimarySequence(mission.PrimarySteps);
            _primarySequence.Completed += OnPrimaryCompleted;
            _primarySequence.StepCompleted += OnPrimaryStepCompleted;
            _primarySequence.StepStarted += OnPrimaryStepStarted;

            for (int i = 0; i < mission.OptionalObjectives.Count; i++)
            {
                MissionObjectiveDefinition definition = mission.OptionalObjectives[i];
                if (definition == null)
                    continue;

                IMissionObjective objective = _objectiveFactory.Create(definition);
                string objectiveId = definition.Id;
                Action handler = () => OnOptionalObjectiveCompleted(objectiveId);
                objective.Completed += handler;
                _optionalCompletedHandlers.Add(handler);
                _optionalObjectives.Add(objective);
            }

            _primarySequence.Start();

            for (int i = 0; i < _optionalObjectives.Count; i++)
                _optionalObjectives[i].Start();
        }

        private void OnPrimaryStepStarted(int stepIndex)
        {
            _questTracker.SetActivePrimary(stepIndex);
        }

        private void OnPrimaryStepCompleted(int stepIndex)
        {
            _questTracker.CompletePrimaryStep(stepIndex);
        }

        private void OnPrimaryCompleted()
        {
            _questTracker.ClearActivePrimary();
            Complete(MissionEndReason.ObjectivesComplete);
        }

        private void OnOptionalObjectiveCompleted(string objectiveId)
        {
            if (_isFinished)
                return;

            _questTracker.CompleteOptional(objectiveId);
            _missionService.CompleteOptional(_missionId, objectiveId);
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
            if (_primarySequence != null)
            {
                _primarySequence.Completed -= OnPrimaryCompleted;
                _primarySequence.StepCompleted -= OnPrimaryStepCompleted;
                _primarySequence.StepStarted -= OnPrimaryStepStarted;
                _primarySequence.Stop();
                _primarySequence = null;
            }

            for (int i = 0; i < _optionalObjectives.Count; i++)
            {
                if (i < _optionalCompletedHandlers.Count)
                    _optionalObjectives[i].Completed -= _optionalCompletedHandlers[i];

                _optionalObjectives[i].Stop();
            }

            _optionalObjectives.Clear();
            _optionalCompletedHandlers.Clear();
        }
    }
}
