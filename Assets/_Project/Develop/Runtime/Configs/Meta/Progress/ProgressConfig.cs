using UnityEngine;

namespace _Project.Develop.Runtime.Configs.Meta.Progress
{
    [CreateAssetMenu(menuName = "Configs/Meta/Progress/ProgressConfig", fileName = "ProgressConfig")]
    public class ProgressConfig : ScriptableObject
    {
        [field: SerializeField] public int ValueToResetProgress { get; private set; } = 50;
    }
}