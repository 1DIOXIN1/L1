
using _Project.Develop.Runtime.Configs.Meta.Player;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Controllers
{
    public class CharacterControllerDirectionalMover
    {
        private readonly CharacterController _controller;
        private readonly float _speed;

        public CharacterControllerDirectionalMover(
            CharacterController controller,
             PlayerConfig playerConfig)
        {
            _controller = controller;
            _speed = playerConfig.Speed;
        }
        
        public void SetDirectional(
            Vector3 direction,
            float deltaTime)
        {
            _controller.Move(direction.normalized * _speed * deltaTime);
        }
    }
}