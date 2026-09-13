using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Detection
{
    public sealed class EnemyDetectionIconsPresenter : IPresenter
    {
        private readonly EnemyAIService _enemyAIService;
        private readonly ViewsFactory _viewsFactory;
        private readonly GameplayPresentersFactory _presentersFactory;
        private readonly Dictionary<EnemyBase, EnemyDetectionIconPresenter> _presenters = new();

        private Camera _camera;
        private bool _visible = true;

        public EnemyDetectionIconsPresenter(
            EnemyAIService enemyAIService,
            ViewsFactory viewsFactory,
            GameplayPresentersFactory presentersFactory)
        {
            _enemyAIService = enemyAIService;
            _viewsFactory = viewsFactory;
            _presentersFactory = presentersFactory;
        }

        public void Initialize()
        {
            _enemyAIService.EnemyRegistered += OnEnemyRegistered;
            _enemyAIService.EnemyUnregistered += OnEnemyUnregistered;

            IReadOnlyList<EnemyBase> enemies = _enemyAIService.Enemies;
            for (int i = 0; i < enemies.Count; i++)
                OnEnemyRegistered(enemies[i]);
        }

        public void BindCamera(Camera camera)
        {
            _camera = camera;

            foreach (KeyValuePair<EnemyBase, EnemyDetectionIconPresenter> pair in _presenters)
                pair.Value.BindCamera(camera);
        }

        public void Tick()
        {
            foreach (KeyValuePair<EnemyBase, EnemyDetectionIconPresenter> pair in _presenters)
                pair.Value.Tick();
        }

        public void SetVisible(bool visible)
        {
            _visible = visible;

            foreach (KeyValuePair<EnemyBase, EnemyDetectionIconPresenter> pair in _presenters)
                pair.Value.SetVisible(visible);
        }

        public void Dispose()
        {
            _enemyAIService.EnemyRegistered -= OnEnemyRegistered;
            _enemyAIService.EnemyUnregistered -= OnEnemyUnregistered;

            foreach (KeyValuePair<EnemyBase, EnemyDetectionIconPresenter> pair in _presenters)
            {
                pair.Value.Dispose();
                _viewsFactory.Release(pair.Value.View);
            }

            _presenters.Clear();
        }

        private void OnEnemyRegistered(EnemyBase enemy)
        {
            if (enemy == null || _presenters.ContainsKey(enemy))
                return;

            EnemyDetectionIconView view =
                _viewsFactory.Create<EnemyDetectionIconView>(ViewIDs.EnemyDetectionIcon);

            float heightOffset = enemy.Context.Agent.height + 0.35f;
            EnemyDetectionIconPresenter presenter = _presentersFactory.CreateEnemyDetectionIconPresenter(
                enemy.Context.Awareness,
                view,
                enemy.transform,
                heightOffset,
                _camera);

            presenter.Initialize();
            presenter.SetVisible(_visible);
            _presenters.Add(enemy, presenter);
        }

        private void OnEnemyUnregistered(EnemyBase enemy)
        {
            if (enemy == null || _presenters.TryGetValue(enemy, out EnemyDetectionIconPresenter presenter) == false)
                return;

            presenter.Dispose();
            _viewsFactory.Release(presenter.View);
            _presenters.Remove(enemy);
        }
    }
}
