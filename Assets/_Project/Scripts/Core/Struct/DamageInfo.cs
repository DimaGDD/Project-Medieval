using UnityEngine;

namespace Game.Features.Player
{
    public struct DamageInfo
    {
        public float amount;
        public GameObject source;
        public DamageType type;

        public DamageInfo(float amount, GameObject source, DamageType type)
        {
            this.amount = amount;
            this.source = source;
            this.type = type;
        }
    }
}
