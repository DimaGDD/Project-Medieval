using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Features.Player
{
    [CreateAssetMenu(fileName = "SO_MovementSettings", menuName = "Scriptable Objects/Player/SO_MovementSettings")]
    public class SO_MovementSettings : ScriptableObject
    {
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 12f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _jumpHeight = 1.5f;

        [Header("Ощущение управления")]

        [Tooltip("Сколько секунд помнить нажатие прыжка")]
        [SerializeField] private float _jumpBufferTime = 0.15f;
        [Tooltip("Сколько секунд после схода с платформы можно прыгнуть")]
        [SerializeField] private float _coyoteTime = 0.15f;

        public float MoveSpeed { get { return _moveSpeed; } }
        public float RotationSpeed { get { return _rotationSpeed; } }
        public float Gravity { get { return _gravity; } }
        public float JumpHeight { get { return _jumpHeight; } }
        public float JumpBufferTime { get { return _jumpBufferTime; } }
        public float CoyoteTime { get { return _coyoteTime; } }
    }
}
