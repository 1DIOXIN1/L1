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
        [field: SerializeField] public EnemyType DefaultType { get; private set; } = EnemyType.Guard;
        [field: SerializeField] public float DefaultViewRadius { get; private set; } = 20f;
        [field: SerializeField] public float DefaultViewAngle { get; private set; } = 120f;
        [field: SerializeField] public float DetectionFillTime { get; private set; } = 3f;
        [field: SerializeField] public float ForgetTime { get; private set; } = 7f;
        [field: SerializeField] public float SearchDuration { get; private set; } = 10f;
        [field: SerializeField] public float HelpCallDelay { get; private set; } = 20f;

        [SerializeField] private List<EnemyPreset> presets = new()
        {
            EnemyPreset.CreateGuard(),
            EnemyPreset.CreateTeacher(),
            EnemyPreset.CreateCutter()
        };

        public int Health => DefaultPreset.Health;
        public float Speed => DefaultPreset.Speed;
        public EnemyPreset DefaultPreset => GetPreset(DefaultType);

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
                EnemyType.Teacher => EnemyPreset.CreateTeacher(),
                EnemyType.Cutter => EnemyPreset.CreateCutter(),
                _ => EnemyPreset.CreateGuard()
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
        [field: SerializeField] public float CombatSpeed { get; private set; }
        [field: SerializeField] public float ViewRadius { get; private set; }
        [field: SerializeField] public float ViewAngle { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float MinAttackDistance { get; private set; }
        [field: SerializeField] public float MaxAttackDistance { get; private set; }
        [field: SerializeField] public float AttackCooldown { get; private set; }
        [field: SerializeField] public int BurstShots { get; private set; }
        [field: SerializeField] public float BurstInterval { get; private set; }
        [field: SerializeField] public float Accuracy { get; private set; }
        [field: SerializeField] public float LaserAimTime { get; private set; }
        [field: SerializeField] public int LaserDamagePerSecond { get; private set; }
        [field: SerializeField] public float LaserPowerShotTime { get; private set; }
        [field: SerializeField] public int LaserPowerShotDamage { get; private set; }
        [field: SerializeField] public float DashStartDistance { get; private set; }
        [field: SerializeField] public float DashSpeed { get; private set; }
        [field: SerializeField] public float DashCooldown { get; private set; }
        [field: SerializeField] public int ComboHits { get; private set; }
        [field: SerializeField] public float ComboDuration { get; private set; }
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public float ProjectileLifeTime { get; private set; }
        [field: SerializeField] public float AttackOriginHeight { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; }

        public float Speed => PatrolSpeed;

        public EnemyPreset(
            EnemyType type,
            string id,
            int health,
            float patrolSpeed,
            float combatSpeed,
            float viewRadius,
            float viewAngle,
            int damage,
            float minAttackDistance,
            float maxAttackDistance,
            float attackCooldown,
            int burstShots = 1,
            float burstInterval = 0.1f,
            float accuracy = 1f,
            float laserAimTime = 0f,
            int laserDamagePerSecond = 0,
            float laserPowerShotTime = 0f,
            int laserPowerShotDamage = 0,
            float dashStartDistance = 0f,
            float dashSpeed = 0f,
            float dashCooldown = 0f,
            int comboHits = 1,
            float comboDuration = 0f,
            float projectileSpeed = 16f,
            float projectileLifeTime = 4f,
            float attackOriginHeight = 1f,
            float rotationSpeed = 12f)
        {
            Type = type;
            Id = id;
            Health = health;
            PatrolSpeed = patrolSpeed;
            CombatSpeed = combatSpeed;
            ViewRadius = viewRadius;
            ViewAngle = viewAngle;
            Damage = damage;
            MinAttackDistance = minAttackDistance;
            MaxAttackDistance = maxAttackDistance;
            AttackCooldown = attackCooldown;
            BurstShots = burstShots;
            BurstInterval = burstInterval;
            Accuracy = accuracy;
            LaserAimTime = laserAimTime;
            LaserDamagePerSecond = laserDamagePerSecond;
            LaserPowerShotTime = laserPowerShotTime;
            LaserPowerShotDamage = laserPowerShotDamage;
            DashStartDistance = dashStartDistance;
            DashSpeed = dashSpeed;
            DashCooldown = dashCooldown;
            ComboHits = comboHits;
            ComboDuration = comboDuration;
            ProjectileSpeed = projectileSpeed;
            ProjectileLifeTime = projectileLifeTime;
            AttackOriginHeight = attackOriginHeight;
            RotationSpeed = rotationSpeed;
        }

        public static EnemyPreset CreateGuard()
        {
            return new EnemyPreset(
                EnemyType.Guard,
                "enemy_guard",
                30,
                4f,
                6f,
                20f,
                120f,
                5,
                5f,
                12f,
                1.5f,
                3,
                0.16f,
                1f);
        }

        public static EnemyPreset CreateTeacher()
        {
            return new EnemyPreset(
                EnemyType.Teacher,
                "enemy_teacher",
                45,
                2.5f,
                2.5f,
                20f,
                120f,
                2,
                5f,
                25f,
                3f,
                laserAimTime: 1f,
                laserDamagePerSecond: 2,
                laserPowerShotTime: 3f,
                laserPowerShotDamage: 25);
        }

        public static EnemyPreset CreateCutter()
        {
            return new EnemyPreset(
                EnemyType.Cutter,
                "enemy_cutter",
                60,
                8f,
                8f,
                20f,
                120f,
                8,
                0f,
                3f,
                1.5f,
                dashStartDistance: 5f,
                dashSpeed: 15f,
                dashCooldown: 3f,
                comboHits: 3,
                comboDuration: 1.5f);
        }
    }
}
