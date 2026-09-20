using _Project.Develop.Runtime.Cutscenes;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Interactables
{
    public sealed class InteractionSetup
    {
        public InteractionSetup(
            ICutsceneService cutscenes,
            Player player,
            StealItemService stealItems,
            InteractionService interactions)
        {
            Cutscenes = cutscenes;
            Player = player;
            StealItems = stealItems;
            Interactions = interactions;
        }

        public ICutsceneService Cutscenes { get; }
        public Player Player { get; }
        public StealItemService StealItems { get; }
        public InteractionService Interactions { get; }
    }
}
