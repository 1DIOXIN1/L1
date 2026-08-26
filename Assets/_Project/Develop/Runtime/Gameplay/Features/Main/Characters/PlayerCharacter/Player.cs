using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using _Project.Develop.Runtime.Gameplay.Features.Main.Noise;
using _Project.Develop.Runtime.Utilities.InputManagement;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class Player : Character
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform viewTransform;

        private IInputService _input;
        private PlayerMotor _motor;
        private PlayerCombatController _combat;
        private PlayerNoiseEmitter _noiseEmitter;
        private PlayerControlMode _controlMode = PlayerControlMode.Free;

        public Transform FirePoint => firePoint;
        public Transform ViewTransform => viewTransform;
        public PlayerCombatController Combat => _combat;
        public float Stamina => _motor?.Stamina ?? 0f;
        public float MaxStamina => _motor?.MaxStamina ?? 0f;
        public bool IsCrouching => _motor != null && _motor.IsCrouching;
        public bool IsSprinting => _motor != null && _motor.IsSprinting;
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
            _input.SelectPrimarySlot += OnSelectPrimarySlot;
            _input.SelectSecondarySlot += OnSelectSecondarySlot;
        }

        public void SetControlMode(PlayerControlMode mode)
        {
            _controlMode = mode;
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

        private void LateUpdate()
        {
            if (_motor == null || _controlMode == PlayerControlMode.Locked)
                return;

            _motor.LateTick();
        }

        private void OnDestroy()
        {
            if (_input == null)
                return;

            _input.Jump -= OnJump;
            _input.Move -= OnMove;
            _input.Shoot -= OnShoot;
            _input.Reload -= OnReload;
            _input.UseGadget -= OnUseGadget;
            _input.Crouch -= OnCrouch;
            _input.SelectPrimarySlot -= OnSelectPrimarySlot;
            _input.SelectSecondarySlot -= OnSelectSecondarySlot;
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

        private void OnSelectPrimarySlot()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _combat.SelectPrimary();
        }

        private void OnSelectSecondarySlot()
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _combat.SelectSecondary();
        }

        private void OnMove(Vector3 move)
        {
            if (_controlMode == PlayerControlMode.Locked)
                return;

            _motor.SetMoveInput(move);
        }
    }
}
