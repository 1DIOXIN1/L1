using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class InteractionSetup
    {
        public InteractionSetup(ICutsceneService cutscenes, Player player)
        {
            Cutscenes = cutscenes;
            Player = player;
        }

        public ICutsceneService Cutscenes { get; }
        public Player Player { get; }
    }
}
