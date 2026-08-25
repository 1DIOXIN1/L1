using _Project.Develop.Runtime.Configs.Meta.Characters.Player;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public sealed class PlayerMotor
    {
        private readonly CharacterController _characterController;
        private readonly Transform _transform;
        private readonly Transform _viewTransform;
        private readonly PlayerConfig _config;

        private Vector3 _moveInput;
        private float _verticalVelocity;
        private float _stamina;
        private float _staminaRecoveryTimer;
        private bool _isCrouching;
        private bool _sprintHeld;

        public PlayerMotor(
            CharacterController characterController,
            Transform transform,
            Transform viewTransform,
            PlayerConfig config)
        {
            _characterController = characterController;
            _transform = transform;
            _viewTransform = viewTransform;
            _config = config;
            _stamina = config.MaxStamina;

            ApplyCharacterHeight(config.StandingHeight);
            SnapToGround();
        }

        public float Stamina => _stamina;
        public bool IsCrouching => _isCrouching;
        public bool IsSprinting { get; private set; }

        public void SetMoveInput(Vector3 move)
        {
            _moveInput = Vector3.ClampMagnitude(move, 1f);
        }

        public void SetSprintHeld(bool held)
        {
            _sprintHeld = held;
        }

        public void Jump()
        {
            if (_characterController.isGrounded == false || _isCrouching)
                return;

            _verticalVelocity = Mathf.Sqrt(_config.JumpHeight * -2f * _config.Gravity);
        }

        public void ToggleCrouch()
        {
            _isCrouching = !_isCrouching;
            ApplyCharacterHeight(_isCrouching ? _config.CrouchHeight : _config.StandingHeight);
        }

        public void Tick(float deltaTime)
        {
            IsSprinting = CanSprint();
            UpdateStamina(deltaTime);

            float speed = _config.Speed;

            if (_isCrouching)
                speed *= _config.CrouchSpeedMultiplier;
            else if (IsSprinting)
                speed *= _config.SprintSpeedMultiplier;

            Vector3 motion = GetCameraRelativeMoveDirection() * speed;

            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            _verticalVelocity += _config.Gravity * deltaTime;
            motion.y = _verticalVelocity;

            _characterController.Move(motion * deltaTime);
        }

        public void LateTick()
        {
            if (_viewTransform == null)
                return;

            Vector3 forward = _viewTransform.forward;
            forward.y = 0f;

            if (forward == Vector3.zero)
                return;

            _transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        private bool CanSprint()
        {
            return _sprintHeld && _isCrouching == false && _moveInput != Vector3.zero && _stamina > 0f;
        }

        private void UpdateStamina(float deltaTime)
        {
            if (IsSprinting)
            {
                _stamina = Mathf.Max(0f, _stamina - _config.SprintStaminaSpendPerSecond * deltaTime);
                _staminaRecoveryTimer = _config.StaminaRecoveryDelay;
                return;
            }

            if (_staminaRecoveryTimer > 0f)
            {
                _staminaRecoveryTimer -= deltaTime;
                return;
            }

            _stamina = Mathf.Min(_config.MaxStamina, _stamina + _config.StaminaRecoveryPerSecond * deltaTime);
        }

        private void ApplyCharacterHeight(float height)
        {
            float feetY = _transform.position.y - _characterController.height * 0.5f;
            _characterController.height = height;
            _characterController.center = new Vector3(
                _characterController.center.x,
                0f,
                _characterController.center.z);

            _transform.position = new Vector3(
                _transform.position.x,
                feetY + height * 0.5f,
                _transform.position.z);
        }

        private void SnapToGround()
        {
            RaycastHit[] hits = Physics.RaycastAll(
                _transform.position + Vector3.up * _config.GroundSnapDistance,
                Vector3.down,
                _config.GroundSnapDistance * 2f);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform.root == _transform)
                    continue;

                _transform.position = new Vector3(
                    _transform.position.x,
                    hit.point.y + _characterController.height * 0.5f,
                    _transform.position.z);
                return;
            }
        }

        private Vector3 GetCameraRelativeMoveDirection()
        {
            if (_viewTransform == null)
                return _moveInput;

            Vector3 forward = _viewTransform.forward;
            Vector3 right = _viewTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * _moveInput.z + right * _moveInput.x;
        }
    }
}
