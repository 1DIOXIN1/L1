using UnityEditor;
using UnityEngine;

namespace _Project.Develop.Editor
{
    public sealed class FabricatorVfxPreviewWindow : BotVfxPreviewWindow
    {
        private static readonly string[] States = { "Idle", "Craft" };
        private static readonly string[] Labels = { "Ожидание", "Крафт" };

        protected override string[] PreviewStates => States;
        protected override string[] PreviewLabels => Labels;
        protected override string DefaultPrefabPath =>
            "Assets/_Project/Art/Prefabs/PlayerBase/Objects/25_Ground_Fabrication_workbench/Fabricator.prefab";
        protected override bool ShowBotPresets => false;
        protected override bool UseAnimatedParticleActivation => true;
        protected override Vector2 DefaultOrbit => new Vector2(155f, 15f);
        protected override string EmptyMessage => "Префаб верстака не найден.";

        [MenuItem("Tools/L1/Fabricator VFX Preview")]
        public new static void Open()
        {
            var window = GetWindow<FabricatorVfxPreviewWindow>("Fabricator VFX Preview");
            window.minSize = new Vector2(480f, 420f);
            window.Show();
        }
    }
}
