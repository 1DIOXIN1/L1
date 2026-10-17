using _Project.Develop.Runtime.Configs.Meta.Impact;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Impact
{
    public sealed class HitImpactEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private Transform decalRoot;
        [SerializeField] private Renderer[] decalRenderers;

        private MaterialPropertyBlock _decalPropertyBlock;

        public void Show(
            Vector3 point,
            Vector3 normal,
            HitImpactSurfaceSettings settings,
            float decalSize,
            float surfaceOffset,
            float minDecalScale)
        {
            Vector3 safeNormal = normal.sqrMagnitude > 0.0001f ? normal.normalized : Vector3.up;
            transform.position = point + safeNormal * surfaceOffset;
            transform.rotation = Quaternion.LookRotation(safeNormal);

            ParticleSystem.MainModule main = particles.main;
            main.startSize = settings.ParticleSize;
            main.startColor = settings.ParticleColor;

            bool showDecal = settings.SpawnDecal;
            decalRoot.gameObject.SetActive(showDecal);
            if (showDecal)
            {
                decalRoot.localScale = Vector3.one * Mathf.Max(minDecalScale, decalSize);
                ApplyDecalColor(settings.DecalColor);
            }

            gameObject.SetActive(true);
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.Play(true);
        }

        public void StopEmitting()
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void Hide()
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            _decalPropertyBlock = new MaterialPropertyBlock();
            gameObject.SetActive(false);
        }

        private void ApplyDecalColor(Color color)
        {
            color.a = 1f;
            for (int i = 0; i < decalRenderers.Length; i++)
            {
                Renderer renderer = decalRenderers[i];
                renderer.GetPropertyBlock(_decalPropertyBlock);
                _decalPropertyBlock.SetColor("_Color", color);
                renderer.SetPropertyBlock(_decalPropertyBlock);
            }
        }
    }
}
