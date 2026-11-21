using System.Collections.Generic;
using UnityEngine;

namespace Game.Features.MagicSystem.Statuses
{
    [DefaultExecutionOrder(-200)]
    public class StatusTickSystem : MonoBehaviour
    {
        public static StatusTickSystem Instance;

        private static readonly List<StatusController> _controllers = new List<StatusController>();

        public static void Register(StatusController controller)
        {
            if (controller == null) return;
            for (int i = 0; i < _controllers.Count; i++)
            {
                if (_controllers[i] == controller) return;
            }
            _controllers.Add(controller);
        }

        public static bool Contains(StatusController controller)
        {
            for (int i = 0; i < _controllers.Count; i++)
            {
                if (_controllers[i] == controller) return true;
            }
            return false;
        }

        public static void Unregister(StatusController controller)
        {
            if (controller == null) return;
            for (int i = _controllers.Count - 1; i >= 0; i--)
            {
                if (_controllers[i] == controller)
                {
                    _controllers.RemoveAt(i);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetStatics()
        {
            _controllers.Clear();
            Instance = null;
        }

        private float _accumulator;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            _accumulator += Time.deltaTime;
            if (_accumulator >= 0.1f)
            {
                float elapsed = _accumulator;
                _accumulator = 0f;
                for (int i = 0; i < _controllers.Count; i++)
                {
                    if (_controllers[i] != null)
                    {
                        _controllers[i].Tick(elapsed);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
