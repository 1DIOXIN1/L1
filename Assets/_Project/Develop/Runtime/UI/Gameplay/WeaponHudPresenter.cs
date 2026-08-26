using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon.WeaponsType;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.UI.Core;
using _Project.Develop.Runtime.Utilities.ConfigsManagement;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class WeaponHudPresenter : IPresenter
    {
        private readonly GameplayScreenView _view;
        private readonly PlayerStateService _playerStateService;
        private readonly ConfigsProviderService _configsProviderService;

        private WeaponInventory _inventory;
        private IWeapon _subscribedWeapon;

        public WeaponHudPresenter(
            GameplayScreenView view,
            PlayerStateService playerStateService,
            ConfigsProviderService configsProviderService)
        {
            _view = view;
            _playerStateService = playerStateService;
            _configsProviderService = configsProviderService;
        }

        public void Initialize()
        {
            SlotWeaponType selectedSlot = _playerStateService.SelectedWeaponSlot;
            WeaponType? weaponType = ResolveWeaponTypeFromSlot(selectedSlot);

            if (weaponType.HasValue)
            {
                int ammo = _playerStateService.GetAmmo(weaponType.Value);
                WeaponsCatalogConfig catalog = _configsProviderService.GetConfig<WeaponsCatalogConfig>();
                int magazine = catalog.GetWeapon(weaponType.Value).MagazineSize;
                _view.SetAmmo(ammo, magazine);
                _view.SetWeaponName(weaponType.Value.ToString());
            }
            else
            {
                _view.SetAmmo(0, 0);
                _view.SetWeaponName(string.Empty);
            }
        }

        public void AttachPlayer(Player player)
        {
            DetachPlayer();

            if (player?.Combat?.Weapons == null)
                return;

            _inventory = player.Combat.Weapons;
            _inventory.WeaponChanged += OnWeaponChanged;
            OnWeaponChanged(_inventory.CurrentWeapon);
        }

        public void Dispose()
        {
            DetachPlayer();
        }

        private void DetachPlayer()
        {
            UnsubscribeAmmo();

            if (_inventory != null)
            {
                _inventory.WeaponChanged -= OnWeaponChanged;
                _inventory = null;
            }
        }

        private void OnWeaponChanged(IWeapon weapon)
        {
            UnsubscribeAmmo();
            _subscribedWeapon = weapon;

            if (_subscribedWeapon != null)
                _subscribedWeapon.AmmoChanged += OnAmmoChanged;

            UpdateWeaponView(_subscribedWeapon);
        }

        private void OnAmmoChanged()
        {
            UpdateWeaponView(_subscribedWeapon);
        }

        private void UnsubscribeAmmo()
        {
            if (_subscribedWeapon == null)
                return;

            _subscribedWeapon.AmmoChanged -= OnAmmoChanged;
            _subscribedWeapon = null;
        }

        private void UpdateWeaponView(IWeapon weapon)
        {
            if (weapon == null)
            {
                _view.SetAmmo(0, 0);
                _view.SetWeaponName(string.Empty);
                return;
            }

            _view.SetAmmo(weapon.Ammo, weapon.MagazineSize);
            _view.SetWeaponName(weapon.Type.ToString());
        }

        private WeaponType? ResolveWeaponTypeFromSlot(SlotWeaponType slotType)
        {
            var inventoryConfig = _configsProviderService.GetConfig<PlayerWeaponInventoryConfig>();

            foreach (var slot in inventoryConfig.Slots)
            {
                if (slot.SlotType == slotType)
                    return slot.WeaponType;
            }

            return null;
        }
    }
}
