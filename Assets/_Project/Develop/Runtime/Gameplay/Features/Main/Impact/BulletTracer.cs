using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Impact
{
    public sealed class BulletTracer : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;

        public void Show(float width, float endWidthRatio, Color color)
        {
            line.startWidth = width;
            line.endWidth = width * endWidthRatio;
            line.startColor = color;
            line.endColor = color;
            line.enabled = true;
            gameObject.SetActive(true);
        }

        public void SetEndpoints(Vector3 start, Vector3 end)
        {
            line.SetPosition(0, start);
            line.SetPosition(1, end);
        }

        public void Hide()
        {
            line.enabled = false;
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            line.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
