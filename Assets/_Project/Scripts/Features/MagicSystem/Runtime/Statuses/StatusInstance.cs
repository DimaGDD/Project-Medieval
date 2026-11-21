using UnityEngine;

namespace Game.Features.MagicSystem.Statuses
{
    public class StatusInstance
    {
        public StatusDefinition Definition;
        public int Stacks;
        public float RemainingTime;
        public float TickTimer;
        public IStatusSource Source;
        public StatusController Controller;

        public GameObject Target => Controller != null ? Controller.gameObject : null;
    }
}
