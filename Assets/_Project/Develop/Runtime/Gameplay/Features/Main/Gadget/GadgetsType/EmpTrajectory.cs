using System;
using _Project.Develop.Runtime.Configs.Meta.Gadget.GadgetsConfigs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public sealed class EmpTrajectory : IDisposable
    {
        private const float MaxStep = 1f / 30f;

        public struct Motion
        {
            public Vector3 Position;
            public Vector3 Velocity;
            public float Remaining;
        }

        private readonly MissionSuppliesConfig _config;
        private readonly Transform _aim;
        private readonly Transform _owner;
        private readonly LineRenderer _line;
        private readonly Vector3[] _points;
        private readonly RaycastHit[] _hits = new RaycastHit[16];

        public EmpTrajectory(MissionSuppliesConfig config, Transform aim, Transform owner)
        {
            _config = config;
            _aim = aim;
            _owner = owner;
            _line = Object.Instantiate(config.EmpTrajectory);
            _points = new Vector3[Mathf.CeilToInt(config.EmpFlightTime / MaxStep) + 1];
        }

        public Motion Start()
        {
            Vector3 position = _aim.position;
            MoveUntilCollision(ref position, _aim.TransformDirection(_config.EmpLaunchOffset));

            return new Motion
            {
                Position = position,
                Velocity = _aim.forward * _config.EmpThrowSpeed + Vector3.up * _config.EmpLiftSpeed,
                Remaining = _config.EmpFlightTime
            };
        }

        public void Show()
        {
            Motion motion = Start();
            _points[0] = motion.Position;
            int count = 1;

            while (count < _points.Length)
            {
                bool finished = Advance(ref motion, MaxStep);
                _points[count++] = motion.Position;
                if (finished)
                    break;
            }

            _line.positionCount = count;
            _line.SetPositions(_points);
            _line.enabled = true;
        }

        public void Hide() => _line.enabled = false;

        public void Dispose()
        {
            if (_line)
                Object.Destroy(_line.gameObject);
        }

        public bool Advance(ref Motion motion, float deltaTime)
        {
            float timeLeft = Mathf.Min(deltaTime, motion.Remaining);
            while (timeLeft > 0f)
            {
                float step = Mathf.Min(MaxStep, timeLeft);
                Vector3 gravity = Physics.gravity;
                Vector3 offset = (motion.Velocity + gravity * (0.5f * step)) * step;
                motion.Velocity += gravity * step;
                motion.Remaining -= step;
                timeLeft -= step;

                if (MoveUntilCollision(ref motion.Position, offset))
                    return true;
            }

            return motion.Remaining <= 0.0001f;
        }

        private bool MoveUntilCollision(ref Vector3 position, Vector3 offset)
        {
            float distance = offset.magnitude;
            if (distance <= Mathf.Epsilon)
                return false;

            Vector3 direction = offset / distance;
            int count = Physics.SphereCastNonAlloc(position, _config.EmpCollisionRadius, direction, _hits,
                distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            RaycastHit[] hits = _hits;
            if (count == _hits.Length)
            {
                hits = Physics.SphereCastAll(position, _config.EmpCollisionRadius, direction,
                    distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                count = hits.Length;
            }

            float hitDistance = float.PositiveInfinity;
            for (int i = 0; i < count; i++)
            {
                if (!hits[i].collider.transform.IsChildOf(_owner))
                    hitDistance = Mathf.Min(hitDistance, hits[i].distance);
            }

            position += direction * Mathf.Min(hitDistance, distance);
            return hitDistance <= distance;
        }
    }
}
