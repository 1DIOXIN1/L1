using System;
using _Project.Develop.Runtime.Configs.Core.Gameplay.Atmosphere;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Atmosphere
{
    public sealed class LevelAtmosphere : MonoBehaviour
    {
        private const string PostProcessingLayerName = "PostProcessing";

        [SerializeField] private LevelAtmosphereConfig config;
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private PostProcessVolume volume;

        private Transform _dustFollow;
        private PostProcessProfile _runtimeProfile;
        private int _postProcessingLayer;

        private void Awake()
        {
            if (config == null)
                throw new MissingReferenceException($"{nameof(LevelAtmosphere)} requires a {nameof(LevelAtmosphereConfig)}.");

            _postProcessingLayer = LayerMask.NameToLayer(PostProcessingLayerName);
            if (_postProcessingLayer < 0)
                _postProcessingLayer = 0;

            ApplyLightingAndFog();
            SetupPostProcessingVolume();
            ApplyDustSettings();
            TryBindCameras();
        }

        private void LateUpdate()
        {
            if (dustParticles == null || config.DustEnabled == false)
                return;

            if (_dustFollow == null)
                TryBindCameras();

            if (_dustFollow == null)
                return;

            dustParticles.transform.position = _dustFollow.position;
        }

        private void OnDestroy()
        {
            if (_runtimeProfile != null)
                Destroy(_runtimeProfile);
        }

        private void ApplyLightingAndFog()
        {
            RenderSettings.fog = config.FogEnabled;
            RenderSettings.fogMode = config.FogMode;
            RenderSettings.fogColor = config.FogColor;
            RenderSettings.fogDensity = config.FogDensity;
            RenderSettings.fogStartDistance = config.FogStartDistance;
            RenderSettings.fogEndDistance = config.FogEndDistance;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = config.AmbientSkyColor;
            RenderSettings.ambientEquatorColor = config.AmbientEquatorColor;
            RenderSettings.ambientGroundColor = config.AmbientGroundColor;
            RenderSettings.ambientIntensity = config.AmbientIntensity;
        }

        private void SetupPostProcessingVolume()
        {
            if (config.PostProcessEnabled == false)
                return;

            if (volume == null)
                throw new MissingComponentException(
                    $"{nameof(LevelAtmosphere)} requires a {nameof(PostProcessVolume)} on the prefab.");

            gameObject.layer = _postProcessingLayer;

            _runtimeProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            ConfigureProfile(_runtimeProfile);

            volume.isGlobal = true;
            volume.priority = 0f;
            volume.profile = _runtimeProfile;
        }

        private void TryBindCameras()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                if (camera.enabled == false)
                    continue;

                if (config.PostProcessEnabled)
                    EnsurePostProcessLayer(camera);

                if (_dustFollow == null && camera.GetComponent<AudioListener>() != null)
                    _dustFollow = camera.transform;
            }
        }

        private void ConfigureProfile(PostProcessProfile profile)
        {
            ColorGrading colorGrading = profile.AddSettings<ColorGrading>();
            colorGrading.enabled.Override(true);
            colorGrading.gradingMode.Override(GradingMode.HighDefinitionRange);
            colorGrading.tonemapper.Override(Tonemapper.ACES);
            colorGrading.saturation.Override(config.Saturation);
            colorGrading.contrast.Override(config.Contrast);
            colorGrading.postExposure.Override(config.PostExposure);
            colorGrading.colorFilter.Override(config.ColorFilter);

            Vignette vignette = profile.AddSettings<Vignette>();
            vignette.enabled.Override(true);
            vignette.intensity.Override(config.VignetteIntensity);
            vignette.smoothness.Override(0.55f);
            vignette.roundness.Override(1f);

            Grain grain = profile.AddSettings<Grain>();
            grain.enabled.Override(true);
            grain.colored.Override(false);
            grain.intensity.Override(config.GrainIntensity);
            grain.size.Override(0.8f);

            Bloom bloom = profile.AddSettings<Bloom>();
            bloom.enabled.Override(true);
            bloom.intensity.Override(config.BloomIntensity);
            bloom.threshold.Override(config.BloomThreshold);
            bloom.softKnee.Override(0.5f);
            bloom.diffusion.Override(5f);
        }

        private void EnsurePostProcessLayer(Camera camera)
        {
            PostProcessLayer layer = camera.GetComponent<PostProcessLayer>();
            if (layer == null)
                layer = camera.gameObject.AddComponent<PostProcessLayer>();

            if (config.PostProcessResources != null)
                layer.Init(config.PostProcessResources);

            layer.volumeTrigger = camera.transform;
            layer.volumeLayer = 1 << _postProcessingLayer;
            layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        }

        private void ApplyDustSettings()
        {
            if (config.DustEnabled == false)
            {
                if (dustParticles != null)
                    dustParticles.gameObject.SetActive(false);
                return;
            }

            if (dustParticles == null)
                throw new MissingReferenceException(
                    $"{nameof(LevelAtmosphere)} requires dustParticles assigned on the prefab when dust is enabled.");

            dustParticles.gameObject.SetActive(true);

            ParticleSystem.MainModule main = dustParticles.main;
            main.startSize = config.DustSize;
            main.startColor = config.DustColor;

            ParticleSystem.EmissionModule emission = dustParticles.emission;
            emission.rateOverTime = config.DustEmissionRate;

            ParticleSystem.ShapeModule shape = dustParticles.shape;
            shape.scale = Vector3.one * config.DustBoxSize;

            if (dustParticles.isPlaying == false)
                dustParticles.Play(true);
        }
    }
}
