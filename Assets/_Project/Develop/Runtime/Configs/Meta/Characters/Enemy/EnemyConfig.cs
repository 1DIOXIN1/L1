using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Enemy
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Enemy", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject, ICharacterConfig
    {
        [field: SerializeField] public float InfiltrationTriggerTime { get; private set; } = 5f;
        [field: SerializeField] public float InfiltrationSpreadRadius { get; private set; } = 25f;
        [field: SerializeField] public float PatrolPointWaitTime { get; private set; } = 1.5f;
        [field: SerializeField] public float PatrolPointReachDistance { get; private set; } = 0.6f;
        [field: SerializeField] public GameObject CookerPrefab { get; private set; }
        [field: SerializeField] public GameObject RangerPrefab { get; private set; }
        [field: SerializeField] public GameObject MeleePrefab { get; private set; }

        [SerializeField] private List<EnemyPreset> presets = new()
        {
            EnemyPreset.CreateCooker(),
            EnemyPreset.CreateRanger(),
            EnemyPreset.CreateMelee()
        };

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
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: SerializeField] public float MinAttackDistance { get; private set; }
        [field: SerializeField] public float MaxAttackDistance { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float AttackCooldown { get; private set; }
        [field: SerializeField] public int BurstShots { get; private set; }
        [field: SerializeField] public float BurstInterval { get; private set; }
        [field: SerializeField] public float Accuracy { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public float ProjectileLifeTime { get; private set; }
        [field: SerializeField] public float FlameRange { get; private set; }
        [field: SerializeField] public float FlameConeAngle { get; private set; }
        [field: SerializeField] public int FlameDamagePerSecond { get; private set; }
        [field: SerializeField] public float FlameWarmupTime { get; private set; }
        [field: SerializeField] public float DashTriggerDistance { get; private set; }
        [field: SerializeField] public float DashSpeed { get; private set; }
        [field: SerializeField] public float DashCooldown { get; private set; }
        [field: SerializeField] public float AttackOriginHeight { get; private set; }
        [field: SerializeField] public float PostAttackPause { get; private set; }

        public float Speed => PatrolSpeed;

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
            float attackRange,
            float minAttackDistance,
            float maxAttackDistance,
            int damage,
            float attackCooldown,
            int burstShots = 1,
            float burstInterval = 0.15f,
            float accuracy = 0.85f,
            float projectileSpeed = 18f,
            float projectileLifeTime = 4f,
            float flameRange = 0f,
            float flameConeAngle = 0f,
            int flameDamagePerSecond = 0,
            float flameWarmupTime = 0f,
            float dashTriggerDistance = 0f,
            float dashSpeed = 0f,
            float dashCooldown = 0f,
            float attackOriginHeight = 1f,
            float postAttackPause = 0f)
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
            AttackRange = attackRange;
            MinAttackDistance = minAttackDistance;
            MaxAttackDistance = maxAttackDistance;
            Damage = damage;
            AttackCooldown = attackCooldown;
            BurstShots = burstShots;
            BurstInterval = burstInterval;
            Accuracy = accuracy;
            ProjectileSpeed = projectileSpeed;
            ProjectileLifeTime = projectileLifeTime;
            FlameRange = flameRange;
            FlameConeAngle = flameConeAngle;
            FlameDamagePerSecond = flameDamagePerSecond;
            FlameWarmupTime = flameWarmupTime;
            DashTriggerDistance = dashTriggerDistance;
            DashSpeed = dashSpeed;
            DashCooldown = dashCooldown;
            AttackOriginHeight = attackOriginHeight;
            PostAttackPause = postAttackPause;
        }

        public static EnemyPreset CreateCooker()
        {
            return new EnemyPreset(
                EnemyType.Cooker,
                "enemy_cooker",
                50,
                2.5f,
                4.5f,
                18f,
                110f,
                10f,
                360f,
                12f,
                2f,
                1f,
                8f,
                3,
                0.5f,
                flameRange: 7f,
                flameConeAngle: 70f,
                flameDamagePerSecond: 5,
                flameWarmupTime: 0.35f);
        }

        public static EnemyPreset CreateRanger()
        {
            return new EnemyPreset(
                EnemyType.Ranger,
                "enemy_ranger",
                35,
                2f,
                5f,
                22f,
                120f,
                12f,
                420f,
                16f,
                10f,
                5f,
                18f,
                6,
                1.2f,
                3,
                0.14f,
                0.8f);
        }

        public static EnemyPreset CreateMelee()
        {
            return new EnemyPreset(
                EnemyType.Melee,
                "enemy_melee",
                60,
                2.5f,
                6f,
                16f,
                140f,
                14f,
                480f,
                18f,
                2.5f,
                0f,
                2.5f,
                10,
                1f,
                dashTriggerDistance: 6f,
                dashSpeed: 9f,
                dashCooldown: 3f,
                postAttackPause: 0.8f);
        }
    }
}
