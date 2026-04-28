using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class Player : Character
    {
        [SerializeField] private Transform firePoint;

        private IInputService _input;
        private CharacterControllerDirectionalMover _mover;
        private WeaponInventory _inventory;

        public Transform FirePoint => firePoint;

        public void Initialize(
            IInputService input,
            CharacterControllerDirectionalMover mover,
            WeaponInventory inventory,
            PlayerConfig playerConfig)
        {
            _input = input;
            _mover = mover;
            _inventory = inventory;

            InitializeHealth(playerConfig);

            _input.Sprint += OnSprint;
            _input.Jump += OnJump;
            _input.Move += OnMove;
            _input.Shoot += OnShoot;
            _input.SelectPrimarySlot += OnSelectPrimarySlot;
            _input.SelectSecondarySlot += OnSelectSecondarySlot;
        }

        private void OnDestroy()
        {
            if (_input == null)
                return;

            _input.Sprint -= OnSprint;
            _input.Jump -= OnJump;
            _input.Move -= OnMove;
            _input.Shoot -= OnShoot;
            _input.SelectPrimarySlot -= OnSelectPrimarySlot;
            _input.SelectSecondarySlot -= OnSelectSecondarySlot;
        }

        private void OnJump(){}

        private void OnSprint(){}

        private void OnShoot() => _inventory.CurrentWeapon.Shoot();

        private void OnSelectPrimarySlot() => _inventory.EquipWeapon(SlotWeaponType.PrimarySlot);

        private void OnSelectSecondarySlot() => _inventory.EquipWeapon(SlotWeaponType.SecondarySlot);

        private void OnMove(Vector3 move) => _mover.SetDirectional(move, Time.deltaTime);
    }
}
