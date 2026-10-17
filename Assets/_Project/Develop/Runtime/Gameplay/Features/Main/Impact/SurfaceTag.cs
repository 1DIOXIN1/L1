using _Project.Develop.Runtime.Configs.Meta.Impact;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Impact
{
    public sealed class SurfaceTag : MonoBehaviour
    {
        [SerializeField] private SurfaceType surface = SurfaceType.Default;

        public SurfaceType Surface => surface;
    }
}
