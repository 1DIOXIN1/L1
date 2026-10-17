using System.IO;
using _Project.Develop.Runtime.Gameplay.Features.Main.Atmosphere;
using _Project.Develop.Runtime.Gameplay.Features.Main.Impact;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace _Project.Develop.Editor
{
    [InitializeOnLoad]
    public static class GameplayVfxPrefabBuilder
    {
        private const int BuildVersion = 6;
        private const string BuildVersionPrefsKey = "L1.GameplayVfxPrefabBuilder.Version";

        private const string ImpactFolder = "Assets/_Project/Develop/Resources/Prefabs/Impact";
        private const string MuzzleFolder = "Assets/_Project/Develop/Resources/Prefabs/Weapons/Muzzle";
        private const string MaterialsFolder = "Assets/_Project/Develop/Resources/Prefabs/Impact/Materials";
        private const string AtmospherePrefabPath =
            "Assets/_Project/Develop/Resources/Prefabs/Atmosphere/LevelAtmosphere.prefab";
        private const string TracerPrefabPath = ImpactFolder + "/BulletTracer.prefab";
        private const string HitImpactPrefabPath = ImpactFolder + "/HitImpactEffect.prefab";
        private const string MuzzlePrefabPath = MuzzleFolder + "/WeaponMuzzleVfx.prefab";

        static GameplayVfxPrefabBuilder()
        {
            EditorApplication.delayCall += EnsurePrefabsExist;
        }

        private static void EnsurePrefabsExist()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            bool missing =
                AssetDatabase.LoadAssetAtPath<GameObject>(TracerPrefabPath) == null ||
                AssetDatabase.LoadAssetAtPath<GameObject>(HitImpactPrefabPath) == null ||
                AssetDatabase.LoadAssetAtPath<GameObject>(MuzzlePrefabPath) == null;

            bool outdated = EditorPrefs.GetInt(BuildVersionPrefsKey, 0) < BuildVersion;
            if (missing || outdated)
                Build();
        }

        [MenuItem("Tools/L1/Build Gameplay VFX Prefabs")]
        public static void BuildFromMenu()
        {
            Build();
        }

        public static void Build()
        {
            EnsureFolder("Assets/_Project/Develop/Resources/Prefabs");
            EnsureFolder(ImpactFolder);
            EnsureFolder(MaterialsFolder);
            EnsureFolder("Assets/_Project/Develop/Resources/Prefabs/Weapons");
            EnsureFolder(MuzzleFolder);

            Material tracerMaterial = CreateParticleMaterial(
                $"{MaterialsFolder}/VfxTracer.mat",
                new Color(1f, 0.95f, 0.55f, 1f),
                additive: true);
            Material particleMaterial = CreateParticleMaterial(
                $"{MaterialsFolder}/VfxParticleAlpha.mat",
                new Color(0.65f, 0.6f, 0.5f, 0.85f),
                additive: false);
            Material decalMaterial = CreateDecalMaterial(
                $"{MaterialsFolder}/VfxDecal.mat",
                new Color(0.05f, 0.04f, 0.03f, 1f));
            Material muzzleFlashMaterial = CreateParticleMaterial(
                $"{MaterialsFolder}/VfxParticleAdditive.mat",
                new Color(1f, 0.92f, 0.7f, 1f),
                additive: true);
            Material dustMaterial = CreateParticleMaterial(
                $"{MaterialsFolder}/VfxDust.mat",
                new Color(0.65f, 0.62f, 0.5f, 0.28f),
                additive: false);

            BuildBulletTracerPrefab(tracerMaterial, TracerPrefabPath);
            BuildHitImpactPrefab(particleMaterial, decalMaterial, HitImpactPrefabPath);
            BuildMuzzlePrefab(
                muzzleFlashMaterial,
                particleMaterial,
                particleMaterial,
                tracerMaterial,
                MuzzlePrefabPath);
            UpgradeAtmospherePrefab(dustMaterial);

            EditorPrefs.SetInt(BuildVersionPrefsKey, BuildVersion);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Gameplay VFX prefabs built (v{BuildVersion}).");
        }

        private static void BuildBulletTracerPrefab(Material material, string path)
        {
            GameObject root = new GameObject("BulletTracer");
            LineRenderer line = root.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.alignment = LineAlignment.View;
            line.textureMode = LineTextureMode.Stretch;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.sortingOrder = 100;
            line.startWidth = 0.035f;
            line.endWidth = 0.016f;
            line.sharedMaterial = material;
            line.enabled = false;

            BulletTracer tracer = root.AddComponent<BulletTracer>();
            SerializedObject so = new SerializedObject(tracer);
            so.FindProperty("line").objectReferenceValue = line;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildHitImpactPrefab(
            Material particleMaterial,
            Material decalMaterial,
            string path)
        {
            GameObject root = new GameObject("HitImpactEffect");

            GameObject particlesObject = CreateChild(root.transform, "Particles", Vector3.zero);
            ParticleSystem particles = particlesObject.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(
                particles,
                duration: 0.15f,
                lifetime: 0.22f,
                speedMin: 0.4f,
                speedMax: 1.4f,
                sizeMin: 0.03f,
                sizeMax: 0.05f,
                color: new Color(0.65f, 0.6f, 0.5f, 0.85f),
                burstMin: 8,
                burstMax: 14,
                coneAngle: 28f,
                material: particleMaterial,
                worldSpace: true,
                gravity: 0.35f,
                maxParticles: 24);

            GameObject decalRootObject = CreateChild(root.transform, "Decal", Vector3.zero);
            Transform decalRoot = decalRootObject.transform;
            decalRoot.localScale = Vector3.one * 0.2f;

            Renderer front = CreateDecalQuad(decalRoot, "Front", Quaternion.identity, decalMaterial);
            Renderer back = CreateDecalQuad(decalRoot, "Back", Quaternion.Euler(0f, 180f, 0f), decalMaterial);

            HitImpactEffect effect = root.AddComponent<HitImpactEffect>();
            SerializedObject so = new SerializedObject(effect);
            so.FindProperty("particles").objectReferenceValue = particles;
            so.FindProperty("decalRoot").objectReferenceValue = decalRoot;
            SerializedProperty renderers = so.FindProperty("decalRenderers");
            renderers.arraySize = 2;
            renderers.GetArrayElementAtIndex(0).objectReferenceValue = front;
            renderers.GetArrayElementAtIndex(1).objectReferenceValue = back;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void BuildMuzzlePrefab(
            Material flashMaterial,
            Material smokeMaterial,
            Material casingMaterial,
            Material flashSpriteMaterial,
            string path)
        {
            GameObject root = new GameObject("WeaponMuzzleVfx");

            GameObject flashObject = CreateChild(root.transform, "VFX_MuzzleFlash", new Vector3(0f, 0.02f, 0.42f));
            ParticleSystem flash = flashObject.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(
                flash,
                duration: 0.08f,
                lifetime: 0.06f,
                speedMin: 0.4f,
                speedMax: 1.2f,
                sizeMin: 0.05f,
                sizeMax: 0.12f,
                color: new Color(1f, 0.97f, 0.8f, 1f),
                burstMin: 6,
                burstMax: 10,
                coneAngle: 18f,
                material: flashMaterial,
                worldSpace: false,
                gravity: 0f,
                maxParticles: 14);

            Light light = flashObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 2.4f;
            light.color = new Color(1f, 0.9f, 0.65f, 1f);
            light.shadows = LightShadows.None;
            light.enabled = false;

            GameObject flashSprite = GameObject.CreatePrimitive(PrimitiveType.Quad);
            flashSprite.name = "FlashSprite";
            flashSprite.transform.SetParent(flashObject.transform, false);
            flashSprite.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
            Object.DestroyImmediate(flashSprite.GetComponent<Collider>());
            Renderer flashRenderer = flashSprite.GetComponent<Renderer>();
            flashRenderer.sharedMaterial = flashSpriteMaterial;
            flashRenderer.shadowCastingMode = ShadowCastingMode.Off;
            flashRenderer.receiveShadows = false;
            flashRenderer.enabled = false;

            MuzzleFlashPulse pulse = flashObject.AddComponent<MuzzleFlashPulse>();
            SerializedObject pulseSo = new SerializedObject(pulse);
            pulseSo.FindProperty("flashLight").objectReferenceValue = light;
            pulseSo.FindProperty("flashRenderer").objectReferenceValue = flashRenderer;
            pulseSo.ApplyModifiedPropertiesWithoutUndo();

            GameObject smokeObject = CreateChild(root.transform, "VFX_MuzzleSmoke", new Vector3(0f, 0.02f, 0.42f));
            ParticleSystem smoke = smokeObject.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(
                smoke,
                duration: 0.2f,
                lifetime: 0.35f,
                speedMin: 0.15f,
                speedMax: 0.4f,
                sizeMin: 0.03f,
                sizeMax: 0.08f,
                color: new Color(0.55f, 0.55f, 0.5f, 0.35f),
                burstMin: 2,
                burstMax: 4,
                coneAngle: 16f,
                material: smokeMaterial,
                worldSpace: true,
                gravity: -0.05f,
                maxParticles: 10);

            GameObject casingObject = CreateChild(root.transform, "VFX_CasingEject", new Vector3(0.05f, 0.02f, 0.1f));
            casingObject.transform.localRotation = Quaternion.Euler(0f, 0f, -35f);
            ParticleSystem casing = casingObject.AddComponent<ParticleSystem>();
            ConfigureBurstParticles(
                casing,
                duration: 0.15f,
                lifetime: 0.45f,
                speedMin: 1.4f,
                speedMax: 2.2f,
                sizeMin: 0.012f,
                sizeMax: 0.018f,
                color: new Color(0.85f, 0.72f, 0.28f, 1f),
                burstMin: 1,
                burstMax: 1,
                coneAngle: 12f,
                material: casingMaterial,
                worldSpace: true,
                gravity: 1.2f,
                maxParticles: 4);

            WeaponMuzzleVfx muzzleVfx = root.AddComponent<WeaponMuzzleVfx>();
            SerializedObject muzzleSo = new SerializedObject(muzzleVfx);
            muzzleSo.FindProperty("muzzleFlash").objectReferenceValue = flash;
            muzzleSo.FindProperty("muzzleSmoke").objectReferenceValue = smoke;
            muzzleSo.FindProperty("casingEject").objectReferenceValue = casing;
            muzzleSo.FindProperty("flashPulse").objectReferenceValue = pulse;
            muzzleSo.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void UpgradeAtmospherePrefab(Material dustMaterial)
        {
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(AtmospherePrefabPath);

            PostProcessVolume volume = prefabRoot.GetComponent<PostProcessVolume>();
            if (volume == null)
                volume = prefabRoot.AddComponent<PostProcessVolume>();
            volume.isGlobal = true;

            Transform dustTransform = prefabRoot.transform.Find("VFX_DustAmbient");
            GameObject dustObject = dustTransform != null
                ? dustTransform.gameObject
                : CreateChild(prefabRoot.transform, "VFX_DustAmbient", Vector3.zero);

            ParticleSystem existing = dustObject.GetComponent<ParticleSystem>();
            if (existing != null)
                Object.DestroyImmediate(existing);

            ParticleSystem dust = dustObject.AddComponent<ParticleSystem>();
            dust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = dust.main;
            main.loop = true;
            main.playOnAwake = true;
            main.startLifetime = 8f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
            main.startSize = 0.02f;
            main.startColor = new Color(0.65f, 0.62f, 0.5f, 0.28f);
            main.maxParticles = 128;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 0f;

            ParticleSystem.EmissionModule emission = dust.emission;
            emission.rateOverTime = 10f;
            emission.SetBursts(System.Array.Empty<ParticleSystem.Burst>());

            ParticleSystem.ShapeModule shape = dust.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = Vector3.one * 10f;

            ParticleSystem.VelocityOverLifetimeModule velocity = dust.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.06f, 0.06f);
            velocity.y = new ParticleSystem.MinMaxCurve(-0.06f, 0.06f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.06f, 0.06f);

            ParticleSystem.NoiseModule noise = dust.noise;
            noise.enabled = true;
            noise.strength = 0.45f;
            noise.frequency = 0.35f;
            noise.scrollSpeed = 0.25f;
            noise.octaveCount = 2;
            noise.quality = ParticleSystemNoiseQuality.Medium;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = dust.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 0.2f),
                    new GradientAlphaKey(1f, 0.8f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            ParticleSystemRenderer dustRenderer = dust.GetComponent<ParticleSystemRenderer>();
            dustRenderer.sharedMaterial = dustMaterial;
            dustRenderer.renderMode = ParticleSystemRenderMode.Billboard;
            dustRenderer.velocityScale = 0f;
            dustRenderer.lengthScale = 1f;
            dustRenderer.cameraVelocityScale = 0f;

            LevelAtmosphere atmosphere = prefabRoot.GetComponent<LevelAtmosphere>();
            if (atmosphere != null)
            {
                SerializedObject so = new SerializedObject(atmosphere);
                so.FindProperty("dustParticles").objectReferenceValue = dust;
                so.FindProperty("volume").objectReferenceValue = volume;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, AtmospherePrefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        private static Renderer CreateDecalQuad(
            Transform parent,
            string name,
            Quaternion localRotation,
            Material material)
        {
            GameObject quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quadObject.name = name;
            Transform quad = quadObject.transform;
            quad.SetParent(parent, false);
            quad.localRotation = localRotation;
            Object.DestroyImmediate(quadObject.GetComponent<Collider>());

            Renderer renderer = quadObject.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            return renderer;
        }

        private static GameObject CreateChild(Transform parent, string name, Vector3 localPosition)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            child.transform.localPosition = localPosition;
            return child;
        }

        private static void ConfigureBurstParticles(
            ParticleSystem particleSystem,
            float duration,
            float lifetime,
            float speedMin,
            float speedMax,
            float sizeMin,
            float sizeMax,
            Color color,
            short burstMin,
            short burstMax,
            float coneAngle,
            Material material,
            bool worldSpace,
            float gravity,
            int maxParticles)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = particleSystem.main;
            main.playOnAwake = false;
            main.loop = false;
            main.duration = duration;
            main.startLifetime = lifetime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(speedMin, speedMax);
            main.startSize = new ParticleSystem.MinMaxCurve(sizeMin, sizeMax);
            main.startColor = color;
            main.maxParticles = maxParticles;
            main.simulationSpace = worldSpace
                ? ParticleSystemSimulationSpace.World
                : ParticleSystemSimulationSpace.Local;
            main.gravityModifier = gravity;

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.rateOverTime = 0f;
            emission.rateOverDistance = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, burstMin, burstMax) });

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = coneAngle;
            shape.radius = 0.01f;

            ParticleSystem.VelocityOverLifetimeModule velocity = particleSystem.velocityOverLifetime;
            velocity.enabled = false;

            ParticleSystem.NoiseModule noise = particleSystem.noise;
            noise.enabled = false;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });
            colorOverLifetime.color = gradient;

            ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.velocityScale = 0f;
            renderer.lengthScale = 1f;
            renderer.cameraVelocityScale = 0f;
            renderer.maxParticleSize = 0.5f;

            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private static Material CreateParticleMaterial(string assetPath, Color color, bool additive)
        {
            string shaderName = additive
                ? "Legacy Shaders/Particles/Additive"
                : "Legacy Shaders/Particles/Alpha Blended";

            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            Shader shader = Shader.Find(shaderName);
            if (shader == null)
                shader = Shader.Find("Particles/Standard Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");

            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, assetPath);
            }
            else
            {
                material.shader = shader;
            }

            material.color = color;
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
            if (material.HasProperty("_TintColor"))
                material.SetColor("_TintColor", color);

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateDecalMaterial(string assetPath, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");

            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, assetPath);
            }
            else
            {
                material.shader = shader;
            }

            material.color = color;
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);

            EditorUtility.SetDirty(material);
            return material;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (string.IsNullOrEmpty(parent) == false && AssetDatabase.IsValidFolder(parent) == false)
                EnsureFolder(parent);

            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
