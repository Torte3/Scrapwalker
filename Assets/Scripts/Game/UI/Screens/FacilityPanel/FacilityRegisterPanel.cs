using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityRegisterPanel : UIScreen
    {
        private const int MaxTimes = 99;

        private readonly RowList<FacilityRecipeRow> _taskRows;
        private readonly Label _taskEmpty;
        private readonly VisualElement _upgradeGroup;
        private readonly RowList<FacilityRecipeRow> _upgradeRows;
        private readonly Label _upgradeEmpty;
        private readonly RowList<FacilityDoneRow> _doneRows;
        private readonly Label _doneEmpty;
        private readonly ScrollView _scroll;
        private readonly List<FacilityRecipeRow> _recipes = new List<FacilityRecipeRow>();
        private readonly List<FacilityRecipeRow> _entries = new List<FacilityRecipeRow>();
        private readonly List<FacilityEffect> _effects = new List<FacilityEffect>();
        private readonly RecipeShelves _shelves = new RecipeShelves();
        private readonly Action<QualityFilter> _applyEntryFilter;
        private readonly ShownValue<(World, int, Facility, RecipeDefinition, bool)> _builtFor = new ShownValue<(World, int, Facility, RecipeDefinition, bool)>();

        private RecipeDefinition _open;
        private int _times = 1;
        private QualityFilter _filter = QualityFilter.Any;
        private bool _filterOpen;

        public FacilityRegisterPanel(VisualElement root, FacilityTemplates templates) : base(root)
        {
            Func<FacilityRecipeRow> recipe = () => new FacilityRecipeRow(templates.Recipe.Instantiate(), templates.Material);
            Func<FacilityRecipeRow, VisualElement> recipeElement = row => row.Element;
            _taskRows = new RowList<FacilityRecipeRow>(Q("task-rows"), recipe, recipeElement);
            _taskEmpty = Q<Label>("task-empty");
            _taskEmpty.text = RoomWords.EmptyRegister;
            root.WriteLabel("task-title", RoomWords.SectionTasks);
            root.WriteLabel("upgrade-title", RoomWords.SectionUpgrades);
            root.WriteLabel("done-title", RoomWords.SectionDoneUpgrades);
            _upgradeGroup = Q("upgrade-group");
            _upgradeRows = new RowList<FacilityRecipeRow>(Q("upgrade-rows"), recipe, recipeElement);
            _upgradeEmpty = Q<Label>("upgrade-empty");
            _upgradeEmpty.text = RoomWords.NoUpgrades;
            _doneRows = new RowList<FacilityDoneRow>(Q("done-rows"), () => new FacilityDoneRow(templates.Done.Instantiate()), row => row.Element);
            _doneEmpty = Q<Label>("done-empty");
            _doneEmpty.text = RoomWords.NoDoneUpgrades;
            _scroll = Q<ScrollView>("register");
            _applyEntryFilter = chosen => _filter = chosen;
        }

        public void Reset()
        {
            _open = null;
            _scroll.scrollOffset = Vector2.zero;
            ResetEntry();
            _builtFor.Reset();
        }

        private void ResetEntry()
        {
            _times = 1;
            _filter = QualityFilter.Any;
            _filterOpen = false;
        }

        public void Refresh(Facility f)
        {
            if (_builtFor.Change((World, World.Revision, f, _open, _filterOpen)))
            {
                Fill(f);
            }

            foreach (FacilityRecipeRow row in _entries)
            {
                CommandResult block = Session.Preview(new AddWorkTask(f.ID, row.Recipe.ID)).Own;
                row.SetBlocked(block.IsOK ? string.Empty : block.Text(Data));
                if (row.IsOpen)
                {
                    UpdateEntry(f, row);
                }
            }
        }

        public VisualElement GuideToRecipe(RecipeDefinition recipe, int times, out TutorialStep step)
        {
            step = StepToward(recipe, times);
            FacilityRecipeRow row = FindRow(recipe);
            if (row == null)
            {
                return null;
            }

            switch (step)
            {
                case TutorialStep.OpenRecipe:
                    return row.Head;
                case TutorialStep.MoreTimes:
                    return row.More;
                case TutorialStep.FewerTimes:
                    return row.Fewer;
                default:
                    return row.Once;
            }
        }

        public VisualElement RowOf(RecipeDefinition recipe)
        {
            return FindRow(recipe)?.Box;
        }

        private FacilityRecipeRow FindRow(RecipeDefinition recipe)
        {
            return recipe != null ? _recipes.Find(r => r.Recipe == recipe) : null;
        }

        private TutorialStep StepToward(RecipeDefinition recipe, int times)
        {
            if (_open != recipe)
            {
                return TutorialStep.OpenRecipe;
            }

            int wanted = recipe != null && recipe.IsOneOff ? 1 : times;
            if (_times < wanted)
            {
                return TutorialStep.MoreTimes;
            }

            return _times > wanted ? TutorialStep.FewerTimes : TutorialStep.RunOnce;
        }

        private void Fill(Facility f)
        {
            _recipes.Clear();
            _entries.Clear();
            _shelves.Sort(f.Definition.Recipes);
            FillListed(_taskRows, f, _shelves.Tasks);
            _taskEmpty.Display(_taskRows.Count == 0);

            List<RecipeDefinition> upgrades = _shelves.Upgrades;
            _upgradeGroup.Display(upgrades.Count > 0);
            FillListed(_upgradeRows, f, upgrades);
            FillDone(f, upgrades);
            _upgradeEmpty.Display(_upgradeRows.Count == 0);
            _doneEmpty.Display(_doneRows.Count == 0);
        }

        private void FillListed(RowList<FacilityRecipeRow> rows, Facility f, List<RecipeDefinition> recipes)
        {
            rows.Begin();
            foreach (RecipeDefinition recipe in recipes)
            {
                if (recipe.Offer(World, f).IsListed)
                {
                    BindEntry(NextRecipeRow(rows), f, recipe);
                }
            }

            rows.End();
        }

        private void FillDone(Facility f, List<RecipeDefinition> upgrades)
        {
            _doneRows.Begin();
            foreach (RecipeDefinition upgrade in upgrades)
            {
                if (f.Work.Upgrades.Has(upgrade.ID))
                {
                    _doneRows.Next().Show(Data.NameOf(upgrade));
                }
            }

            _doneRows.End();
        }

        private FacilityRecipeRow NextRecipeRow(RowList<FacilityRecipeRow> rows)
        {
            FacilityRecipeRow row = rows.Next();
            _recipes.Add(row);
            return row;
        }

        private void BindEntry(FacilityRecipeRow row, Facility f, RecipeDefinition recipe)
        {
            bool open = _open == recipe;
            _entries.Add(row);
            row.Recipe = recipe;
            row.SetHead(Data.NameOf(recipe), RoomWords.RecipeSeconds(Texts.Seconds(recipe.Seconds)), open);
            row.Toggled = () =>
            {
                _open = _open == recipe ? null : recipe;
                ResetEntry();
            };
            if (!open)
            {
                return;
            }

            row.Materials.ShowWithEnergy(Data, recipe.Inputs, recipe.Energy);
            row.Fewer.OnClick(() => _times = Mathf.Max(1, _times - 1));
            row.More.OnClick(() => _times = Mathf.Min(MaxTimes, _times + 1));
            row.Filter.OnToggle(() => _filterOpen = !_filterOpen);
            row.OnRegister(() => Register(f, recipe, false), () => Register(f, recipe, true));
            row.SetTimes(_times, recipe.IsOneOff);
        }

        private void Register(Facility f, RecipeDefinition recipe, bool repeat)
        {
            Session.Execute(new AddWorkTask(f.ID, recipe.ID, TimesOf(recipe), repeat, _filter));
            _open = null;
        }

        private void UpdateEntry(Facility f, FacilityRecipeRow row)
        {
            RecipeDefinition recipe = row.Recipe;
            int times = TimesOf(recipe);
            row.Materials.SetCounts(World.Ledger, _filter, times);
            row.Materials.SetEnergy(Data, recipe.Energy * times, World.Energy.Stored, false);

            new RecipeOutcome(World, Data, f, row, times, _effects).Show(recipe);
            CommandResult check = Session.Preview(new AddWorkTask(f.ID, recipe.ID, times, false, _filter)).Own;
            row.SetReject(check.IsOK ? string.Empty : check.Text(Data));
            row.Filter.Refresh(recipe.Inputs.Count > 0, _filter, _filterOpen, World.Progress, _applyEntryFilter);
            row.SetTimes(_times, recipe.IsOneOff);
        }

        private int TimesOf(RecipeDefinition recipe)
        {
            return recipe.IsOneOff ? 1 : _times;
        }
    }
}
