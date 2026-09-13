using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public sealed class PlayerCameraController
    {
        private readonly Transform _body;
        private readonly Transform _lookPivot;
        private readonly PlayerConfig _config;

        private float _yaw;
        private float _pitch;

        public PlayerCameraController(
            Transform body,
            Transform lookPivot,
            PlayerConfig config)
        {
            _body = body;
            _lookPivot = lookPivot;
            _config = config;
            _yaw = _body.eulerAngles.y;
            _pitch = NormalizePitch(_lookPivot.localEulerAngles.x);
            Apply();
        }

        public void Tick(Vector2 lookDelta)
        {
            _yaw += lookDelta.x * _config.LookSensitivity;
            _pitch = Mathf.Clamp(
                _pitch - lookDelta.y * _config.LookSensitivity,
                _config.MinPitch,
                _config.MaxPitch);
            Apply();
        }

        public void Apply()
        {
            _body.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _lookPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        public void SyncFromBody()
        {
            _yaw = _body.eulerAngles.y;
            _pitch = NormalizePitch(_lookPivot.localEulerAngles.x);
            Apply();
        }

        private static float NormalizePitch(float eulerX)
        {
            if (eulerX > 180f)
                eulerX -= 360f;

            return eulerX;
        }
    }
}
