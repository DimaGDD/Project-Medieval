using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SO_CameraSettings", menuName = "Scriptable Objects/Player/SO_CameraSettings")]
public class SO_CameraSettings : ScriptableObject
{
    [SerializeField] private InputActionReference _lookInput;
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private float _minPitch = -45f;
    [SerializeField] private float _maxPitch = 85f;

    public InputActionReference LookInput { get { return _lookInput; } }
    public float MouseSensitivity { get { return _mouseSensitivity; } }
    public float MinPitch { get { return _minPitch; } }
    public float MaxPitch { get { return _maxPitch; } }
}
