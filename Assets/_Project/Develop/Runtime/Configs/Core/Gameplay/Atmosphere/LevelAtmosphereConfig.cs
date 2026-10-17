using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace _Project.Develop.Runtime.Configs.Core.Gameplay.Atmosphere
{
    [CreateAssetMenu(
        menuName = "Configs/Core/Gameplay/Atmosphere/LevelAtmosphere",
        fileName = "LevelAtmosphereConfig")]
    public sealed class LevelAtmosphereConfig : ScriptableObject
    {
        [Header("Fog")]
        [SerializeField] private bool fogEnabled = true;
        [SerializeField] private FogMode fogMode = FogMode.ExponentialSquared;
        [SerializeField] private Color fogColor = new Color(0.22f, 0.24f, 0.22f, 1f);
        [SerializeField] private float fogDensity = 0.035f;
        [SerializeField] private float fogStartDistance = 8f;
        [SerializeField] private float fogEndDistance = 55f;

        [Header("Ambient")]
        [SerializeField] private Color ambientSkyColor = new Color(0.18f, 0.2f, 0.18f, 1f);
        [SerializeField] private Color ambientEquatorColor = new Color(0.14f, 0.15f, 0.13f, 1f);
        [SerializeField] private Color ambientGroundColor = new Color(0.08f, 0.08f, 0.07f, 1f);
        [SerializeField] private float ambientIntensity = 0.65f;

        [Header("Post Processing")]
        [SerializeField] private bool postProcessEnabled = true;
        [SerializeField] private PostProcessResources postProcessResources;
        [SerializeField] private float saturation = -28f;
        [SerializeField] private float contrast = 22f;
        [SerializeField] private float postExposure = -0.6f;
        [SerializeField] private Color colorFilter = new Color(0.92f, 0.86f, 0.75f, 1f);
        [SerializeField] private float vignetteIntensity = 0.48f;
        [SerializeField] private float grainIntensity = 0.28f;
        [SerializeField] private float bloomIntensity = 0.35f;
        [SerializeField] private float bloomThreshold = 1.1f;

        [Header("Dust")]
        [SerializeField] private bool dustEnabled = true;
        [SerializeField] private float dustEmissionRate = 10f;
        [SerializeField] private float dustSize = 0.02f;
        [SerializeField] private Color dustColor = new Color(0.65f, 0.62f, 0.5f, 0.28f);
        [SerializeField] private float dustBoxSize = 10f;

        public bool FogEnabled => fogEnabled;
        public FogMode FogMode => fogMode;
        public Color FogColor => fogColor;
        public float FogDensity => fogDensity;
        public float FogStartDistance => fogStartDistance;
        public float FogEndDistance => fogEndDistance;

        public Color AmbientSkyColor => ambientSkyColor;
        public Color AmbientEquatorColor => ambientEquatorColor;
        public Color AmbientGroundColor => ambientGroundColor;
        public float AmbientIntensity => ambientIntensity;

        public bool PostProcessEnabled => postProcessEnabled;
        public PostProcessResources PostProcessResources => postProcessResources;
        public float Saturation => saturation;
        public float Contrast => contrast;
        public float PostExposure => postExposure;
        public Color ColorFilter => colorFilter;
        public float VignetteIntensity => vignetteIntensity;
        public float GrainIntensity => grainIntensity;
        public float BloomIntensity => bloomIntensity;
        public float BloomThreshold => bloomThreshold;

        public bool DustEnabled => dustEnabled;
        public float DustEmissionRate => dustEmissionRate;
        public float DustSize => dustSize;
        public Color DustColor => dustColor;
        public float DustBoxSize => dustBoxSize;
    }
}
