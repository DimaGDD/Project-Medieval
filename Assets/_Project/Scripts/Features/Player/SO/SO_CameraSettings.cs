using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Features.Player
{
    [CreateAssetMenu(fileName = "SO_CameraSettings", menuName = "Scriptable Objects/Player/SO_CameraSettings")]
    public class SO_CameraSettings : ScriptableObject
    {
        [SerializeField] private float _mouseSensitivity = 0.1f;
        [SerializeField] private float _minPitch = -45f;
        [SerializeField] private float _maxPitch = 85f;
        [SerializeField] private float _cameraHeightOffset = 1.2f;

        public float MouseSensitivity { get { return _mouseSensitivity; } }
        public float MinPitch { get { return _minPitch; } }
        public float MaxPitch { get { return _maxPitch; } }
        public float CameraHeightOffset { get { return _cameraHeightOffset;  } }
    }
}