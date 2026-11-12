using Scrapwalker.Core.Outpost.Hulls;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.TopBar
{
    internal sealed class HullSection
    {
        private readonly Meter _hull;
        private readonly VisualElement _barrierGroup;
        private readonly Label _barrierValue;
        private readonly Meter _barrier;

        public HullSection(VisualElement root)
        {
            _hull = new Meter(root.Need<Label>("hull-value"), root.Need<VisualElement>("hull-fill"),
                (hp, max) => Texts.Monospace(TopBarWords.HullValue(hp, max)));
            _barrierGroup = root.Need<VisualElement>("barrier-group");
            _barrierValue = root.Need<Label>("barrier-value");
            _barrier = new Meter(_barrierValue, root.Need<VisualElement>("barrier-fill"),
                (now, max) => Texts.Monospace(TopBarWords.BarrierValue(now, max)));
        }

        public void Reset()
        {
            _hull.Reset();
            _barrier.Reset();
        }

        public void Refresh(World world)
        {
            Hull hull = world.Hull;
            _hull.Set(hull.HP, hull.Max);
            Barrier barrier = world.Barrier;
            _barrierGroup.Show(barrier.Capacity > 0f);
            int now = Mathf.CeilToInt(barrier.HP - 0.001f);
            if (_barrier.Set(now, Mathf.RoundToInt(barrier.Capacity), barrier.Fill))
            {
                _barrierValue.EnableInClassList("top-bar__barrier-value--empty", now <= 0);
            }
        }
    }
}
