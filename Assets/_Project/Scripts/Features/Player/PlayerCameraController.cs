using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Features.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Настройки чувствительности")]
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private SO_CameraSettings _lookData;

        private PlayerInputHandler _playerInputHandler;

        private float _mouseSensitivity;
        private float _minPitch;
        private float _maxPitch;
        private float _cameraHeightOffset;

        private float _xRotation = 0f;

        private void Awake()
        {
            _playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Start()
        {
            _mouseSensitivity = _lookData.MouseSensitivity;
            _minPitch = _lookData.MinPitch;
            _maxPitch = _lookData.MaxPitch;
            _cameraHeightOffset = _lookData.CameraHeightOffset;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            Vector2 lookInput = _playerInputHandler.LookInput * _mouseSensitivity;

            _xRotation -= lookInput.y;
            _xRotation = Mathf.Clamp(_xRotation, _minPitch, _maxPitch);

            float currentYaw = _cameraTarget.localEulerAngles.y;

            _cameraTarget.localRotation = Quaternion.Euler(_xRotation, currentYaw + lookInput.x, 0f);
        }

        private void LateUpdate()
        {
            _cameraTarget.position = transform.position + Vector3.up * _cameraHeightOffset;
        }
    }
}