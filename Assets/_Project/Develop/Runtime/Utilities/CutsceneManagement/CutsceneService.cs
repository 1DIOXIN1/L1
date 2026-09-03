using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;
using UnityEngine.Playables;

namespace _Project.Develop.Runtime.Utilities.CutsceneManagement
{
    public sealed class CutsceneService : ICutsceneService
    {
        private readonly IGameplayBlocker _gameplayBlocker;
        private readonly IInputService _input;
        private readonly CoroutinesPerformer _coroutines;
        private readonly Dictionary<string, CutsceneConfig> _configs = new();

        private PlayableDirector _director;
        private TaskCompletionSource<bool> _playCompletion;
        private UnityEngine.Object _playerBinding;

        public bool IsPlaying { get; private set; }

        public CutsceneService(
            IGameplayBlocker gameplayBlocker,
            IInputService input,
            CoroutinesPerformer coroutines)
        {
            _gameplayBlocker = gameplayBlocker;
            _input = input;
            _coroutines = coroutines;
            _input.ConfirmPressed += OnConfirmPressed;
        }

        public void Register(CutsceneConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.Id))
                throw new ArgumentException("Cutscene id is missing.", nameof(config));

            _configs[config.Id] = config;
        }

        public void SetPlayerBinding(UnityEngine.Object binding)
        {
            _playerBinding = binding;
        }

        public Task Play(string id)
        {
            if (IsPlaying)
                return Task.CompletedTask;

            if (_configs.TryGetValue(id, out CutsceneConfig config) == false)
                throw new InvalidOperationException($"Cutscene '{id}' is not registered.");

            if (config.Timeline == null)
                throw new InvalidOperationException($"Cutscene '{id}' has no timeline.");

            _playCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _coroutines.StartPerform(PlayRoutine(config));
            return _playCompletion.Task;
        }

        public void Skip()
        {
            if (IsPlaying == false || _director == null)
                return;

            _director.time = _director.duration;
            _director.Evaluate();
            _director.Stop();
        }

        private IEnumerator PlayRoutine(CutsceneConfig config)
        {
            IsPlaying = true;
            IDisposable gameplayLock = _gameplayBlocker.Block();
            bool stopped = false;

            Action<PlayableDirector> onStopped = _ => stopped = true;

            try
            {
                _director = CreateDirector(config);
                ApplyBindings(_director, config, _playerBinding);
                _director.RebuildGraph();
                _director.time = 0d;
                _director.Evaluate();
                _director.stopped += onStopped;
                _director.Play();

                while (stopped == false && _director != null)
                {
                    if (_director.duration > 0d && _director.time >= _director.duration)
                        break;

                    yield return null;
                }
            }
            finally
            {
                if (_director != null)
                {
                    _director.stopped -= onStopped;
                    UnityEngine.Object.Destroy(_director.gameObject);
                    _director = null;
                }

                gameplayLock.Dispose();
                IsPlaying = false;
                _playCompletion?.TrySetResult(true);
                _playCompletion = null;
            }
        }

        private void OnConfirmPressed()
        {
            if (IsPlaying)
                Skip();
        }

        private static PlayableDirector CreateDirector(CutsceneConfig config)
        {
            GameObject gameObject = new GameObject($"Cutscene_{config.Id}");
            PlayableDirector director = gameObject.AddComponent<PlayableDirector>();
            director.playOnAwake = false;
            director.extrapolationMode = DirectorWrapMode.None;
            director.playableAsset = config.Timeline;
            return director;
        }

        private static void ApplyBindings(
            PlayableDirector director,
            CutsceneConfig config,
            UnityEngine.Object playerBinding)
        {
            if (director.playableAsset == null)
                return;

            foreach (PlayableBinding output in director.playableAsset.outputs)
            {
                if (output.sourceObject == null)
                    continue;

                if (playerBinding != null &&
                    (output.outputTargetType == typeof(Animator) ||
                     output.streamName == "Player" ||
                     output.sourceObject.name == "Player"))
                {
                    director.SetGenericBinding(output.sourceObject, playerBinding);
                    continue;
                }

                if (config.Bindings == null)
                    continue;

                for (int i = 0; i < config.Bindings.Length; i++)
                {
                    CutsceneBinding binding = config.Bindings[i];
                    if (binding == null || binding.Target == null)
                        continue;

                    if (output.streamName == binding.ActorId || output.sourceObject.name == binding.ActorId)
                        director.SetGenericBinding(output.sourceObject, binding.Target);
                }
            }
        }
    }
}
