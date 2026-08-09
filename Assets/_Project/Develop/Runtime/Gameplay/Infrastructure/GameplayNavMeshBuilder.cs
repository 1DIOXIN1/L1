using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure
{
    public static class GameplayNavMeshBuilder
    {
        public static void Build()
        {
            NavMeshSurface[] surfaces = Object.FindObjectsOfType<NavMeshSurface>();

            if (surfaces.Length == 0)
            {
                var surfaceObject = new GameObject("NavMeshSurface");
                NavMeshSurface surface = surfaceObject.AddComponent<NavMeshSurface>();
                ConfigureSurface(surface);
                surface.BuildNavMesh();
                return;
            }

            foreach (NavMeshSurface surface in surfaces)
            {
                ConfigureSurface(surface);
                surface.BuildNavMesh();
            }
        }

        private static void ConfigureSurface(NavMeshSurface surface)
        {
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        }
    }
}
