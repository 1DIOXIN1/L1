using _Project.Develop.Runtime.Gameplay.Main;
using _Project.Develop.Runtime.UI.Core;
using UnityEngine;

namespace _Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public SequenceView SequenceView { get; private set; }
    }
}