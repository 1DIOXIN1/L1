using System.Collections;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.PlayerCharacter;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Stealth
{
    public interface IStealthKillPresentation
    {
        IEnumerator Play(Player player, EnemyBase enemy, PlayerCamera playerCamera);

        void Cancel();
    }
}
