using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Presentation
{
    public static class EnemyAnimatorParameters
    {
        public const string Speed = "Speed";
        public const string IsMoving = "IsMoving";
        public const string IsAlerted = "IsAlerted";
        public const string AlertLevel = "AlertLevel";
        public const string State = "State";
        public const string Attack = "Attack";
        public const string AttackFire = "AttackFire";
        public const string Hit = "Hit";
        public const string Die = "Die";

        public static readonly int SpeedHash = Animator.StringToHash(Speed);
        public static readonly int IsMovingHash = Animator.StringToHash(IsMoving);
        public static readonly int IsAlertedHash = Animator.StringToHash(IsAlerted);
        public static readonly int AlertLevelHash = Animator.StringToHash(AlertLevel);
        public static readonly int StateHash = Animator.StringToHash(State);
        public static readonly int AttackHash = Animator.StringToHash(Attack);
        public static readonly int AttackFireHash = Animator.StringToHash(AttackFire);
        public static readonly int HitHash = Animator.StringToHash(Hit);
        public static readonly int DieHash = Animator.StringToHash(Die);
    }
}
