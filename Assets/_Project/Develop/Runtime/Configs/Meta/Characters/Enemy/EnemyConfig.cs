using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters;
using _Project.Develop.Runtime.Configs.Meta.Enemy.AttackBehaviors;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Enemy/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject, ICharacterConfig
    {
        [field: SerializeField] public float InfiltrationTriggerTime { get; private set; } = 5f;
        [field: SerializeField] public float InfiltrationSpreadRadius { get; private set; } = 25f;
        [field: SerializeField] public float PatrolPointWaitTime { get; private set; } = 1.5f;
        [field: SerializeField] public float PatrolPointReachDistance { get; private set; } = 0.6f;
        [field: SerializeField] public float SuspiciousFillTime { get; private set; } = 1.25f;
        [field: SerializeField] public float LostHoldTime { get; private set; } = 1.5f;
        [field: SerializeField] public float LostDecayTime { get; private set; } = 2f;
        [field: SerializeField, Range(0f, 1f)] public float DamageSuspicionBurst { get; private set; } = 0.45f;
        [field: SerializeField] public GameObject CookerPrefab { get; private set; }
        [field: SerializeField] public GameObject RangerPrefab { get; private set; }
        [field: SerializeField] public GameObject MeleePrefab { get; private set; }

        [SerializeField] private List<EnemyPreset> presets = new List<EnemyPreset>();

        public int Health => DefaultPreset.Health;
        public float Speed => DefaultPreset.PatrolSpeed;
        public EnemyPreset DefaultPreset => GetPreset(EnemyType.Ranger);

        public EnemyPreset GetPreset(EnemyType type)
        {
            if (presets != null)
            {
                foreach (EnemyPreset preset in presets)
                {
                    if (preset.Type == type)
                        return preset;
                }
            }

            return type switch
            {
                EnemyType.Cooker => EnemyPreset.CreateCooker(),
                EnemyType.Melee => EnemyPreset.CreateMelee(),
                _ => EnemyPreset.CreateRanger()
            };
        }

        public GameObject GetPrefab(EnemyType type)
        {
            return type switch
            {
                EnemyType.Cooker => CookerPrefab,
                EnemyType.Melee => MeleePrefab,
                EnemyType.Ranger => RangerPrefab,
                _ => RangerPrefab
            };
        }
    }

    [Serializable]
    public class EnemyPreset : ICharacterConfig
    {
        [field: SerializeField] public EnemyType Type { get; private set; }
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public float PatrolSpeed { get; private set; }
        [field: SerializeField] public float ChaseSpeed { get; private set; }
        [field: SerializeField] public float ViewRadius { get; private set; }
        [field: SerializeField] public float ViewAngle { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; }
        [field: SerializeField] public float AgentAngularSpeed { get; private set; }
        [field: SerializeField] public float AgentAcceleration { get; private set; }
        [field: SerializeField] public float AttackOriginHeight { get; private set; } = 1f;

        [Tooltip("Share the same BehaviorConfig asset for identical stats. " +
                 "Use a unique/duplicated asset to customize one enemy.")]
        [SerializeField] private EnemyAttackBehaviorConfig[] attackBehaviors = Array.Empty<EnemyAttackBehaviorConfig>();

        public IReadOnlyList<EnemyAttackBehaviorConfig> AttackBehaviors => attackBehaviors;
        public float Speed => PatrolSpeed;

        public EnemyPreset()
        {
        }

        public EnemyPreset(
            EnemyType type,
            string id,
            int health,
            float patrolSpeed,
            float chaseSpeed,
            float viewRadius,
            float viewAngle,
            float rotationSpeed,
            float agentAngularSpeed,
            float agentAcceleration,
            float attackOriginHeight = 1f)
        {
            Type = type;
            Id = id;
            Health = health;
            PatrolSpeed = patrolSpeed;
            ChaseSpeed = chaseSpeed;
            ViewRadius = viewRadius;
            ViewAngle = viewAngle;
            RotationSpeed = rotationSpeed;
            AgentAngularSpeed = agentAngularSpeed;
            AgentAcceleration = agentAcceleration;
            AttackOriginHeight = attackOriginHeight;
            attackBehaviors = Array.Empty<EnemyAttackBehaviorConfig>();
        }

        public static EnemyPreset CreateCooker()
        {
            return new EnemyPreset(
                EnemyType.Cooker, "enemy_cooker", 50, 2.5f, 4.5f, 18f, 110f, 10f, 360f, 12f);
        }

        public static EnemyPreset CreateRanger()
        {
            return new EnemyPreset(
                EnemyType.Ranger, "enemy_ranger", 35, 2f, 5f, 22f, 120f, 12f, 420f, 16f);
        }

        public static EnemyPreset CreateMelee()
        {
            return new EnemyPreset(
                EnemyType.Melee, "enemy_melee", 60, 2.5f, 6f, 16f, 140f, 14f, 480f, 18f);
        }
    }
}
