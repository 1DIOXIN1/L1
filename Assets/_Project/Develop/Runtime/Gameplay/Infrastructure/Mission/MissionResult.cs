namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public readonly struct MissionResult
    {
        public MissionEndReason Reason { get; }

        public bool IsSuccess => Reason == MissionEndReason.ObjectivesComplete;

        public MissionResult(MissionEndReason reason)
        {
            Reason = reason;
        }
    }
}
