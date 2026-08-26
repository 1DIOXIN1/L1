using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.UI.Core;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly GameplayPresentersFactory _presentersFactory;
        private readonly List<IPresenter> _childPresenters = new();

        private PlayerVitalsPresenter _vitalsPresenter;
        private WeaponHudPresenter _weaponHudPresenter;

        public GameplayScreenPresenter(GameplayScreenView view, GameplayPresentersFactory presentersFactory)
        {
            _view = view;
            _presentersFactory = presentersFactory;
        }

        public void Initialize()
        {
            _vitalsPresenter = _presentersFactory.CreatePlayerVitalsPresenter(_view);
            _weaponHudPresenter = _presentersFactory.CreateWeaponHudPresenter(_view);

            _childPresenters.Add(_vitalsPresenter);
            _childPresenters.Add(_weaponHudPresenter);

            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Initialize();
        }

        public void AttachPlayer(Player player)
        {
            _vitalsPresenter?.AttachPlayer(player);
            _weaponHudPresenter?.AttachPlayer(player);
        }

        public void Tick()
        {
            _vitalsPresenter?.Tick();
        }

        public void Dispose()
        {
            foreach (IPresenter childPresenter in _childPresenters)
                childPresenter.Dispose();

            _childPresenters.Clear();
            _vitalsPresenter = null;
            _weaponHudPresenter = null;
        }
    }
}
