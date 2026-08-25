using System;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public interface IMissionObjective
    {
        bool IsComplete { get; }
        event Action Completed;

        void Start();
        void Stop();
    }
}
