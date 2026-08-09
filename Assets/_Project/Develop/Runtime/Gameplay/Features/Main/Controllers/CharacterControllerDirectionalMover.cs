using _Project.Develop.Runtime.Configs.Meta.Characters;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Controllers
{
    public class CharacterControllerDirectionalMover
    {
        private readonly CharacterController _controller;
        private float _speed;

        public CharacterControllerDirectionalMover(
            CharacterController controller,
            ICharacterConfig characterConfig)
        {
            _controller = controller;
            _speed = characterConfig.Speed;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetDirectional(
            Vector3 direction,
            float deltaTime)
        {
            if (direction == Vector3.zero)
                return;

            _controller.Move(direction.normalized * _speed * deltaTime);
        }
    }
}
