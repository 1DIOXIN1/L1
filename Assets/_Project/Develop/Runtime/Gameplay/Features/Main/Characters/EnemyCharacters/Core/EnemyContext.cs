using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core
{
    public sealed class EnemyContext
    {
        public EnemyBase Enemy { get; }
        public Transform Player { get; }
        public EnemyConfig Config { get; }
        public EnemyPreset Preset { get; }
        public NavMeshAgent Agent { get; }
        public EnemyAIService AIService { get; }
        public GameObject ProjectilePrefab { get; }
        public IReadOnlyList<Transform> PatrolPoints { get; }

        public int PatrolPointIndex { get; set; }
        public float PatrolWaitTimer { get; set; }
        public float DetectingTimer { get; set; }
        public bool InfiltrationTriggered { get; set; }
        public bool IsSpotter { get; set; }
        public float SpotterTimer { get; set; }
        public bool AlarmSpreadTriggered { get; set; }

        public EnemyContext(
            EnemyBase enemy,
            Transform player,
            EnemyConfig config,
            EnemyPreset preset,
            NavMeshAgent agent,
            EnemyAIService aiService,
            GameObject projectilePrefab,
            IReadOnlyList<Transform> patrolPoints)
        {
            Enemy = enemy;
            Player = player;
            Config = config;
            Preset = preset;
            Agent = agent;
            AIService = aiService;
            ProjectilePrefab = projectilePrefab;
            PatrolPoints = patrolPoints;
        }
    }
}
