using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Presentation
{
    public sealed class EnemyDeathFade
    {
        private readonly Material[] _materials;
        private readonly Color[] _baseColors;
        private readonly float _duration;
        private float _elapsed;

        public EnemyDeathFade(Renderer[] renderers, float duration)
        {
            _duration = Mathf.Max(0.01f, duration);

            int count = 0;
            for (int i = 0; i < renderers.Length; i++)
                count += renderers[i].materials.Length;

            _materials = new Material[count];
            _baseColors = new Color[count];

            int index = 0;
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].materials;
                for (int m = 0; m < materials.Length; m++)
                {
                    Material material = materials[m];
                    ConfigureTransparent(material);
                    _materials[index] = material;
                    _baseColors[index] = material.HasProperty("_Color")
                        ? material.color
                        : Color.white;
                    index++;
                }
            }
        }

        public bool Tick(float deltaTime)
        {
            _elapsed += deltaTime;
            float alpha = 1f - Mathf.Clamp01(_elapsed / _duration);

            for (int i = 0; i < _materials.Length; i++)
            {
                Material material = _materials[i];
                if (material.HasProperty("_Color") == false)
                    continue;

                Color color = _baseColors[i];
                color.a = alpha;
                material.color = color;
            }

            return _elapsed >= _duration;
        }

        private static void ConfigureTransparent(Material material)
        {
            if (material.HasProperty("_Mode"))
                material.SetFloat("_Mode", 2f);

            if (material.HasProperty("_SrcBlend"))
                material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);

            if (material.HasProperty("_DstBlend"))
                material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);

            if (material.HasProperty("_ZWrite"))
                material.SetInt("_ZWrite", 0);

            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
    }
}
