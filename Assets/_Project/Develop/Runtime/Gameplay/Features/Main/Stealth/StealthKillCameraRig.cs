using System;
using Cinemachine;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Stealth
{
    public sealed class StealthKillCameraRig : MonoBehaviour
    {
        [Serializable]
        private struct ShotBinding
        {
            [SerializeField] private string name;
            [SerializeField] private CinemachineVirtualCameraBase virtualCamera;

            public string Name => name;
            public CinemachineVirtualCameraBase VirtualCamera => virtualCamera;
        }

        [SerializeField] private ShotBinding[] shots = Array.Empty<ShotBinding>();

        public bool TryGetShot(string shotName, out CinemachineVirtualCameraBase virtualCamera)
        {
            virtualCamera = null;
            if (string.IsNullOrEmpty(shotName) || shots == null)
                return false;

            for (int i = 0; i < shots.Length; i++)
            {
                ShotBinding shot = shots[i];
                if (string.Equals(shot.Name, shotName, StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                virtualCamera = shot.VirtualCamera;
                return virtualCamera != null;
            }

            return false;
        }
    }
}
