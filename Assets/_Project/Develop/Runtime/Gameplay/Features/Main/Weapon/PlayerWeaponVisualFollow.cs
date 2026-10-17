using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class PlayerWeaponVisualFollow
    {
        private readonly Transform _pivot;
        private readonly Transform _firePoint;
        private readonly PlayerConfig _config;
        private readonly Player _player;
        private readonly PlayerCamera _camera;

        private Vector3 _smoothedPosition;
        private Quaternion _smoothedRotation;
        private float _bobPhase;
        private float _breathPhase;
        private float _sprintDip;
        private float _strafeAmount;
        private float _landingAmount;
        private Vector3 _bobOffset;
        private Vector3 _bobEuler;
        private Vector3 _recoilPosition;
        private Vector3 _recoilEuler;

        public PlayerWeaponVisualFollow(
            Transform pivot,
            Transform firePoint,
            PlayerConfig config,
            Player player,
            PlayerCamera camera)
        {
            _pivot = pivot;
            _firePoint = firePoint;
            _config = config;
            _player = player;
            _camera = camera;

            _smoothedPosition = _firePoint.position;
            _smoothedRotation = _firePoint.rotation;
            _pivot.SetPositionAndRotation(_smoothedPosition, _smoothedRotation);
        }

        public void AddRecoil()
        {
            float strength = _camera.IsAiming ? _config.WeaponAimRecoilMultiplier : 1f;

            _recoilPosition += new Vector3(
                Random.Range(-0.002f, 0.002f),
                _config.WeaponRecoilKickUp,
                -_config.WeaponRecoilKickBack) * strength;

            _recoilEuler += new Vector3(
                -_config.WeaponRecoilPitch,
                Random.Range(-_config.WeaponRecoilYaw, _config.WeaponRecoilYaw),
                0f) * strength;
        }

        public void Tick(float deltaTime)
        {
            bool isAiming = _camera.IsAiming;
            bool isSprinting = _player.IsSprinting;
            float moveSpeed = Mathf.Clamp01(_player.PlanarMoveDirection.magnitude);
            float strafe = Mathf.Clamp(_player.StrafeInput, -1f, 1f);

            float positionSpeed = isAiming
                ? _config.WeaponAimPositionFollowSpeed
                : _config.WeaponPositionFollowSpeed;
            float rotationSpeed = isAiming
                ? _config.WeaponAimRotationFollowSpeed
                : _config.WeaponRotationFollowSpeed;

            float positionT = 1f - Mathf.Exp(-Mathf.Max(0.01f, positionSpeed) * deltaTime);
            float rotationT = 1f - Mathf.Exp(-Mathf.Max(0.01f, rotationSpeed) * deltaTime);

            _smoothedPosition = Vector3.Lerp(_smoothedPosition, _firePoint.position, positionT);
            _smoothedRotation = Quaternion.Slerp(_smoothedRotation, _firePoint.rotation, rotationT);

            UpdateSprintDip(isAiming, isSprinting, deltaTime);
            UpdateBob(isAiming, isSprinting, moveSpeed, deltaTime);
            UpdateStrafe(strafe, deltaTime);
            UpdateLanding(deltaTime);
            UpdateBreath(deltaTime);
            UpdateRecoil(deltaTime);

            float breathAmp = _config.WeaponBreathAmplitude;
            if (isAiming)
                breathAmp *= _config.WeaponAimBreathMultiplier;

            Vector3 breathOffset = new Vector3(0f, Mathf.Sin(_breathPhase) * breathAmp, 0f);

            Vector3 localOffset = new Vector3(
                _bobOffset.x,
                _bobOffset.y
                - _sprintDip * _config.WeaponSprintDip
                - _landingAmount * _config.WeaponLandingDip,
                _bobOffset.z) + _recoilPosition + breathOffset;

            Quaternion localRotation = Quaternion.Euler(
                _bobEuler.x
                - _sprintDip * _config.WeaponSprintDipPitch
                - _landingAmount * _config.WeaponLandingPitch
                + _recoilEuler.x
                + Mathf.Sin(_breathPhase) * breathAmp * 25f,
                _bobEuler.y + _recoilEuler.y - _strafeAmount * _config.WeaponStrafeYawDegrees,
                _bobEuler.z + _recoilEuler.z + _strafeAmount * _config.WeaponStrafeRollDegrees);

            _pivot.SetPositionAndRotation(
                _smoothedPosition + _smoothedRotation * localOffset,
                _smoothedRotation * localRotation);
        }

        private void UpdateSprintDip(bool isAiming, bool isSprinting, float deltaTime)
        {
            float target = isAiming == false && isSprinting ? 1f : 0f;
            float t = 1f - Mathf.Exp(-Mathf.Max(0.01f, _config.WeaponSprintDipSpeed) * deltaTime);
            _sprintDip = Mathf.Lerp(_sprintDip, target, t);
        }

        private void UpdateBob(bool isAiming, bool isSprinting, float moveSpeed, float deltaTime)
        {
            if (moveSpeed < 0.05f)
            {
                float settleT = 1f - Mathf.Exp(-10f * deltaTime);
                _bobOffset = Vector3.Lerp(_bobOffset, Vector3.zero, settleT);
                _bobEuler = Vector3.Lerp(_bobEuler, Vector3.zero, settleT);
                return;
            }

            float amplitude = _config.WeaponBobAmplitude * moveSpeed;
            float frequency = _config.WeaponBobFrequency;
            float rollDegrees = _config.WeaponBobRollDegrees * moveSpeed;

            if (isSprinting && isAiming == false)
            {
                amplitude *= _config.WeaponSprintBobMultiplier;
                frequency *= _config.WeaponSprintBobFrequencyMultiplier;
                rollDegrees *= _config.WeaponSprintBobMultiplier;
            }

            if (isAiming)
            {
                amplitude *= _config.WeaponAimBobMultiplier;
                rollDegrees *= _config.WeaponAimBobMultiplier;
            }

            _bobPhase += deltaTime * frequency * Mathf.Max(moveSpeed, 0.35f);

            _bobOffset = new Vector3(
                Mathf.Sin(_bobPhase) * amplitude,
                Mathf.Abs(Mathf.Cos(_bobPhase)) * amplitude * 0.65f,
                0f);
            _bobEuler = new Vector3(
                Mathf.Sin(_bobPhase * 2f) * amplitude * 40f,
                0f,
                Mathf.Sin(_bobPhase) * rollDegrees);
        }

        private void UpdateStrafe(float strafe, float deltaTime)
        {
            float t = 1f - Mathf.Exp(-Mathf.Max(0.01f, _config.WeaponStrafeSmoothSpeed) * deltaTime);
            _strafeAmount = Mathf.Lerp(_strafeAmount, strafe, t);
        }

        private void UpdateLanding(float deltaTime)
        {
            if (_player.JustLanded)
            {
                _landingAmount = Mathf.Max(
                    _landingAmount,
                    Mathf.Lerp(0.55f, 1f, Mathf.Clamp01(_player.LandingStrength)));
            }

            float t = 1f - Mathf.Exp(-Mathf.Max(0.01f, _config.WeaponLandingReturnSpeed) * deltaTime);
            _landingAmount = Mathf.Lerp(_landingAmount, 0f, t);
        }

        private void UpdateBreath(float deltaTime)
        {
            _breathPhase += deltaTime * _config.WeaponBreathFrequency * Mathf.PI * 2f;
        }

        private void UpdateRecoil(float deltaTime)
        {
            float t = 1f - Mathf.Exp(-Mathf.Max(0.01f, _config.WeaponRecoilReturnSpeed) * deltaTime);
            _recoilPosition = Vector3.Lerp(_recoilPosition, Vector3.zero, t);
            _recoilEuler = Vector3.Lerp(_recoilEuler, Vector3.zero, t);
        }
    }
}
