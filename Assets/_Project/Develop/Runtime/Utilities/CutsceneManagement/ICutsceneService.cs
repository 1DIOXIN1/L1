using System.Threading.Tasks;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using UnityEngine;

namespace _Project.Develop.Runtime.Cutscenes
{
    public interface ICutsceneService
    {
        bool IsPlaying { get; }

        void Register(CutsceneConfig config);

        void SetPlayerBinding(UnityEngine.Object binding);

        /// <param name="spaceOrigin">
        /// World space origin for Animation Track root motion/position.
        /// Use the cutscene start point so recorded root animation plays from there, not from (0,0,0).
        /// </param>
        Task Play(string id, Transform spaceOrigin = null);

        void Skip();
    }
}
