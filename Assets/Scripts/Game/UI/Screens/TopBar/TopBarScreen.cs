using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.UI.Screens.TopBar
{
    public sealed class TopBarScreen : UIScreen
    {
        private readonly HullSection _hull;
        private readonly RuinSection _ruin;
        private readonly WaveSection _wave;
        private readonly PowerSection _power;

        public TopBarScreen(VisualElement root) : base(root)
        {
            Panel = Q("top-bar");
            HullCell = Q("hull-cell");
            WaveCell = Q("wave-cell");
            _hull = new HullSection(root);
            _ruin = new RuinSection(root);
            _wave = new WaveSection(root);
            _power = new PowerSection(root);
            root.WriteLabel("hull-head", TopBarWords.Hull);
            root.WriteLabel("barrier-head", TopBarWords.Barrier);
            root.WriteLabel("time-head", TopBarWords.TimeLeft);
            root.WriteLabel("power-head", TopBarWords.Power);
        }

        public VisualElement Panel { get; }
        public VisualElement HullCell { get; }
        public VisualElement WaveCell { get; }

        protected override void OnBind()
        {
            _hull.Reset();
            _ruin.Reset();
            _wave.Reset();
            _power.Reset(Data);
        }

        public void Refresh()
        {
            _ruin.Refresh(Data, World.Journey);
            _hull.Refresh(World);
            _wave.Refresh(World, App.GameDelta);
            _power.Refresh(World);
        }
    }
}
