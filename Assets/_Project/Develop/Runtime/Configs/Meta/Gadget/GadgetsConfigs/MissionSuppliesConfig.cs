using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs
{
    [CreateAssetMenu(menuName = "Configs/Core/Gameplay/Gadget/MissionSuppliesConfig", fileName = "MissionSuppliesConfig")]
    public sealed class MissionSuppliesConfig : ScriptableObject
    {
        [field: SerializeField, Min(1)] public int CapacityPerType { get; private set; } = 3;
        [field: SerializeField, Min(0)] public int StartingAmount { get; private set; } = 3;

        [field: SerializeField, Min(1)] public int MedkitHealing { get; private set; } = 40;
        [field: SerializeField, Min(0.1f)] public float EnergyDuration { get; private set; } = 8f;

        [field: SerializeField, Min(0.1f)] public float EmpStunDuration { get; private set; } = 4f;
        [field: SerializeField, Min(0.1f)] public float EmpRadius { get; private set; } = 5f;
        [field: SerializeField, Min(0.1f)] public float EmpThrowSpeed { get; private set; } = 13f;
        [field: SerializeField, Min(0f)] public float EmpLiftSpeed { get; private set; } = 3f;
        [field: SerializeField, Min(0.1f)] public float EmpFlightTime { get; private set; } = 2.5f;
        [field: SerializeField, Min(0.01f)] public float EmpCollisionRadius { get; private set; } = 0.12f;

        [field: SerializeField] public Vector3 EmpLaunchOffset { get; private set; } = new(0.38f, -0.25f, 0.65f);

        [field: SerializeField] public GameObject EmpProjectile { get; private set; }
        [field: SerializeField] public LineRenderer EmpTrajectory { get; private set; }
    }
}
