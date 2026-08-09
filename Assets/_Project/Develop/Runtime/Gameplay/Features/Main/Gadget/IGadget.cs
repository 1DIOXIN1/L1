namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public interface IGadget
    {
        int Amount { get; }
        float Cooldown { get; }
        bool CanUse { get; }

        void Use();
    }
}
