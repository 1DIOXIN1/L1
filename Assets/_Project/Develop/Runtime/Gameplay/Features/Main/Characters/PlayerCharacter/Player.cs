using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Noise;
using _Project.Develop.Runtime.Gameplay.Features.Main.Weapon;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class Player : Character
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform lookPivot;
        [SerializeField] private Transform viewTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;

        private IInputService _input;
        private PlayerMotor _motor;
        private PlayerCombatController _combat;
        private PlayerNoiseEmitter _noiseEmitter;
        private PlayerWeaponView _weaponView;
        private WeaponInventory _weaponInventory;
        private PlayerControlMode _controlMode = PlayerControlMode.Free;

        public Transform FirePoint => firePoint;
        public Transform LookPivot => lookPivot;
        public Transform ViewTransform => viewTransform != null ? viewTransform : lookPivot;
        public Animator Animator => animator;
        public CharacterController CharacterController => characterController;
        public PlayerCombatController Combat => _combat;
        public float Stamina => _motor?.Stamina ?? 0f;
        public float MaxStamina => _motor?.MaxStamina ?? 0f;
        public bool IsCrouching => _motor != null && _motor.IsCrouching;
        public bool IsSprinting => _motor != null && _motor.IsSprinting;
        public Vector3 PlanarMoveDirection => _motor != null ? _motor.PlanarMoveDirection : Vector3.zero;
        public bool IsControlLocked => _controlMode == PlayerControlMode.Locked;
        public PlayerControlMode ControlMode => _controlMode;

        public void Initialize(
            IInputService input,
            PlayerMotor motor,
            PlayerCombatController combat,
            PlayerConfig playerConfig,
            int currentHealth,
            PlayerNoiseEmitter noiseEmitter = null)
        {
            _input = input;
            _motor = motor;
            _combat = combat;
            _noiseEmitter = noiseEmitter;

            InitializeHealth(currentHealth, playerConfig.Health);

            _input.Jump += OnJump;
            _input.Move += OnMove;
            _input.Shoot += OnShoot;
            _input.Reload += OnReload;
            _input.UseGadget += OnUseGadget;
            _input.Crouch += OnCrouch;
        }

        public void SetWeaponView(PlayerWeaponView weaponView, WeaponInventory weaponInventory)
        {
            ClearWeaponView();

            _weaponView = weaponView;
            _weaponInventory = weaponInventory;

            _weaponInventory.WeaponChanged += OnWeaponViewChanged;
            _weaponView.Show(_weaponInventory.CurrentWeapon);
        }

        public void SetControlMode(PlayerControlMode mode)
        {
            _controlMode = mode;

            if (mode == PlayerControlMode.Free)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Update()
        {
            if (_motor == null || _controlMode == PlayerControlMode.Locked)
                return;

            float deltaTime = Time.deltaTime;

            _motor.SetSprintHeld(_input.IsSprintHeld);
            _motor.Tick(deltaTime);
            _noiseEmitter?.Tick(deltaTime);
            _combat.Tick(deltaTime, _input.IsShootHeld);
        }

        private void OnDestroy()
        {
            ClearWeaponView();

            if (_input == null)
                return;

            _input.Jump -= OnJump;
            _input.Move -= OnMove;
            _input.Shoot -= OnShoot;
            _input.Reload -= OnReload;
            _input.UseGadget -= OnUseGadget;
            _input.Crouch -= OnCrouch;
        }

        private void OnWeaponViewChanged(IWeapon weapon)
        {
            _weaponView?.Show(weapon);
        }

        private void ClearWeaponView()
        {
            if (_weaponInventory != null)
            {
                _weaponInventory.WeaponChanged -= OnWeaponViewChanged;
                _weaponInventory = null;
            }

            _weaponView?.Clear();
            _weaponView = null;
        }

        private void OnCrouch()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _motor.ToggleCrouch();
        }

        private void OnJump()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _motor.Jump();
        }

        private void OnShoot()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _combat.OnShootPressed();
        }

        private void OnReload()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _combat.Reload();
        }

        private void OnUseGadget()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _combat.UseGadget();
        }

        private void OnMove(Vector3 move)
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _motor.SetMoveInput(move);
        }
    }
}
