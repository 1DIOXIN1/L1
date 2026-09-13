using _Project.Develop.Runtime.Configs.Meta.Enemy;
using _Project.Develop.Runtime.Utilities.AssetsManagement;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Stealth
{
    public sealed class StealthKillPresentationFactory
    {
        private readonly ResourcesAssetsLoader _assetsLoader;

        public StealthKillPresentationFactory(ResourcesAssetsLoader assetsLoader)
        {
            _assetsLoader = assetsLoader;
        }

        public IStealthKillPresentation Create(
            StealthKillPresentationConfig presentation,
            float cameraReturnBlendDuration)
        {
            if (presentation == null || presentation.HasTimeline == false)
                return null;

            return new TimelineStealthKillPresentation(
                presentation,
                cameraReturnBlendDuration,
                _assetsLoader);
        }
    }
}
