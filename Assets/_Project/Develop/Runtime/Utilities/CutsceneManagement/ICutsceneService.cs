using System.Threading.Tasks;
using _Project.Develop.Runtime.Configs.Core.Gameplay;
using UnityEngine;

namespace _Project.Develop.Runtime.Cutscenes
{
    public interface ICutsceneService
    {
        bool IsPlaying { get; }

        void Register(CutsceneConfig config);

        void SetPlayerBinding(Object binding);

        Task Play(string id);

        void Skip();
    }
}
