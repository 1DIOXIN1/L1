using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Project.Develop.Editor
{
    public sealed class BotVfxPreviewWindow : EditorWindow
    {
        private const string PrefabFolder = "Assets/_Project/Develop/Resources/Prefabs/Entities/";
        private static readonly string[] States = { "Idle", "Move", "Attack", "PowerOff" };
        private static readonly string[] Labels = { "Ожидание", "Движение", "Атака", "Отключение" };
        private static readonly string[] Bots = { "Guard", "SawDrone", "Teacher", "Chef" };
        [SerializeField] private GameObject prefab;
        [SerializeField] private int stateIndex = 2;
        [SerializeField] private bool loop = true;
        [SerializeField] private float speed = 1f;
        private PreviewRenderUtility preview;
        private GameObject instance;
        private Animator animator;
        private ParticleSystem[] particles = Array.Empty<ParticleSystem>();
        private ParticleSystem[] externalAttackParticles = Array.Empty<ParticleSystem>();
        private float[] externalAttackRates = Array.Empty<float>();
        private bool playing = true;
        private float time;
        private float duration = 1f;
        private double lastTick;
        private Vector2 orbit = new Vector2(30f, 12f);
        private float zoom = 1f;
        private Bounds bounds;
        private string error;

        [MenuItem("Tools/L1/Bot VFX Preview")]
        public static void Open()
        {
            var window = GetWindow<BotVfxPreviewWindow>("Bot VFX Preview");
            window.minSize = new Vector2(480f, 420f);
            window.Show();
        }

        [MenuItem("Assets/Preview Bot VFX", false, 2000)]
        private static void OpenSelected()
        {
            Open();
            var window = GetWindow<BotVfxPreviewWindow>();
            window.prefab = Selection.activeObject as GameObject;
            window.Rebuild();
        }

        [MenuItem("Assets/Preview Bot VFX", true)]
        private static bool CanOpenSelected()
        {
            var selected = Selection.activeObject as GameObject;
            return selected != null && AssetDatabase.Contains(selected)
                && selected.GetComponentInChildren<Animator>(true) != null;
        }

        private void OnEnable()
        {
            EditorApplication.update += Tick;
            AssemblyReloadEvents.beforeAssemblyReload += Cleanup;
            lastTick = EditorApplication.timeSinceStartup;
            if (prefab == null)
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabFolder + "Guard.prefab");
        }

        private void OnDisable()
        {
            EditorApplication.update -= Tick;
            AssemblyReloadEvents.beforeAssemblyReload -= Cleanup;
            Cleanup();
        }

        private void Cleanup()
        {
            if (preview != null)
                preview.Cleanup();
            preview = null;
            instance = null;
            animator = null;
            particles = Array.Empty<ParticleSystem>();
            externalAttackParticles = Array.Empty<ParticleSystem>();
            externalAttackRates = Array.Empty<float>();
        }

        private void Rebuild()
        {
            Cleanup();
            error = null;
            if (prefab == null)
                return;
            try
            {
                preview = new PreviewRenderUtility();
                // Instantiate directly into the isolated preview scene: the user's scene stays untouched.
                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, preview.camera.gameObject.scene);
                if (PrefabUtility.IsPartOfPrefabInstance(instance))
                    PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                preview.AddSingleGO(instance);
                instance.hideFlags = HideFlags.HideAndDontSave;
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                AddExternalAttackEffect();
                // Never run gameplay logic or attack events on a preview copy.
                foreach (var behaviour in instance.GetComponentsInChildren<MonoBehaviour>(true))
                    DestroyImmediate(behaviour);
                foreach (var behaviour in instance.GetComponentsInChildren<Behaviour>(true))
                    if (!(behaviour is Animator)) behaviour.enabled = false;
                animator = instance.GetComponentInChildren<Animator>(true);
                if (animator == null || animator.runtimeAnimatorController == null)
                    throw new InvalidOperationException("У префаба нет настроенного Animator.");
                animator.enabled = true;
                animator.fireEvents = false;
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                particles = instance.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].useAutoRandomSeed = false;
                    particles[i].randomSeed = (uint)(117 + i * 71);
                }
                foreach (var skin in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    skin.updateWhenOffscreen = true;
                    skin.forceMatrixRecalculationPerRender = true;
                }
                var meshes = instance.GetComponentsInChildren<Renderer>(true)
                    .Where(r => !(r is ParticleSystemRenderer) && !(r is LineRenderer)).ToArray();
                bounds = meshes.Length > 0 ? meshes[0].bounds : new Bounds(Vector3.up, Vector3.one * 2f);
                foreach (var mesh in meshes.Skip(1)) bounds.Encapsulate(mesh.bounds);
                preview.camera.orthographic = true;
                preview.camera.nearClipPlane = 0.01f;
                preview.camera.farClipPlane = 100f;
                preview.camera.clearFlags = CameraClearFlags.Color;
                preview.camera.backgroundColor = new Color(0.12f, 0.13f, 0.15f);
                preview.lights[0].intensity = 1.3f;
                preview.lights[0].transform.rotation = Quaternion.Euler(35f, 25f, 0f);
                preview.lights[1].intensity = 0.8f;
                preview.ambientColor = new Color(0.35f, 0.35f, 0.35f);
                Restart();
            }
            catch (Exception exception)
            {
                error = exception.Message;
                Cleanup();
            }
            Repaint();
        }

        private void AddExternalAttackEffect()
        {
            foreach (var behaviour in instance.GetComponents<MonoBehaviour>())
            {
                var serialized = new SerializedObject(behaviour);
                var source = serialized.FindProperty("flameParticles")?.objectReferenceValue as ParticleSystem;
                var origin = serialized.FindProperty("attackOrigin")?.objectReferenceValue as Transform;
                if (source == null || !EditorUtility.IsPersistent(source) || origin == null)
                    continue;
                var effect = (GameObject)PrefabUtility.InstantiatePrefab(source.gameObject, instance.scene);
                effect.name = "AttackEffectPreview";
                effect.transform.SetParent(origin, false);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localRotation = Quaternion.identity;
                effect.transform.localScale = source.transform.localScale;
                externalAttackParticles = effect.GetComponentsInChildren<ParticleSystem>(true);
                externalAttackRates = externalAttackParticles.Select(p => p.emission.rateOverTimeMultiplier).ToArray();
                break;
            }
        }

        private void UpdateExternalAttackPreview()
        {
            float progress = time / duration;
            float warmup = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.12f, 0.22f, progress));
            float cooldown = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.78f, 0.90f, progress));
            float intensity = stateIndex == 2 ? warmup * cooldown : 0f;
            for (int i = 0; i < externalAttackParticles.Length; i++)
            {
                var emission = externalAttackParticles[i].emission;
                emission.rateOverTimeMultiplier = externalAttackRates[i] * intensity;
            }
        }

        private void Restart()
        {
            if (animator == null) return;
            var clip = animator.runtimeAnimatorController.animationClips
                .FirstOrDefault(c => c.name.IndexOf(States[stateIndex], StringComparison.OrdinalIgnoreCase) >= 0);
            duration = clip == null ? 1f : Mathf.Max(0.01f, clip.length);
            time = 0f;
            animator.Rebind();
            animator.Play(States[stateIndex], 0, 0f);
            animator.Update(0f);
            foreach (var particle in particles)
            {
                particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Play(false);
            }
            UpdateExternalAttackPreview();
            lastTick = EditorApplication.timeSinceStartup;
        }

        private void Advance(float delta)
        {
            if (animator == null) return;
            float remaining = Mathf.Min(delta, duration - time);
            while (remaining > 0.00001f)
            {
                float step = Mathf.Min(1f / 60f, remaining);
                time = Mathf.Min(duration, time + step);
                // Explicit sampling keeps the selected pose visible instead of taking gameplay transitions.
                animator.Play(States[stateIndex], 0, Mathf.Min(time / duration, 0.99999f));
                animator.Update(0f);
                UpdateExternalAttackPreview();
                foreach (var particle in particles)
                    particle.Simulate(step, false, false, true);
                remaining -= step;
            }
        }

        private void Tick()
        {
            double now = EditorApplication.timeSinceStartup;
            float delta = Mathf.Clamp((float)(now - lastTick), 0f, 0.1f);
            lastTick = now;
            if (!playing || animator == null || EditorApplication.isCompiling)
                return;
            if (time >= duration - 0.00001f)
            {
                if (loop) Restart();
                else playing = false;
            }
            if (playing) Advance(delta * speed);
            Repaint();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                var selected = (GameObject)EditorGUILayout.ObjectField("Префаб", prefab, typeof(GameObject), false);
                if (EditorGUI.EndChangeCheck()) { prefab = selected; Rebuild(); }
                if (GUILayout.Button("Обновить", GUILayout.Width(85f))) Rebuild();
            }
            using (new EditorGUILayout.HorizontalScope())
                foreach (string bot in Bots)
                    if (GUILayout.Button(bot, EditorStyles.miniButton))
                    {
                        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabFolder + bot + ".prefab");
                        Rebuild();
                    }
            int selectedState = GUILayout.Toolbar(stateIndex, Labels);
            if (selectedState != stateIndex)
            {
                stateIndex = selectedState;
                Restart();
                playing = true;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(playing ? "Пауза" : "Играть", GUILayout.Width(75f)))
                {
                    if (!playing && time >= duration - 0.00001f) Restart();
                    playing = !playing;
                    lastTick = EditorApplication.timeSinceStartup;
                }
                if (GUILayout.Button("Сначала", GUILayout.Width(75f))) Restart();
                loop = GUILayout.Toggle(loop, "Повтор", GUILayout.Width(70f));
                speed = EditorGUILayout.Slider("Скорость", speed, 0.1f, 2f);
            }
            EditorGUI.BeginChangeCheck();
            float selectedTime = EditorGUILayout.Slider("Время", time, 0f, duration);
            if (EditorGUI.EndChangeCheck())
            {
                playing = false;
                Restart();
                Advance(selectedTime);
            }
            if (preview == null && prefab != null && error == null) Rebuild();
            Rect rect = GUILayoutUtility.GetRect(100f, 10000f, 150f, 10000f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (preview != null && instance != null)
            {
                HandleCamera(rect);
                if (Event.current.type == EventType.Repaint)
                {
                    Bounds framing = bounds;
                    if (stateIndex == 2)
                    {
                        foreach (var line in instance.GetComponentsInChildren<LineRenderer>(true))
                            for (int i = 0; i < line.positionCount; i++)
                                framing.Encapsulate(line.useWorldSpace ? line.GetPosition(i) : line.transform.TransformPoint(line.GetPosition(i)));
                    }
                    Quaternion rotation = Quaternion.Euler(-orbit.y, orbit.x, 0f);
                    Quaternion inverse = Quaternion.Inverse(rotation);
                    float aspect = Mathf.Max(0.1f, rect.width / rect.height);
                    float fit = 0f;
                    for (int corner = 0; corner < 8; corner++)
                    {
                        Vector3 offset = new Vector3(
                            (corner & 1) == 0 ? -framing.extents.x : framing.extents.x,
                            (corner & 2) == 0 ? -framing.extents.y : framing.extents.y,
                            (corner & 4) == 0 ? -framing.extents.z : framing.extents.z);
                        Vector3 projected = inverse * offset;
                        fit = Mathf.Max(fit, Mathf.Abs(projected.y), Mathf.Abs(projected.x) / aspect);
                    }
                    preview.camera.orthographicSize = Mathf.Max(0.6f, fit * 1.15f + 0.15f) * zoom;
                    preview.camera.transform.position = framing.center + rotation * Vector3.forward * 10f;
                    preview.camera.transform.LookAt(framing.center);
                    preview.BeginPreview(rect, GUIStyle.none);
                    preview.Render(true);
                    preview.EndAndDrawPreview(rect);
                }
            }
            else GUI.Label(rect, error ?? "Выбери префаб бота.", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField("Мышь — вращение · Колесо — масштаб · Работает без Play Mode", EditorStyles.centeredGreyMiniLabel);
        }

        private void HandleCamera(Rect rect)
        {
            Event e = Event.current;
            if (!rect.Contains(e.mousePosition)) return;
            if (e.type == EventType.MouseDrag && e.button == 0)
            {
                orbit.x += e.delta.x * 0.5f;
                orbit.y = Mathf.Clamp(orbit.y + e.delta.y * 0.5f, -70f, 70f);
                e.Use(); Repaint();
            }
            else if (e.type == EventType.ScrollWheel)
            {
                zoom = Mathf.Clamp(zoom * (1f + e.delta.y * 0.05f), 0.3f, 4f);
                e.Use(); Repaint();
            }
        }
    }
}
