using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Features.Player
{
    [CreateAssetMenu(fileName = "SO_InputActions", menuName = "Scriptable Objects/Player/SO_InputActions")]
    public class SO_InputActions : ScriptableObject
    {
        [SerializeField] private InputActionReference _lookInput;
        [SerializeField] private InputActionReference _moveInput;
        [SerializeField] private InputActionReference _jumpInput;


        public InputActionReference LookInput { get { return _lookInput; } }
        public InputActionReference MoveInput { get { return _moveInput; } }
        public InputActionReference JumpInput { get { return _jumpInput; } }
    }
}
