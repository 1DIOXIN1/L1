using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Transform _firepoint;
        
        public  Transform Firepoint => _firepoint;
    }
}