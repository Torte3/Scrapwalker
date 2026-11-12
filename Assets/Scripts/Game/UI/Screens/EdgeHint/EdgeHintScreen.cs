using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.EdgeHint
{
    public sealed class EdgeHintScreen : UIScreen
    {
        private const float FadePerSecond = 6f;
        private const float PulseRadiansPerSecond = 4f;

        private readonly EdgeHintSide _left;
        private readonly EdgeHintSide _right;
        private float _elapsedSeconds;

        public EdgeHintScreen(VisualElement root) : base(root)
        {
            _left = new EdgeHintSide(root, "left", 1f);
            _right = new EdgeHintSide(root, "right", -1f);
        }

        protected override void OnBind()
        {
            _elapsedSeconds = 0f;
            _left.Reset();
            _right.Reset();
        }

        public void Set(bool canScrollLeft, bool canScrollRight, float pullLeft, float pullRight)
        {
            _left.IsScrollable = canScrollLeft;
            _left.Pull = pullLeft;
            _right.IsScrollable = canScrollRight;
            _right.Pull = pullRight;
        }

        public void Refresh()
        {
            float deltaTime = App.UIDelta;
            _elapsedSeconds += deltaTime;
            float pulse = 0.5f + 0.5f * Mathf.Sin(_elapsedSeconds * PulseRadiansPerSecond);
            _left.Refresh(deltaTime * FadePerSecond, pulse);
            _right.Refresh(deltaTime * FadePerSecond, pulse);
        }
    }
}
