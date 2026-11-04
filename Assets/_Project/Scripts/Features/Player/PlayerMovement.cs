using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace Game.Features.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Ссылки")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private SO_MovementSettings _movementData;

        private PlayerInputHandler _playerInputHandler;

        private float _moveSpeed;
        private float _rotationSpeed;
        private float _gravity;
        private float _jumpHeight;

        private CharacterController _characterController;
        private float _verticalVelocity;
        private Vector3 _horizontalMovement;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInputHandler = GetComponent<PlayerInputHandler>();
            _cameraTransform = Camera.main.transform;

            _moveSpeed = _movementData.MoveSpeed;
            _rotationSpeed = _movementData.RotationSpeed;
            _gravity = _movementData.Gravity;
            _jumpHeight = _movementData.JumpHeight;
        }

        private void Update()
        {
            MoveCharacter();
        }

        private void MoveCharacter()
        {
            CalculateHorizontalMovement();
            CalculateGravity();

            Vector3 finalMovement = _horizontalMovement + new Vector3(0, _verticalVelocity, 0);
            _characterController.Move(finalMovement * Time.deltaTime);
        }

        private void CalculateHorizontalMovement()
        {
            Vector2 input = _playerInputHandler.MoveInput;

            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * input.y + right * input.x;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

                _horizontalMovement = moveDirection.normalized * _moveSpeed;
            }
            else
            {
                _horizontalMovement = Vector3.zero;
            }
        }

        private void OnJumpPerformed()
        {
            if (_characterController.isGrounded)
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }

        private void CalculateGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            _verticalVelocity += _gravity * Time.deltaTime;
        }

        private void OnEnable()
        {
            _playerInputHandler.JumpPressed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            _playerInputHandler.JumpPressed -= OnJumpPerformed;
        }
    }
}
