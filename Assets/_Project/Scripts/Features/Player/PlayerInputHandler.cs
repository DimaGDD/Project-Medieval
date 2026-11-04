using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Features.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public event System.Action JumpPressed;

        [SerializeField] private SO_InputActions _inputActions;

        public Vector2 LookInput { get; private set; }
        public Vector2 MoveInput { get; private set;  }

        private InputActionReference _lookAction;
        private InputActionReference _moveAction;
        private InputActionReference _jumpAction;

        private void Awake()
        {
            _lookAction = _inputActions.LookInput;
            _moveAction = _inputActions.MoveInput;
            _jumpAction = _inputActions.JumpInput;

            _jumpAction.action.performed += ctx => JumpPressed?.Invoke();
        }

        private void Update()
        {
            MoveInput = _moveAction.action.ReadValue<Vector2>();
            LookInput = _lookAction.action.ReadValue<Vector2>();
        }
    }
}