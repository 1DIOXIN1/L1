using System;
using UnityEngine;
using UnityEngine.Playables;

namespace _Project.Develop.Runtime.Configs.Core.Gameplay
{
    [CreateAssetMenu(
        fileName = "Cutscene",
        menuName = "Project/Cutscene")]
    public sealed class CutsceneConfig : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private PlayableAsset timeline;
        [SerializeField] private CutsceneBinding[] bindings;

        public string Id => id;
        public PlayableAsset Timeline => timeline;
        public CutsceneBinding[] Bindings => bindings;
    }

    [Serializable]
    public sealed class CutsceneBinding
    {
        [SerializeField] private string actorId;
        [SerializeField] private UnityEngine.Object target;

        public string ActorId => actorId;
        public UnityEngine.Object Target => target;
    }
}
