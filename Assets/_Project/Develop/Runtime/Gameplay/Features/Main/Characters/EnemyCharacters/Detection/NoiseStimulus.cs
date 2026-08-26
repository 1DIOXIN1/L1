using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection
{
    public enum NoiseStimulusType
    {
        Damage = 0,
        Gunshot = 1,
        Footstep = 2,
        ThrownObject = 3,
        Alarm = 4
    }

    public readonly struct NoiseStimulus
    {
        public NoiseStimulus(Vector3 position, float suspicionAmount, NoiseStimulusType type)
        {
            Position = position;
            SuspicionAmount = Mathf.Clamp01(suspicionAmount);
            Type = type;
        }

        public Vector3 Position { get; }
        public float SuspicionAmount { get; }
        public NoiseStimulusType Type { get; }
    }
}
