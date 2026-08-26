using _Project.Develop.Runtime.Configs.Meta.Noise;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Noise
{
    public sealed class PlayerNoiseEmitter
    {
        private readonly PlayerMotor _motor;
        private readonly Transform _transform;
        private readonly NoiseService _noiseService;
        private readonly NoiseConfig _config;

        private float _emitCooldown;

        public PlayerNoiseEmitter(
            PlayerMotor motor,
            Transform transform,
            NoiseService noiseService,
            NoiseConfig config)
        {
            _motor = motor;
            _transform = transform;
            _noiseService = noiseService;
            _config = config;
        }

        public void Tick(float deltaTime)
        {
            _emitCooldown -= deltaTime;

            if (_emitCooldown > 0f)
                return;

            if (_motor.IsMovingHorizontally == false)
                return;

            float radius;
            float suspicion;
            float interval;

            if (_motor.IsCrouching)
            {
                radius = _config.CrouchHearingRadius;
                suspicion = _config.CrouchSuspicion;
                interval = _config.CrouchEmitInterval;
            }
            else if (_motor.IsSprinting)
            {
                radius = _config.RunHearingRadius;
                suspicion = _config.RunSuspicion;
                interval = _config.RunEmitInterval;
            }
            else
            {
                radius = _config.WalkHearingRadius;
                suspicion = _config.WalkSuspicion;
                interval = _config.WalkEmitInterval;
            }

            _emitCooldown = Mathf.Max(0.05f, interval);

            if (radius <= 0f)
                return;

            _noiseService.Emit(
                _transform.position,
                radius,
                new NoiseStimulus(_transform.position, suspicion, NoiseStimulusType.Footstep));
        }
    }
}
