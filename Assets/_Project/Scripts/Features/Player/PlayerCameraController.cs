using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Настройки чувствительности")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private SO_CameraSettings _lookData;

    private InputActionReference _lookInput;
    private float _mouseSensitivity;
    private float _minPitch;
    private float _maxPitch;

    private float _xRotation = 0f;

    private void Start()
    {
        _lookInput = _lookData.LookInput;
        _mouseSensitivity = _lookData.MouseSensitivity;
        _minPitch = _lookData.MinPitch;
        _maxPitch = _lookData.MaxPitch;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = _lookInput.action.ReadValue<Vector2>() * _mouseSensitivity;

        _xRotation -= lookInput.y;
        _xRotation = Mathf.Clamp(_xRotation, _minPitch, _maxPitch);

        float currentYaw = _cameraTarget.localEulerAngles.y;

        _cameraTarget.localRotation = Quaternion.Euler(_xRotation, currentYaw + lookInput.x, 0f);
    }
}
