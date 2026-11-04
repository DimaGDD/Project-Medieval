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
        public float MoveSpeed { get { return _moveSpeed; } }
        public float RotationSpeed { get { return _rotationSpeed; } }
        public float Gravity { get { return _gravity; } }
        public float JumpHeight { get { return _jumpHeight; } }
    }
}
