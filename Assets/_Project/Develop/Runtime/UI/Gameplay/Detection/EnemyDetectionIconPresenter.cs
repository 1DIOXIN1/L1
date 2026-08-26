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
        private readonly Transform _player;
        private readonly float _heightOffset;

        private Camera _camera;

        public EnemyDetectionIconPresenter(
            EnemyAwareness awareness,
            EnemyDetectionIconView view,
            Transform followTarget,
            Transform player,
            float heightOffset)
        {
            _awareness = awareness;
            _view = view;
            _followTarget = followTarget;
            _player = player;
            _heightOffset = heightOffset;
        }

        public EnemyDetectionIconView View => _view;

        public void Initialize()
        {
            if (_awareness != null)
                _awareness.Changed += OnAwarenessChanged;

            OnAwarenessChanged();
            Tick();
        }

        public void Tick()
        {
            if (_view == null || _followTarget == null)
                return;

            if (_camera == null || _camera.isActiveAndEnabled == false)
                ResolveCamera();

            if (_camera == null)
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

        private void ResolveCamera()
        {
            if (_player != null)
                _camera = _player.GetComponentInChildren<Camera>(true);

            if (_camera == null || _camera.isActiveAndEnabled == false)
                _camera = Camera.main;
        }
    }
}
