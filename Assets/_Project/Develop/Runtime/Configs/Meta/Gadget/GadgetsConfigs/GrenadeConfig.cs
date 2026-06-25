using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Gadget/GrenadeConfig", fileName = "GrenadeConfig")]
    public class GrenadeConfig : ScriptableObject, IGadgetConfig
    {
        [field: SerializeField] public int StartAmount { get; private set; } = 1;
        [field: SerializeField] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField] public int Damage { get; private set; } = 75;
        [field: SerializeField] public float ExplosionRadius { get; private set; } = 4f;
        [field: SerializeField] public float ExplosionForce { get; private set; } = 500f;
        [field: SerializeField] public float FuseTime { get; private set; } = 2f;
        [field: SerializeField] public float ThrowForce { get; private set; } = 12f;
        [field: SerializeField] public float UpwardForce { get; private set; } = 3f;
        [field: SerializeField] public float VisualScale { get; private set; } = 0.25f;
    }
}
