using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public sealed class EmpGrenade : IDisposable
    {
        private sealed class Flight
        {
            public GameObject Projectile;
            public EmpTrajectory.Motion Motion;
        }

        private readonly MissionSuppliesConfig _config;
        private readonly DamageableRegistry _damageables;
        private readonly EmpTrajectory _trajectory;
        private readonly List<Flight> _flights = new();

        public EmpGrenade(MissionSuppliesConfig config, Transform aim, Transform owner, DamageableRegistry damageables)
        {
            _config = config;
            _damageables = damageables;
            _trajectory = new EmpTrajectory(config, aim, owner);
        }

        public void ShowTrajectory() => _trajectory.Show();
        public void HideTrajectory() => _trajectory.Hide();

        public void Throw()
        {
            var motion = _trajectory.Start();
            var rotation = Quaternion.LookRotation(motion.Velocity);
            var projectile = Object.Instantiate(_config.EmpProjectile, motion.Position, rotation);

            _flights.Add(new Flight { Projectile = projectile, Motion = motion });
        }

        public void Tick(float deltaTime)
        {
            for (int i = _flights.Count - 1; i >= 0; i--)
            {
                Flight flight = _flights[i];
                bool finished = _trajectory.Advance(ref flight.Motion, deltaTime);
                flight.Projectile.transform.position = flight.Motion.Position;

                if (!finished)
                    continue;

                StunNearby(flight.Motion.Position);
                Object.Destroy(flight.Projectile);
                _flights.RemoveAt(i);
            }
        }

        public void Dispose()
        {
            _trajectory.Dispose();
            foreach (Flight flight in _flights)
                Object.Destroy(flight.Projectile);

            _flights.Clear();
        }

        private void StunNearby(Vector3 position)
        {
            Collider[] colliders = Physics.OverlapSphere(position, _config.EmpRadius, ~0, QueryTriggerInteraction.Collide);

            foreach (Collider collider in colliders)
            {
                if (_damageables.TryGet(collider, out IDamageble target) && target is EnemyBase enemy)
                    enemy.ApplyStun(_config.EmpStunDuration);
            }
        }
    }
}
