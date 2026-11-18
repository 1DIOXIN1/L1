using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.CoroutinesManagement;
using _Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay.Workbench
{
    public sealed class WorkbenchPresenter : IPresenter
    {
        private readonly WorkbenchInteractable _bench;
        private readonly WorkbenchView _view;
        private readonly GameObject _hud;
        private readonly WorkbenchConfig _config;
        private readonly WorkbenchService _shop;
        private readonly Player _player;
        private readonly IInputService _input;
        private readonly PlayerDataProvider _data;
        private readonly CoroutinesPerformer _coroutines;

        private bool _supplies;
        private int _selected;

        public WorkbenchPresenter(WorkbenchInteractable bench, WorkbenchView view, GameObject hud, WorkbenchConfig config,
            WorkbenchService shop, Player player, IInputService input, PlayerDataProvider data, CoroutinesPerformer coroutines)
        {
            _bench = bench;
            _view = view;
            _hud = hud;
            _config = config;
            _shop = shop;
            _player = player;
            _input = input;
            _data = data;
            _coroutines = coroutines;
        }

        public void Initialize()
        {
            _view.SetVisible(false);

            _bench.OpenRequested += Open;

            _view.TabSelected += SelectTab;
            _view.ItemSelected += SelectItem;
            _view.ActionClicked += Buy;
            _view.Closed += Close;

            _input.WorkbenchClosePressed += Close;
        }

        public void Dispose()
        {
            _bench.OpenRequested -= Open;

            _view.TabSelected -= SelectTab;
            _view.ItemSelected -= SelectItem;
            _view.ActionClicked -= Buy;
            _view.Closed -= Close;

            _input.WorkbenchClosePressed -= Close;
        }

        private void Open()
        {
            if (_input.CurrentContext != InputContext.Gameplay || _player.IsControlLocked)
                return;

            _input.SetContext(InputContext.Workbench);
            _player.SetControlMode(PlayerControlMode.Locked);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _hud.SetActive(false);
            _view.SetVisible(true);

            _supplies = false;
            _selected = 0;

            for (int i = 0; i < _config.Weapons.Count; i++)
                if (_config.Weapons[i].Type == _player.Combat.Weapons.CurrentWeaponType)
                    _selected = i;

            Refresh();
        }

        private void Close()
        {
            if (_input.CurrentContext != InputContext.Workbench)
                return;

            _view.SetVisible(false);
            _hud.SetActive(true);
            _player.SetControlMode(PlayerControlMode.Free);
            _input.SetContext(InputContext.Gameplay);
        }

        private void SelectTab(bool supplies)
        {
            _supplies = supplies;
            SelectItem(0);
        }

        private void SelectItem(int index)
        {
            _selected = index;

            Refresh();
        }

        private void Buy(int index)
        {
            if (_input.CurrentContext != InputContext.Workbench)
                return;

            bool bought = _supplies
                ? _shop.TryBuy(_config.Supplies[_selected])
                : _shop.TryBuy(_config.Weapons[_selected], (WorkbenchAction)index);

            if (bought)
                _coroutines.StartPerform(_data.Save());

            Refresh();
        }

        private void Refresh()
        {
            if (_supplies)
                ShowSupplies();
            else
                ShowWeapons();
        }

        private void ShowSupplies()
        {
            _view.SetCatalog(true, _config.Supplies.Count, _shop.Metal);

            for (int i = 0; i < _config.Supplies.Count; i++)
            {
                WorkbenchSupply item = _config.Supplies[i];
                _view.SetItem(i, item.title, string.Format(WorkbenchText.Stock, _shop.SupplyCount(item)), item.icon, i == _selected);
            }

            WorkbenchSupply selected = _config.Supplies[_selected];
            _view.SetDetail(WorkbenchText.Supplies, selected.title, selected.icon, string.Format(WorkbenchText.StockDetail, _shop.SupplyCount(selected)), false);

            ShowOffer(0, WorkbenchText.BuyOne, _shop.Quote(selected));
            _view.SetActionCount(1);
        }

        private void ShowWeapons()
        {
            _view.SetCatalog(false, _config.Weapons.Count, _shop.Metal);

            for (int i = 0; i < _config.Weapons.Count; i++)
            {
                WorkbenchWeapon item = _config.Weapons[i];
                string detail = _shop.Owns(item) ? WorkbenchText.Owned : string.Format(WorkbenchText.MetalPrice, item.weaponPrice);
                _view.SetItem(i, item.title, detail, item.icon, i == _selected);
            }

            WorkbenchWeapon entry = _config.Weapons[_selected];
            WeaponAttachments installed = _shop.Attachments(entry);
            int capacity = entry.MagazineSize(installed);
            float spread = entry.weapon.MaxHipSpreadDegrees * entry.SpreadScale(installed);

            _view.SetDetail(entry.category, entry.title, entry.icon,
                string.Format(WorkbenchText.Reserve, _shop.Reserve(entry), entry.weapon.ReserveAmmo), true);

            _view.SetStat(0, capacity.ToString(), (float)capacity / (entry.weapon.MagazineSize + entry.extraRounds));
            _view.SetStat(1, string.Format(WorkbenchText.SpreadDegrees, spread), spread / entry.weapon.MaxHipSpreadDegrees);

            ShowOffer(0, WorkbenchText.Assemble, _shop.Quote(entry, WorkbenchAction.Weapon));
            ShowOffer(1, string.Format(WorkbenchText.Magazine, entry.extraRounds), _shop.Quote(entry, WorkbenchAction.Magazine));
            ShowOffer(2, string.Format(WorkbenchText.Suppressor, 1f - entry.spreadMultiplier), _shop.Quote(entry, WorkbenchAction.Suppressor));

            int quantity = _shop.AmmoQuantity(entry);
            ShowOffer(3, quantity > 0 ? string.Format(WorkbenchText.AmmoPack, quantity) : WorkbenchText.Ammo, _shop.Quote(entry, WorkbenchAction.Ammo));
        }

        private void ShowOffer(int index, string label, WorkbenchOffer offer)
        {
            string status = offer.Status ?? string.Format(WorkbenchText.ShortPrice, offer.Price);

            if (!offer.Available && offer.Price > 0)
                status = string.Format(WorkbenchText.BlockedPrice, offer.Price, offer.Status);

            _view.SetAction(index, label, status, offer.Available);
        }
    }
}
