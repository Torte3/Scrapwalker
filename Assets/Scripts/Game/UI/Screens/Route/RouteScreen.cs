using System.Linq;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Route
{
    public sealed class RouteScreen : UIScreen
    {
        private readonly WindowFrame _frame;
        private readonly ScrollView _list;
        private readonly RowList<DestinationCard> _cards;
        private readonly ShownValue<(World, int)> _builtFor = new ShownValue<(World, int)>();
        private string _alignedFor;
        private bool _aligning;

        public RouteScreen(VisualElement root) : base(root)
        {
            _frame = new WindowFrame(Q("route-window"));
            _list = Q<ScrollView>("route-list");
            VisualTreeAsset card = Root.TakeTemplate("card-template");
            _cards = new RowList<DestinationCard>(_list, () => NewCard(card), c => c.Root);
        }

        public bool IsOpen => _frame.IsOpen;
        public VisualElement Frame => _frame.Frame;

        protected override void OnBind()
        {
            _builtFor.Reset();
            _alignedFor = null;
            if (IsOpen)
            {
                _frame.Close();
            }
        }

        public VisualElement LandButtonOf(RuinDefinition ruin)
        {
            if (!IsOpen)
            {
                return null;
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].Ruin == ruin)
                {
                    return _cards[i].Land;
                }
            }

            return null;
        }

        public VisualElement CardOf(RuinDefinition ruin)
        {
            if (!IsOpen || ruin == null)
            {
                return null;
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].Ruin == ruin)
                {
                    return _cards[i].Root;
                }
            }

            return null;
        }

        public void Refresh(bool isShown, RuinDefinition showing)
        {
            if (isShown != IsOpen)
            {
                if (isShown)
                {
                    _frame.Open(RouteWords.Title);
                    _frame.SetCloseAction(UIState.CloseRoute);
                    _list.scrollOffset = UnityEngine.Vector2.zero;
                    _builtFor.Reset();
                }
                else
                {
                    _frame.Close();
                }
            }

            if (!IsOpen)
            {
                return;
            }

            if (_builtFor.Change((World, World.Revision)))
            {
                Rebuild();
            }

            if (_aligning)
            {
                AlignNames();
            }

            VisualElement card = CardOf(showing);
            if (card != null && card.IsLaidOut())
            {
                _list.ScrollTo(card);
            }
        }

        private void Rebuild()
        {
            _cards.Begin();
            foreach (RuinDefinition ruin in World.Definitions.Ruins)
            {
                if (World.Exploration.IsOpen(ruin))
                {
                    _cards.Next().Fill(Data, ruin, World.Exploration.HasCleared(ruin), DestinationStatus.Of(World, ruin));
                }
            }

            _cards.End();
            string shown = string.Join("|", Enumerable.Range(0, _cards.Count).Select(i => _cards[i].Ruin.ID));
            if (shown != _alignedFor)
            {
                _alignedFor = shown;
                _aligning = true;
                for (int i = 0; i < _cards.Count; i++)
                {
                    _cards[i].SetNameWidth(StyleKeyword.Null);
                }
            }
        }

        private void AlignNames()
        {
            float widest = 0f;
            for (int i = 0; i < _cards.Count; i++)
            {
                float width = _cards[i].NameWidth;
                if (float.IsNaN(width) || width <= 0f)
                {
                    return;
                }

                widest = UnityEngine.Mathf.Max(widest, width);
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].SetNameWidth(UnityEngine.Mathf.Ceil(widest));
            }

            _aligning = false;
        }

        private DestinationCard NewCard(VisualTreeAsset template)
        {
            var card = new DestinationCard(template.Instantiate());
            card.Land.clicked += () => Choose(new TravelTo(card.Ruin));
            card.Repeat.clicked += () => Choose(TravelTo.Repeating(card.Ruin));
            return card;
        }

        private void Choose(TravelTo travel)
        {
            if (Session.Execute(travel).IsOK)
            {
                UIState.CloseRoute();
            }
        }
    }
}
