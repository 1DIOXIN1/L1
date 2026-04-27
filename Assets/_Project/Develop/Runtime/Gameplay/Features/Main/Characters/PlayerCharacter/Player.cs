using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class Player : MonoBehaviour, IDamageble
    {
        private float _deltaTime;
        private IInputService _input;
        private CharacterControllerDirectionalMover _mover;
        private WeaponInventory _inventory;
        private int _health = 100;
        private Transform _firepoint;
        
        public void Initialize(
            IInputService input,
            CharacterControllerDirectionalMover mover,
            WeaponInventory inventory)
        {
            _input = input;
            _mover = mover;
            _inventory = inventory;
            
            _deltaTime = Time.deltaTime;

            _input.Jump += OnJump;
            _input.Move += OnMove;
            _input.Shoot += OnShoot;
            _input.SelectPrimarySlot += OnSelectPrimarySlot;
            _input.SelectSecondarySlot += OnSelectSecondarySlot;
        }
        
        public void TakeDamage(int damage)
        {
            _health = _health - damage;
            if (_health <= 0)
            {
                _health = 0;
                Destroy(gameObject);
            }
        }
        
        private void OnJump(){}

        private void OnShoot() => _inventory.CurrentWeapon.Shoot();

        private void OnSelectPrimarySlot() => _inventory.EquipWeapon(SlotWeaponType.PrimarySlot);
        
        private void OnSelectSecondarySlot() => _inventory.EquipWeapon(SlotWeaponType.SecondarySlot);
        
        private void OnMove(Vector3 move) => _mover.SetDirectional( move, _deltaTime );
    }
}