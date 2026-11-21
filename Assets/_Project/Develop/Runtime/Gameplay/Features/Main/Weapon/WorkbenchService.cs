using System;
using _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;
using _Project.Develop.Runtime.Meta.Features.Player;
using _Project.Develop.Runtime.Meta.Features.Wallet;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public enum WorkbenchAction { Weapon, Magazine, Suppressor, Ammo }

    public readonly struct WorkbenchOffer
    {
        public WorkbenchOffer(int price, string status = null)
        {
            Price = price;
            Status = status;
        }

        public int Price { get; }
        public string Status { get; }
        public bool Available => Status == null;
    }

    public sealed class WorkbenchService
    {
        private readonly WalletService _wallet;
        private readonly PlayerStateService _state;
        private readonly WeaponFactory _factory;
        private readonly Player _player;
        private readonly PlayerCamera _camera;
        private readonly MissionSuppliesConfig _supplies;

        public WorkbenchService(WalletService wallet, PlayerStateService state, WeaponFactory factory,
            Player player, PlayerCamera camera, MissionSuppliesConfig supplies)
        {
            _wallet = wallet;
            _state = state;
            _factory = factory;
            _player = player;
            _camera = camera;
            _supplies = supplies;
        }

        private WeaponInventory Inventory => _player.Combat.Weapons;

        public int Metal => _wallet.GetCurrency(CurrencyTypes.Metal).Value;

        public bool Owns(WorkbenchWeapon entry) => Inventory.Contains(entry.Type);
        public int SupplyCount(WorkbenchSupply supply) => _state.SupplyCount(supply.type);
        public WeaponAttachments Attachments(WorkbenchWeapon entry) => _state.GetAttachments(entry.Type);

        public int Reserve(WorkbenchWeapon entry) =>
            Inventory.TryGet(entry.Type, out IWeapon weapon) ? weapon.ReserveAmmo : 0;

        public int AmmoQuantity(WorkbenchWeapon entry) =>
            Mathf.Clamp(entry.weapon.ReserveAmmo - Reserve(entry), 0, entry.ammoPack);

        public WorkbenchOffer Quote(WorkbenchWeapon entry, WorkbenchAction action)
        {
            bool owned = Owns(entry);

            if (action == WorkbenchAction.Weapon)
                return owned ? new WorkbenchOffer(0, WorkbenchText.Owned) : QuotePrice(entry.weaponPrice);

            if (!owned)
                return new WorkbenchOffer(0, WorkbenchText.WeaponRequired);

            WeaponAttachments installed = Attachments(entry);

            switch (action)
            {
                case WorkbenchAction.Magazine:
                    return (installed & WeaponAttachments.Magazine) != 0
                        ? new WorkbenchOffer(0, WorkbenchText.Installed) : QuotePrice(entry.magazinePrice);

                case WorkbenchAction.Suppressor:
                    return (installed & WeaponAttachments.Suppressor) != 0
                        ? new WorkbenchOffer(0, WorkbenchText.Installed) : QuotePrice(entry.suppressorPrice);

                case WorkbenchAction.Ammo:
                    int quantity = AmmoQuantity(entry);
                    return quantity == 0 ? new WorkbenchOffer(0, WorkbenchText.ReserveFull)
                        : QuotePrice(Mathf.CeilToInt((float)entry.ammoPrice * quantity / entry.ammoPack));

                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }

        public bool TryBuy(WorkbenchWeapon entry, WorkbenchAction action)
        {
            WorkbenchOffer offer = Quote(entry, action);

            if (!offer.Available)
                return false;

            if (action == WorkbenchAction.Weapon)
            {
                IWeapon weapon = _factory.CreateWeapon(entry.Type, _player.FirePoint, _player, initialReserveAmmo: 0);
                weapon.SetAimCamera(_camera.LookCamera);
                weapon.SetAimingProvider(() => _camera.IsAiming);

                if (!Inventory.Add(weapon))
                    return false;
            }
            else
            {
                Inventory.TryGet(entry.Type, out IWeapon weapon);

                if (action == WorkbenchAction.Ammo)
                    weapon.SetReserveAmmo(weapon.ReserveAmmo + AmmoQuantity(entry));
                else
                {
                    _state.Install(entry.Type, action == WorkbenchAction.Magazine
                        ? WeaponAttachments.Magazine : WeaponAttachments.Suppressor);

                    WeaponAttachments installed = Attachments(entry);
                    weapon.SetModifiers(entry.MagazineSize(installed) - entry.weapon.MagazineSize, entry.SpreadScale(installed));
                }
            }

            _wallet.Spend(CurrencyTypes.Metal, offer.Price);
            return true;
        }

        public WorkbenchOffer Quote(WorkbenchSupply supply) =>
            SupplyCount(supply) >= _supplies.CapacityPerType
                ? new WorkbenchOffer(0, WorkbenchText.SupplyLimitReached)
                : QuotePrice(supply.price);

        public bool TryBuy(WorkbenchSupply supply)
        {
            if (!Quote(supply).Available)
                return false;

            _state.AddSupply(supply.type);

            _wallet.Spend(CurrencyTypes.Metal, supply.price);
            return true;
        }

        private WorkbenchOffer QuotePrice(int price) =>
            new WorkbenchOffer(price, _wallet.Enough(CurrencyTypes.Metal, price) ? null : WorkbenchText.InsufficientMetal);
    }
}
