using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.BuildList
{
    public sealed class BuildListScreen : UIScreen
    {
        public const int PageSize = 4;
        private const float FadeSeconds = 0.25f;

        private readonly VisualElement _band;
        private readonly VisualElement _list;
        private readonly VisualElement _tape;
        private readonly ButtonChoice<BuildCategory> _tabs;
        private readonly Label[] _tabLabels;
        private readonly Button _back;
        private readonly Button _next;
        private readonly Label _page;
        private readonly BuildCard[] _cards = new BuildCard[PageSize];
        private readonly FacilityDefinition[] _cardDefinitions = new FacilityDefinition[PageSize];
        private readonly Action _turnBack;
        private readonly Action _turnNext;

        private readonly ShownValue<(World, int, QualityFilter, BuildCategory, int, FacilityDefinition)> _builtFor = new ShownValue<(World, int, QualityFilter, BuildCategory, int, FacilityDefinition)>();
        private BuildCategory _tab;
        private bool _isTabChosen;
        private int _pageIndex;
        private float? _fade;

        public BuildListScreen(VisualElement root, VisualElement tape) : base(root)
        {
            _band = Q("build-band");
            _list = Q("build-list");
            _tape = Q<VisualElement>(tape ?? throw new ArgumentNullException(nameof(tape)), "build-tape");
            _back = Q<Button>("back");
            _next = Q<Button>("next");
            _page = Q<Label>("page");

            BuildCategory[] categories = FacilityLooks.Categories;
            _tabs = new ButtonChoice<BuildCategory>("tab--selected", ChooseTab);
            _tabLabels = new Label[categories.Length];
            for (int i = 0; i < categories.Length; i++)
            {
                Button tab = Q<Button>("tab-" + i);
                _tabLabels[i] = Q<Label>(tab, "tab-label");
                _tabs.Add(categories[i], tab);
            }

            for (int i = 0; i < PageSize; i++)
            {
                int slot = i;
                _cards[i] = new BuildCard(Q("card-" + i));
                _cards[i].Button.clicked += () =>
                {
                    if (UIState.IsBuildMode && _cardDefinitions[slot] != null)
                    {
                        UIState.BeginPlace(_cardDefinitions[slot]);
                    }
                };
            }

            _turnBack = () => TurnPage(-1);
            _turnNext = () => TurnPage(1);
        }

        public VisualElement Menu => _band;

        public Button TabOf(BuildCategory category)
        {
            return _tabs.ButtonOf(category);
        }

        public VisualElement GuideTo(FacilityDefinition definition, out TutorialStep step)
        {
            BuildCategory category = CategoryOf(definition);
            if (_tab != category)
            {
                step = TutorialStep.ChooseTab;
                return TabOf(category);
            }

            step = TutorialStep.ChooseCard;
            int index = ListedIn(_tab).IndexOf(definition);
            if (index < 0)
            {
                return null;
            }

            int page = index / PageSize;
            if (page != _pageIndex)
            {
                step = TutorialStep.TurnPage;
                return page > _pageIndex ? _next : _back;
            }

            step = TutorialStep.ChooseCard;
            return _cards[index % PageSize].Button;
        }

        protected override void OnBind()
        {
            _builtFor.Reset();
            _tab = default;
            _isTabChosen = false;
            _pageIndex = 0;
        }

        private void TurnPage(int by)
        {
            if (!UIState.IsBuildMode)
            {
                return;
            }

            _pageIndex = Mathf.Clamp(_pageIndex + by, 0, PageCount(ListedIn(_tab).Count) - 1);
        }

        public void Refresh()
        {
            bool isBuildMode = UIState.IsBuildMode;
            float target = isBuildMode ? 1f : 0f;
            if (_fade != target)
            {
                float fade = _fade.HasValue ? Mathf.MoveTowards(_fade.Value, target, App.UIDelta / FadeSeconds) : target;
                _fade = fade;
                _band.Display(fade > 0f);
                _tape.Display(fade > 0f);
                _list.style.opacity = fade;
                _tape.style.opacity = fade;
            }

            if (!isBuildMode)
            {
                return;
            }

            if (_builtFor.Change((World, World.Revision, UIState.Build.Filter, _tab, _pageIndex, UIState.Build.PlacingDefinition)))
            {
                Rebuild();
            }
        }

        private void ChooseTab(BuildCategory category)
        {
            if (!UIState.IsBuildMode)
            {
                return;
            }

            if (UIState.Build.IsActive)
            {
                UIState.Build.Cancel();
            }

            _tab = category;
            _isTabChosen = true;
            _pageIndex = 0;
        }

        private static int PageCount(int count)
        {
            return Mathf.Max(1, (count + PageSize - 1) / PageSize);
        }

        private BuildCategory CategoryOf(FacilityDefinition definition)
        {
            return Data.Asset(definition).Look.Category;
        }

        private List<FacilityDefinition> ListedIn(BuildCategory category)
        {
            return World.Definitions.Facilities.FindAll(definition => CategoryOf(definition) == category && definition.IsListed(World));
        }

        private void Rebuild()
        {
            if (!_isTabChosen && ListedIn(_tab).Count == 0)
            {
                BuildCategory[] filled = FacilityLooks.Categories.Where(category => ListedIn(category).Count > 0).ToArray();
                if (filled.Length > 0)
                {
                    _tab = filled[0];
                }
            }

            for (int i = 0; i < _tabLabels.Length; i++)
            {
                BuildCategory category = FacilityLooks.Categories[i];
                _tabLabels[i].text = BuildWords.Tab(category.CategoryName(), ListedIn(category).Count);
            }

            _tabs.Select(_tab);

            List<FacilityDefinition> definitions = ListedIn(_tab);
            int pages = PageCount(definitions.Count);
            _pageIndex = Mathf.Clamp(_pageIndex, 0, pages - 1);
            _back.SetUsable(_pageIndex > 0, _turnBack);
            _next.SetUsable(_pageIndex < pages - 1, _turnNext);
            _page.text = pages > 1 ? BuildWords.Page(Texts.Monospace(_pageIndex + 1), Texts.Monospace(pages)) : string.Empty;

            for (int slot = 0; slot < PageSize; slot++)
            {
                int index = _pageIndex * PageSize + slot;
                _cardDefinitions[slot] = index < definitions.Count ? definitions[index] : null;
                if (_cardDefinitions[slot] != null)
                {
                    Fill(_cards[slot], _cardDefinitions[slot]);
                }
                else
                {
                    _cards[slot].SetVisible(false);
                }
            }
        }

        private void Fill(BuildCard card, FacilityDefinition definition)
        {
            QualityFilter filter = UIState.Build.Filter;
            card.SetVisible(true);
            card.Set(Data, definition, new BuildCheck(World, definition, filter).IsBuildable, UIState.Build.PlacingDefinition == definition);

            int materials = Mathf.Min(definition.Cost.Count, BuildCard.LineCount);
            card.Lines.Show(Data, definition.Cost, materials);
            card.Lines.SetCounts(World.Ledger, filter);
            int line = materials;
            if (definition.Cost.Count == 0)
            {
                card.Lines.AddPlain(BuildWords.NoCost, string.Empty);
                line++;
            }

            int? limit = definition.Limit;
            if (limit.HasValue && line < BuildCard.LineCount)
            {
                card.Lines.AddPlain(BuildWords.LimitLabel, BuildWords.Limit(World.Facilities.CountIncludingUnbuilt(definition), limit.Value));
            }
        }
    }
}
