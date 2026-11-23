using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Workbench
{
    internal sealed class WorkbenchCameraFocus
    {
        private const float Duration = 0.42f;
        private const float MonitorVerticalFov = 38f;
        private const float MonitorHorizontalFov = 64f;

        private readonly Camera _camera;
        private readonly Transform _pose;
        private Vector3 _homePosition;
        private Quaternion _homeRotation;
        private float _homeFov;
        private float _progress;
        private bool _active;

        public WorkbenchCameraFocus(Camera camera, Transform pose)
        {
            _camera = camera;
            _pose = pose;
        }

        public void Begin()
        {
            Transform cameraTransform = _camera.transform;
            _homePosition = cameraTransform.localPosition;
            _homeRotation = cameraTransform.localRotation;
            _homeFov = _camera.fieldOfView;
            _progress = 0f;
            _active = true;
        }

        public bool Tick(float deltaTime, bool entering)
        {
            if (!_active)
                return false;

            _progress = Mathf.MoveTowards(_progress, entering ? 1f : 0f, deltaTime / Duration);
            float eased = Mathf.SmoothStep(0f, 1f, _progress);
            Transform cameraTransform = _camera.transform;
            Transform parent = cameraTransform.parent;
            Vector3 targetPosition = parent.InverseTransformPoint(_pose.position);
            Quaternion targetRotation = Quaternion.Inverse(parent.rotation) * _pose.rotation;

            cameraTransform.localPosition = Vector3.Lerp(_homePosition, targetPosition, eased);
            cameraTransform.localRotation = Quaternion.Slerp(_homeRotation, targetRotation, eased);
            float monitorFov = Mathf.Max(MonitorVerticalFov, Camera.HorizontalToVerticalFieldOfView(MonitorHorizontalFov, _camera.aspect));
            _camera.fieldOfView = Mathf.Lerp(_homeFov, monitorFov, eased);
            return entering ? _progress >= 1f : _progress <= 0f;
        }

        public void Restore()
        {
            if (!_active || _camera == null)
                return;

            _camera.transform.localPosition = _homePosition;
            _camera.transform.localRotation = _homeRotation;
            _camera.fieldOfView = _homeFov;
            _active = false;
        }
    }
}
