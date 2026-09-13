using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Stealth
{
    public sealed class TimelineStealthKillPresentation : IStealthKillPresentation
    {
        private const string TimelineHostResourcePath = "Prefabs/Cutscenes/StealthKillTimelineHost";

        private readonly StealthKillPresentationConfig _config;
        private readonly float _cameraReturnBlendDuration;
        private readonly ResourcesAssetsLoader _assetsLoader;

        private PlayableDirector _director;
        private StealthKillCameraRig _cameraRig;
        private List<AnimationTrackOffsetState> _offsetStates;
        private PlayerCamera _playerCamera;
        private Transform _lookCameraParent;
        private bool _presentationArmed;
        private bool _cancelled;

        public TimelineStealthKillPresentation(
            StealthKillPresentationConfig config,
            float cameraReturnBlendDuration,
            ResourcesAssetsLoader assetsLoader)
        {
            _config = config;
            _cameraReturnBlendDuration = Mathf.Max(0f, cameraReturnBlendDuration);
            _assetsLoader = assetsLoader;
        }

        public IEnumerator Play(Player player, EnemyBase enemy, PlayerCamera playerCamera)
        {
            _cancelled = false;
            _playerCamera = playerCamera;

            Transform spaceOrigin = enemy.transform;
            _director = SpawnTimelineHost(spaceOrigin);
            _director.playableAsset = _config.Timeline;

            PrepareCutsceneCamera(playerCamera);
            _presentationArmed = true;

            SpawnCameraRig(spaceOrigin);
            ApplyBindings(_director, player, enemy);
            BindCinemachineTrack(_director, playerCamera.LookBrain);
            BindCameraShots(_director, _cameraRig);
            _offsetStates = ApplyAnimationSpaceOrigin(_config.Timeline, spaceOrigin);

            bool stopped = false;
            Action<PlayableDirector> onStopped = _ => stopped = true;

            try
            {
                _director.RebuildGraph();
                _director.time = 0d;
                _director.Evaluate();
                _director.stopped += onStopped;
                _director.Play();

                while (_cancelled == false && stopped == false && _director != null)
                {
                    if (_director.duration > 0d && _director.time >= _director.duration)
                        break;

                    yield return null;
                }

                if (_cancelled == false)
                    yield return BlendToGameplayCamera(playerCamera);
            }
            finally
            {
                if (_director != null)
                    _director.stopped -= onStopped;

                Cleanup();
            }
        }

        public void Cancel()
        {
            _cancelled = true;

            if (_director != null)
                _director.Stop();

            Cleanup();
        }

        private PlayableDirector SpawnTimelineHost(Transform spaceOrigin)
        {
            GameObject hostPrefab = _assetsLoader.Load<GameObject>(TimelineHostResourcePath);
            if (hostPrefab == null)
                throw new InvalidOperationException(
                    $"Missing stealth kill timeline host prefab at Resources/{TimelineHostResourcePath}.");

            GameObject host = UnityEngine.Object.Instantiate(
                hostPrefab,
                spaceOrigin.position,
                spaceOrigin.rotation);
            host.name = "StealthKillTimeline";

            PlayableDirector director = host.GetComponent<PlayableDirector>();
            if (director == null)
                throw new InvalidOperationException("StealthKillTimelineHost prefab requires PlayableDirector.");

            director.playOnAwake = false;
            director.extrapolationMode = DirectorWrapMode.None;
            return director;
        }

        private void SpawnCameraRig(Transform spaceOrigin)
        {
            if (_config.CameraRigPrefab == null)
                return;

            _cameraRig = UnityEngine.Object.Instantiate(
                _config.CameraRigPrefab,
                spaceOrigin.position,
                spaceOrigin.rotation);
            _cameraRig.name = "StealthKillCameraRig";
        }

        private void Cleanup()
        {
            if (_presentationArmed)
            {
                RestorePlayerPresentation(_playerCamera);
                _presentationArmed = false;
            }

            if (_playerCamera != null)
            {
                _playerCamera.SyncFromTransforms();
                _playerCamera = null;
            }

            RestoreAnimationSpaceOrigins(_offsetStates);
            _offsetStates = null;

            if (_cameraRig != null)
            {
                UnityEngine.Object.Destroy(_cameraRig.gameObject);
                _cameraRig = null;
            }

            if (_director != null)
            {
                UnityEngine.Object.Destroy(_director.gameObject);
                _director = null;
            }
        }

        private void PrepareCutsceneCamera(PlayerCamera playerCamera)
        {
            Player player = playerCamera.Player;
            if (player != null && player.Animator != null)
                player.Animator.enabled = true;

            Camera lookCamera = playerCamera.LookCamera;
            if (lookCamera != null)
            {
                _lookCameraParent = lookCamera.transform.parent;
                lookCamera.transform.SetParent(null, true);
            }

            CinemachineBrain brain = playerCamera.LookBrain;
            if (brain != null)
                brain.enabled = true;
        }

        private IEnumerator BlendToGameplayCamera(PlayerCamera playerCamera)
        {
            if (playerCamera == null)
                yield break;

            Camera lookCamera = playerCamera.LookCamera;
            Transform lookPivot = playerCamera.LookPivot;
            if (lookCamera == null || lookPivot == null)
            {
                RestorePlayerPresentation(playerCamera);
                yield break;
            }

            CinemachineBrain brain = playerCamera.LookBrain;
            if (brain != null)
                brain.enabled = false;

            if (_cameraReturnBlendDuration <= 0f)
            {
                RestorePlayerPresentation(playerCamera);
                yield break;
            }

            Vector3 startPosition = lookCamera.transform.position;
            Quaternion startRotation = lookCamera.transform.rotation;
            Vector3 gameplayLocalPosition = new(0f, 0.15f, -2.5f);
            float elapsed = 0f;

            while (elapsed < _cameraReturnBlendDuration && _cancelled == false)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _cameraReturnBlendDuration);

                Vector3 targetPosition = lookPivot.TransformPoint(gameplayLocalPosition);
                Quaternion targetRotation = lookPivot.rotation;

                lookCamera.transform.SetPositionAndRotation(
                    Vector3.Lerp(startPosition, targetPosition, t),
                    Quaternion.Slerp(startRotation, targetRotation, t));

                yield return null;
            }

            if (_cancelled == false)
                RestorePlayerPresentation(playerCamera);
        }

        private void RestorePlayerPresentation(PlayerCamera playerCamera)
        {
            if (playerCamera == null)
                return;

            Player player = playerCamera.Player;
            if (player != null && player.Animator != null)
            {
                player.Animator.runtimeAnimatorController = null;
                player.Animator.enabled = false;
            }

            playerCamera.ConfigureForGameplay();

            if (_lookCameraParent != null && playerCamera.LookCamera != null)
            {
                // ConfigureForGameplay already parents to LookPivot.
                _lookCameraParent = null;
            }

            _presentationArmed = false;
        }

        private static void ApplyBindings(PlayableDirector director, Player player, EnemyBase enemy)
        {
            if (director.playableAsset == null)
                return;

            Animator playerAnimator = player.Animator;
            Animator enemyAnimator = enemy.Animator;

            foreach (PlayableBinding output in director.playableAsset.outputs)
            {
                if (output.sourceObject == null)
                    continue;

                string streamName = output.streamName;
                string trackName = output.sourceObject.name;

                if (MatchesRole(streamName, trackName, "Player"))
                {
                    director.SetGenericBinding(
                        output.sourceObject,
                        ResolveBinding(output, playerAnimator, player.gameObject));
                    continue;
                }

                if (MatchesRole(streamName, trackName, "Enemy"))
                {
                    director.SetGenericBinding(
                        output.sourceObject,
                        ResolveBinding(output, enemyAnimator, enemy.gameObject));
                }
            }
        }

        private static void BindCinemachineTrack(PlayableDirector director, CinemachineBrain brain)
        {
            if (brain == null || director.playableAsset == null)
                return;

            foreach (PlayableBinding output in director.playableAsset.outputs)
            {
                if (output.sourceObject is not CinemachineTrack)
                    continue;

                director.SetGenericBinding(output.sourceObject, brain);
            }
        }

        private static void BindCameraShots(PlayableDirector director, StealthKillCameraRig cameraRig)
        {
            if (cameraRig == null || director.playableAsset is not TimelineAsset timeline)
                return;

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                if (track is not CinemachineTrack)
                    continue;

                foreach (TimelineClip clip in track.GetClips())
                {
                    if (clip.asset is not CinemachineShot shot)
                        continue;

                    string shotName = string.IsNullOrEmpty(shot.DisplayName)
                        ? clip.displayName
                        : shot.DisplayName;

                    if (cameraRig.TryGetShot(shotName, out CinemachineVirtualCameraBase vcam) == false)
                        continue;

                    PropertyName exposedName = shot.VirtualCamera.exposedName;
                    if (exposedName == default)
                        continue;

                    director.SetReferenceValue(exposedName, vcam);
                }
            }
        }

        private static bool MatchesRole(string streamName, string trackName, string role)
        {
            return string.Equals(streamName, role, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(trackName, role, StringComparison.OrdinalIgnoreCase);
        }

        private static UnityEngine.Object ResolveBinding(
            PlayableBinding output,
            Animator animator,
            GameObject root)
        {
            Type targetType = output.outputTargetType;

            if (targetType == typeof(Animator))
                return animator;

            if (targetType == typeof(GameObject))
                return root;

            return root;
        }

        private static List<AnimationTrackOffsetState> ApplyAnimationSpaceOrigin(
            PlayableAsset playableAsset,
            Transform spaceOrigin)
        {
            if (playableAsset is not TimelineAsset timeline)
                return null;

            List<AnimationTrackOffsetState> states = new();

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                if (track is not AnimationTrack animationTrack)
                    continue;

                states.Add(new AnimationTrackOffsetState(
                    animationTrack,
                    animationTrack.trackOffset,
                    animationTrack.position,
                    animationTrack.eulerAngles));

                animationTrack.trackOffset = TrackOffset.ApplyTransformOffsets;
                animationTrack.position = spaceOrigin.position;
                animationTrack.eulerAngles = spaceOrigin.rotation.eulerAngles;
            }

            return states.Count > 0 ? states : null;
        }

        private static void RestoreAnimationSpaceOrigins(List<AnimationTrackOffsetState> states)
        {
            if (states == null)
                return;

            for (int i = 0; i < states.Count; i++)
            {
                AnimationTrackOffsetState state = states[i];
                if (state.Track == null)
                    continue;

                state.Track.trackOffset = state.TrackOffset;
                state.Track.position = state.Position;
                state.Track.eulerAngles = state.EulerAngles;
            }
        }

        private readonly struct AnimationTrackOffsetState
        {
            public AnimationTrackOffsetState(
                AnimationTrack track,
                TrackOffset trackOffset,
                Vector3 position,
                Vector3 eulerAngles)
            {
                Track = track;
                TrackOffset = trackOffset;
                Position = position;
                EulerAngles = eulerAngles;
            }

            public AnimationTrack Track { get; }
            public TrackOffset TrackOffset { get; }
            public Vector3 Position { get; }
            public Vector3 EulerAngles { get; }
        }
    }
}
