using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public sealed class PlayerCameraController
    {
        private readonly Transform _body;
        private readonly Transform _lookPivot;
        private readonly PlayerConfig _config;

        private float _cameraYaw;
        private float _bodyYaw;
        private float _pitch;

        public PlayerCameraController(
            Transform body,
            Transform lookPivot,
            PlayerConfig config)
        {
            _body = body;
            _lookPivot = lookPivot;
            _config = config;
            _cameraYaw = _body.eulerAngles.y;
            _bodyYaw = _cameraYaw;
            _pitch = NormalizePitch(_lookPivot.localEulerAngles.x);
            Apply();
        }

        public float CameraYaw => _cameraYaw;

        public void Tick(
            Vector2 lookDelta,
            bool isAiming,
            bool isShooting,
            Vector3 planarMoveDirection,
            float deltaTime)
        {
            _cameraYaw += lookDelta.x * _config.LookSensitivity;
            _pitch = Mathf.Clamp(
                _pitch - lookDelta.y * _config.LookSensitivity,
                _config.MinPitch,
                _config.MaxPitch);

            UpdateBodyYaw(isAiming, isShooting, planarMoveDirection, deltaTime);
            Apply();
        }

        public void Apply()
        {
            _body.rotation = Quaternion.Euler(0f, _bodyYaw, 0f);

            float relativeYaw = Mathf.DeltaAngle(_bodyYaw, _cameraYaw);
            _lookPivot.localRotation = Quaternion.Euler(_pitch, relativeYaw, 0f);
        }

        public void SyncFromBody()
        {
            _bodyYaw = _body.eulerAngles.y;
            float localYaw = _lookPivot.localEulerAngles.y;
            if (localYaw > 180f)
                localYaw -= 360f;
            _cameraYaw = _bodyYaw + localYaw;
            _pitch = NormalizePitch(_lookPivot.localEulerAngles.x);
            Apply();
        }

        private void UpdateBodyYaw(
            bool isAiming,
            bool isShooting,
            Vector3 planarMoveDirection,
            float deltaTime)
        {
            if (isAiming || isShooting)
            {
                _bodyYaw = Mathf.MoveTowardsAngle(
                    _bodyYaw,
                    _cameraYaw,
                    _config.AimBodyTurnSpeed * deltaTime);
                return;
            }

            if (planarMoveDirection.sqrMagnitude < 0.01f)
                return;

            float moveYaw = Mathf.Atan2(planarMoveDirection.x, planarMoveDirection.z) * Mathf.Rad2Deg;
            _bodyYaw = Mathf.MoveTowardsAngle(
                _bodyYaw,
                moveYaw,
                _config.HipBodyTurnSpeed * deltaTime);
        }

        private static float NormalizePitch(float eulerX)
        {
            if (eulerX > 180f)
                eulerX -= 360f;

            return eulerX;
        }
    }
}
