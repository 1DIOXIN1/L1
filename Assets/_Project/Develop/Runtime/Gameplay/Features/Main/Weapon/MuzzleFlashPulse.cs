using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public sealed class MuzzleFlashPulse : MonoBehaviour
    {
        [SerializeField] private Light flashLight;
        [SerializeField] private Renderer flashRenderer;

        private MaterialPropertyBlock _propertyBlock;
        private float _peakIntensity = 3.5f;
        private float _duration = 0.07f;
        private float _timer;
        private Color _flashColor = new Color(1f, 0.92f, 0.65f, 1f);

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            SetVisible(false);
        }

        public void Configure(float peakIntensity, float duration)
        {
            _peakIntensity = Mathf.Max(0f, peakIntensity);
            _duration = Mathf.Max(0.01f, duration);
        }

        public void Pulse()
        {
            _timer = _duration;
            SetVisible(true);
            flashLight.intensity = _peakIntensity;
            ApplyFlashColor(_flashColor);
        }

        private void LateUpdate()
        {
            if (_timer <= 0f)
                return;

            Camera camera = Camera.main;
            if (camera != null)
            {
                Vector3 toCamera = camera.transform.position - transform.position;
                if (toCamera.sqrMagnitude > 0.0001f)
                    flashRenderer.transform.rotation = Quaternion.LookRotation(toCamera.normalized);
            }

            _timer -= Time.deltaTime;
            float t = Mathf.Clamp01(_timer / _duration);
            flashLight.intensity = _peakIntensity * t;

            Color color = _flashColor;
            color.a = t;
            ApplyFlashColor(color);

            if (_timer <= 0f)
                SetVisible(false);
        }

        private void ApplyFlashColor(Color color)
        {
            flashRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_Color", color);
            flashRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void SetVisible(bool visible)
        {
            flashLight.enabled = visible;
            if (visible == false)
                flashLight.intensity = 0f;

            flashRenderer.enabled = visible;
        }
    }
}
