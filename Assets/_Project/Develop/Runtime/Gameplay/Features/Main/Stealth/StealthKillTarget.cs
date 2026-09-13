using System;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Stealth
{
    public sealed class StealthKillTarget : IInteractable
    {
        private readonly EnemyBase _enemy;
        private readonly Player _player;
        private readonly StealthKillService _stealthKills;
        private readonly int _priority;

        public StealthKillTarget(
            EnemyBase enemy,
            Player player,
            StealthKillService stealthKills,
            int priority = 10)
        {
            _enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _stealthKills = stealthKills ?? throw new ArgumentNullException(nameof(stealthKills));
            _priority = priority;
        }

        public Transform HintAnchor => _enemy.transform;
        public Transform HierarchyRoot => _enemy.transform;
        public int Priority => _priority;
        public bool IsAvailable => _enemy.IsAlive;

        public bool CanInteract()
        {
            return _stealthKills.CanStart(_player, _enemy);
        }

        public void Interact()
        {
            _stealthKills.TryStart(_player, _enemy);
        }
    }
}
