using System;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.UI.Widgets.QualityFilters;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Placing
{
    public sealed class PlacingScreen : UIScreen
    {
        private readonly VisualElement _panel;
        private readonly VisualElement _move;
        private readonly Label _moveSize;
        private readonly Label _moveName;
        private readonly VisualElement _build;
        private readonly Label _buildHead;
        private readonly Label _buildName;
        private readonly ArtThumb _preview;
        private readonly VisualElement _need;
        private readonly MaterialList _costs;
        private readonly Label _quality;
        private readonly Label _block;
        private readonly VisualElement _filterSlot;
        private readonly QualityFilterPicker _filter;
        private readonly Action<QualityFilter> _applyFilter;
        private bool _isFilterOpen;

        private readonly ShownValue<(World, int, QualityFilter, FacilityDefinition, Facility)> _builtFor = new ShownValue<(World, int, QualityFilter, FacilityDefinition, Facility)>();

        public PlacingScreen(VisualElement root) : base(root)
        {
            _panel = Q("placing");
            _move = Q("move");
            _moveSize = Q<Label>("move-size");
            _moveName = Q<Label>("move-name");
            _build = Q("build");
            _buildHead = Q<Label>("build-head");
            _buildName = Q<Label>("build-name");
            _preview = new ArtThumb(Q("preview"));
            _need = Q("need");
            _costs = new MaterialList(Q("costs"), Root.TakeTemplate("cost-row-template"));
            _quality = Q<Label>("quality");
            _block = Q<Label>("block");
            root.WriteLabel("back-label", PlacingWords.Back);
            root.WriteLabel("move-title", PlacingWords.Move);
            root.WriteLabel("build-title", PlacingWords.Build);
            root.WriteLabel("need-title", PlacingWords.Need);
            root.WriteLabel("use-quality-key", PlacingWords.UseQuality);
            root.WriteLabel("result-title", PlacingWords.Result);
            root.WriteLabel("quality-key", PlacingWords.Quality);
            _filterSlot = Q("filter-slot");
            _filter = new QualityFilterPicker(Q("filter-icon"), Q("filter-editor"));
            _filter.OnToggle(() => _isFilterOpen = !_isFilterOpen);
            _applyFilter = chosen => UIState.Build.Filter = chosen;
            Q<Button>("back").clicked += () => UIState.Build.Cancel();
        }

        protected override void OnBind()
        {
            _builtFor.Reset();
            _isFilterOpen = false;
        }

        public void Refresh()
        {
            FacilityDefinition definition = UIState.Build.PlacingDefinition;
            Facility moving = UIState.Build.Relocating;
            bool shown = definition != null || moving != null;
            _panel.Display(shown);
            if (!shown)
            {
                return;
            }

            if (definition != null)
            {
                RefreshFilter();
            }

            if (!_builtFor.Change((World, World.Revision, UIState.Build.Filter, definition, moving)))
            {
                return;
            }

            _move.Display(definition == null);
            _build.Display(definition != null);
            if (definition == null)
            {
                _moveSize.text = moving.Definition.Size();
                _moveName.text = Data.NameOf(moving.Definition);
                return;
            }

            Fill(definition);
        }

        private void RefreshFilter()
        {
            _filter.Refresh(true, UIState.Build.Filter, _isFilterOpen, World.Progress, _applyFilter);
            _filterSlot.Display(_filter.IsShown);
        }

        private void Fill(FacilityDefinition definition)
        {
            BuildCategory category = Data.Asset(definition).Look.Category;
            _buildHead.text = PlacingWords.Head(category.CategoryName(), definition.Size());
            _buildName.text = Data.NameOf(definition);
            _preview.Show(definition, Data.Asset(definition).Look);

            QualityFilter filter = UIState.Build.Filter;
            _need.Display(definition.Cost.Count > 0);
            FillCosts(definition, filter);

            Quality? average = World.Ledger.PreviewQuality(definition, filter);
            _quality.text = average.HasValue ? average.Value.Tier() : PlacingWords.NoQuality;
            _quality.SetQuality(average);

            var check = new BuildCheck(World, definition, filter);
            CommandResult allowed = check.Allowed;
            _block.text = !allowed.IsOK ? allowed.Text(Data) : string.Empty;
            _block.Display(!allowed.IsOK);
        }

        private void FillCosts(FacilityDefinition definition, QualityFilter filter)
        {
            _costs.Show(Data, definition.Cost);
            _costs.SetCounts(World.Ledger, filter);
        }
    }
}
