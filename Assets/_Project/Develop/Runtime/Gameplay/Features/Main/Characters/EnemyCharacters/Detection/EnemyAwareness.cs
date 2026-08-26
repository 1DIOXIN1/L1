using System;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Detection
{
    public sealed class EnemyAwareness
    {
        private readonly float _suspiciousFillTime;
        private readonly float _lostHoldTime;
        private readonly float _lostDecayTime;
        private readonly float _damageSuspicionBurst;

        private DetectionPhase _phase = DetectionPhase.Calm;
        private float _meter;
        private float _lostHoldTimer;
        private bool _alarmPendingSight;

        public EnemyAwareness(
            float suspiciousFillTime,
            float lostHoldTime,
            float lostDecayTime,
            float damageSuspicionBurst)
        {
            _suspiciousFillTime = Mathf.Max(0.05f, suspiciousFillTime);
            _lostHoldTime = Mathf.Max(0f, lostHoldTime);
            _lostDecayTime = Mathf.Max(0.05f, lostDecayTime);
            _damageSuspicionBurst = Mathf.Clamp01(damageSuspicionBurst);
        }

        public DetectionPhase Phase => _phase;
        public float Meter => _meter;
        public bool IsAlerted => _phase == DetectionPhase.Alerted;
        public bool AwaitingPersonalSight => _alarmPendingSight;
        public float DamageSuspicionBurst => _damageSuspicionBurst;

        public event Action Changed;

        /// <summary>
        /// Raises suspicion without confirming the player (no Alerted).
        /// Alerted is reserved for personal line of sight.
        /// </summary>
        public void NotifyStimulus(float suspicionAmount)
        {
            if (_phase == DetectionPhase.Alerted)
                return;

            if (_phase != DetectionPhase.Suspicious)
                SetPhase(DetectionPhase.Suspicious);

            _meter = Mathf.Clamp01(Mathf.Max(_meter, suspicionAmount));
            RaiseChanged();
        }

        public void NotifyAlarm()
        {
            if (_phase == DetectionPhase.Alerted)
                return;

            _alarmPendingSight = true;
            SetPhase(DetectionPhase.Suspicious);
            _meter = 1f;
            RaiseChanged();
        }

        public void TickSight(bool canSeePlayer, float deltaTime)
        {
            switch (_phase)
            {
                case DetectionPhase.Calm:
                    if (canSeePlayer)
                    {
                        SetPhase(DetectionPhase.Suspicious);
                        AddSight(deltaTime);
                    }
                    break;

                case DetectionPhase.Suspicious:
                    if (canSeePlayer)
                    {
                        _alarmPendingSight = false;
                        AddSight(deltaTime);
                    }
                    else if (_alarmPendingSight == false)
                    {
                        _meter = Mathf.Max(0f, _meter - deltaTime / _suspiciousFillTime);
                        RaiseChanged();

                        if (_meter <= 0f)
                            SetPhase(DetectionPhase.Calm);
                    }
                    break;

                case DetectionPhase.Alerted:
                    if (canSeePlayer == false)
                        BeginLost();
                    break;

                case DetectionPhase.Lost:
                    if (canSeePlayer)
                    {
                        BecomeAlerted();
                        break;
                    }

                    _lostHoldTimer -= deltaTime;
                    if (_lostHoldTimer > 0f)
                    {
                        RaiseChanged();
                        break;
                    }

                    _meter = Mathf.Max(0f, _meter - deltaTime / _lostDecayTime);
                    RaiseChanged();

                    if (_meter <= 0f)
                    {
                        _alarmPendingSight = false;
                        SetPhase(DetectionPhase.Calm);
                    }
                    break;
            }
        }

        public void ForceAlerted()
        {
            BecomeAlerted();
        }

        public void ResetToCalm()
        {
            _alarmPendingSight = false;
            _lostHoldTimer = 0f;
            _meter = 0f;
            SetPhase(DetectionPhase.Calm);
        }

        private void AddSight(float deltaTime)
        {
            _meter = Mathf.Clamp01(_meter + deltaTime / _suspiciousFillTime);
            RaiseChanged();

            if (_meter >= 1f)
                BecomeAlerted();
        }

        private void BecomeAlerted()
        {
            _alarmPendingSight = false;
            _meter = 1f;
            SetPhase(DetectionPhase.Alerted);
        }

        private void BeginLost()
        {
            _lostHoldTimer = _lostHoldTime;
            _meter = 1f;
            SetPhase(DetectionPhase.Lost);
        }

        private void SetPhase(DetectionPhase phase)
        {
            if (_phase == phase)
                return;

            _phase = phase;
            RaiseChanged();
        }

        private void RaiseChanged() => Changed?.Invoke();
    }
}
