using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Detection
{
    public sealed class EnemyDetectionIconPresenter : IPresenter
    {
        private readonly EnemyAwareness _awareness;
        private readonly EnemyDetectionIconView _view;
        private readonly Transform _followTarget;
        private readonly float _heightOffset;

        private Camera _camera;
        private bool _visible = true;

        public EnemyDetectionIconPresenter(
            EnemyAwareness awareness,
            EnemyDetectionIconView view,
            Transform followTarget,
            float heightOffset,
            Camera camera)
        {
            _awareness = awareness;
            _view = view;
            _followTarget = followTarget;
            _heightOffset = heightOffset;
            _camera = camera;
        }

        public EnemyDetectionIconView View => _view;

        public void Initialize()
        {
            if (_awareness != null)
                _awareness.Changed += OnAwarenessChanged;

            _view.SetCamera(_camera);
            _view.SetVisible(_visible);
            OnAwarenessChanged();
            Tick();
        }

        public void SetCamera(Camera camera)
        {
            _camera = camera;
            _view.SetCamera(camera);
        }

        public void SetVisible(bool visible)
        {
            _visible = visible;
            _view?.SetVisible(visible);
        }

        public void Tick()
        {
            if (_view == null || _followTarget == null || _camera == null)
                return;

            Vector3 position = _followTarget.position + Vector3.up * _heightOffset;
            _view.SetWorldPose(position, _camera.transform.rotation);
        }

        public void Dispose()
        {
            if (_awareness != null)
                _awareness.Changed -= OnAwarenessChanged;
        }

        private void OnAwarenessChanged()
        {
            if (_awareness == null || _view == null)
                return;

            _view.SetPhase(_awareness.Phase, _awareness.Meter);
        }
    }
}
