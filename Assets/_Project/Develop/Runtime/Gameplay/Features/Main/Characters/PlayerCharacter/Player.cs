using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Configs.Meta.Weapon;
using _Project.Develop.Runtime.Gameplay.Features.Main.Controllers;
using _Project.Develop.Runtime.Gameplay.Features.Main.Gadget;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class Player : Character
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform viewTransform;

        private IInputService _input;
        private CharacterControllerDirectionalMover _mover;
        private WeaponInventory _inventory;
        private GadgetInventory _gadgetInventory;
        private PlayerConfig _playerConfig;
        private CharacterController _characterController;
        private Vector3 _moveInput;
        private float _verticalVelocity;
        private float _stamina;
        private float _staminaRecoveryTimer;
        private bool _isCrouching;
        private bool _sprintRequested;

        public Transform FirePoint => firePoint;
        public float Stamina => _stamina;
        public bool IsCrouching => _isCrouching;
        public bool IsSprinting { get; private set; }

        public void Initialize(
            IInputService input,
            CharacterControllerDirectionalMover mover,
            WeaponInventory inventory,
            GadgetInventory gadgetInventory,
            PlayerConfig playerConfig)
        {
            _input = input;
            _mover = mover;
            _inventory = inventory;
            _gadgetInventory = gadgetInventory;
            _playerConfig = playerConfig;
            _characterController = GetComponent<CharacterController>();
            _stamina = _playerConfig.MaxStamina;

            InitializeHealth(playerConfig);
            ApplyCharacterHeight(_playerConfig.StandingHeight);
            SnapToGround();

            _input.Sprint += OnSprint;
            _input.Jump += OnJump;
            _input.Move += OnMove;
            _input.Shoot += OnShoot;
            _input.UseGadget += OnUseGadget;
            _input.Crouch += OnCrouch;
            _input.SelectPrimarySlot += OnSelectPrimarySlot;
            _input.SelectSecondarySlot += OnSelectSecondarySlot;
        }

        private void Update()
        {
            if (_playerConfig == null)
                return;

            float deltaTime = Time.deltaTime;

            UpdateHorizontalMovement(deltaTime);
            UpdateRotation();
            UpdateVerticalMovement(deltaTime);
            UpdateStamina(deltaTime);

            _sprintRequested = false;
        }

        private void OnDestroy()
        {
            if (_input == null)
                return;

            _input.Sprint -= OnSprint;
            _input.Jump -= OnJump;
            _input.Move -= OnMove;
            _input.Shoot -= OnShoot;
            _input.UseGadget -= OnUseGadget;
            _input.Crouch -= OnCrouch;
            _input.SelectPrimarySlot -= OnSelectPrimarySlot;
            _input.SelectSecondarySlot -= OnSelectSecondarySlot;
        }

        private void OnCrouch()
        {
            _isCrouching = !_isCrouching;
            ApplyCharacterHeight(_isCrouching ? _playerConfig.CrouchHeight : _playerConfig.StandingHeight);
        }

        private void OnJump()
        {
            if (_characterController.isGrounded == false || _isCrouching)
                return;

            _verticalVelocity = Mathf.Sqrt(_playerConfig.JumpHeight * -2f * _playerConfig.Gravity);
        }

        private void OnSprint()
        {
            _sprintRequested = true;
        }

        private void OnShoot() => _inventory.CurrentWeapon.Shoot();

        private void OnUseGadget() => _gadgetInventory.UseCurrentGadget();

        private void OnSelectPrimarySlot() => _inventory.EquipWeapon(SlotWeaponType.PrimarySlot);

        private void OnSelectSecondarySlot() => _inventory.EquipWeapon(SlotWeaponType.SecondarySlot);

        private void OnMove(Vector3 move) => _moveInput = Vector3.ClampMagnitude(move, 1f);

        private void UpdateHorizontalMovement(float deltaTime)
        {
            IsSprinting = CanSprint();

            float speed = _playerConfig.Speed;

            if (_isCrouching)
                speed *= _playerConfig.CrouchSpeedMultiplier;
            else if (IsSprinting)
                speed *= _playerConfig.SprintSpeedMultiplier;

            _mover.SetSpeed(speed);
            _mover.SetDirectional(GetCameraRelativeMoveDirection(), deltaTime);
        }

        private void UpdateVerticalMovement(float deltaTime)
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            _verticalVelocity += _playerConfig.Gravity * deltaTime;
            _characterController.Move(Vector3.up * _verticalVelocity * deltaTime);
        }

        private void UpdateStamina(float deltaTime)
        {
            if (IsSprinting)
            {
                _stamina = Mathf.Max(0f, _stamina - _playerConfig.SprintStaminaSpendPerSecond * deltaTime);
                _staminaRecoveryTimer = _playerConfig.StaminaRecoveryDelay;
                return;
            }

            if (_staminaRecoveryTimer > 0f)
            {
                _staminaRecoveryTimer -= deltaTime;
                return;
            }

            _stamina = Mathf.Min(_playerConfig.MaxStamina, _stamina + _playerConfig.StaminaRecoveryPerSecond * deltaTime);
        }

        private bool CanSprint()
        {
            return _sprintRequested && _isCrouching == false && _moveInput != Vector3.zero && _stamina > 0f;
        }

        private void ApplyCharacterHeight(float height)
        {
            if (_characterController == null)
                return;

            float feetY = transform.position.y - _characterController.height * 0.5f;
            _characterController.height = height;
            _characterController.center = new Vector3(
                _characterController.center.x,
                0f,
                _characterController.center.z);

            transform.position = new Vector3(
                transform.position.x,
                feetY + height * 0.5f,
                transform.position.z);
        }

        private void UpdateRotation()
        {
            if (viewTransform == null)
                return;

            Vector3 forward = viewTransform.forward;
            forward.y = 0f;

            if (forward == Vector3.zero)
                return;

            transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        private void SnapToGround()
        {
            RaycastHit[] hits = Physics.RaycastAll(
                transform.position + Vector3.up * _playerConfig.GroundSnapDistance,
                Vector3.down,
                _playerConfig.GroundSnapDistance * 2f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform.root == transform)
                    continue;

                transform.position = new Vector3(
                    transform.position.x,
                    hit.point.y + _characterController.height * 0.5f,
                    transform.position.z);
                return;
            }
        }

        private Vector3 GetCameraRelativeMoveDirection()
        {
            if (viewTransform == null)
                return _moveInput;

            Vector3 forward = viewTransform.forward;
            Vector3 right = viewTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * _moveInput.z + right * _moveInput.x;
        }
    }
}
