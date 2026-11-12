using System;
using System.Collections.Generic;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Game.UI.Screens.FacilityPanel;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Research
{
    public sealed class ResearchPage : FacilityPage, IRecipeShelfView<bool>
    {
        public const int PrerequisiteSlots = 3;
        public const int InputSlots = 2;
        public const int UnlockSlots = 6;
        public static readonly Vector2 PictureSize = new Vector2(52f, 26f);

        private readonly VisualElement _head;
        private readonly VisualElement _detail;
        private readonly ResearchLines _lines;
        private readonly VisualElement _nodeHost;
        private readonly VisualTreeAsset _nodeAsset;
        private readonly Label _title;
        private readonly ResearchPicture _picture;
        private readonly Label _status;
        private readonly VisualElement[] _prerequisiteTags = new VisualElement[PrerequisiteSlots];
        private readonly Label[] _prerequisiteTexts = new Label[PrerequisiteSlots];
        private readonly ResearchInputs _inputs;
        private readonly ResearchUnlockList _unlocks;
        private readonly Button _act;
        private readonly Label _actLabel;
        private readonly ResearchNowPanel _now;

        private readonly List<ResearchNode> _nodes = new List<ResearchNode>();
        private IReadOnlyList<ResearchTreeEdge> _edges = Array.Empty<ResearchTreeEdge>();
        private GameDefinitions _builtDefinitions;
        private readonly ShownValue<(World, int, Facility, ResearchDefinition)> _builtFor = new ShownValue<(World, int, Facility, ResearchDefinition)>();
        private readonly Action _start;
        private ResearchDefinition _selected;

        public ResearchPage(VisualElement root) : base(root)
        {
            _head = Q("research-legends");
            TreeView = new PanZoomView(Q("tree-port"), Q("tree"));
            _lines = Q<ResearchLines>("lines");
            _nodeHost = Q("nodes");
            _nodeAsset = Root.TakeTemplate("node-template");
            _detail = Q("detail");
            _title = Q<Label>("detail-title");
            _picture = new ResearchPicture(Q("detail-picture"), PictureSize);
            _status = Q<Label>("detail-status");
            for (int i = 0; i < PrerequisiteSlots; i++)
            {
                _prerequisiteTags[i] = Q("pre-" + i);
                _prerequisiteTexts[i] = Q<Label>("pre-text-" + i);
            }

            _inputs = new ResearchInputs(Q("inputs"), Root.TakeTemplate("input-template"));
            _unlocks = new ResearchUnlockList(Q("unlocks"), Root.TakeTemplate("unlock-template"), Q<Label>("unlocks-none"));
            _act = Q<Button>("act");
            _actLabel = Q<Label>("act-label");
            _now = AddPart(new ResearchNowPanel(Q("now")));
            root.WriteLabel("legend-done", ResearchStatus.Done.Text());
            root.WriteLabel("legend-current", ResearchStatus.Current.Text());
            root.WriteLabel("legend-available", ResearchStatus.Available.Text());
            root.WriteLabel("legend-locked", ResearchStatus.Locked.Text());
            root.WriteLabel("detail-none", ResearchWords.DetailNone);
            root.WriteLabel("requires-title", ResearchWords.Requires);
            root.WriteLabel("unlocks-title", ResearchWords.Unlocks);
            root.WriteLabel("unlocks-none", ResearchWords.UnlocksNone);
            _start = () =>
            {
                if (_selected != null)
                {
                    Session.Execute(World.Research.StartOf(_selected));
                }
            };
        }

        public override VisualElement Head => _head;
        public override string PanelClass => "research-panel";

        private PanZoomView TreeView { get; }

        public override VisualElement Revealed => _now.Root;

        public override bool Lists(RecipeDefinition recipe)
        {
            return recipe.Result.ShowShelf(this);
        }

        public bool Task() => false;

        public bool Upgrade() => false;

        public bool Research() => true;

        public override VisualElement FrameOf(RecipeDefinition recipe)
        {
            return _detail;
        }

        public override VisualElement GuideTo(RecipeDefinition recipe, int times, out TutorialStep step)
        {
            ResearchDefinition research = World.Definitions.Research.Find(recipe.Result.Completes);
            if (_selected != null && _selected == research)
            {
                step = TutorialStep.StartResearch;
                return _act;
            }

            step = TutorialStep.ChooseResearch;
            return _nodes.Find(node => node.Definition == research)?.Frame;
        }

        protected override void OnBind()
        {
            _selected = null;
            _builtFor.Reset();
            if (_builtDefinitions != World.Definitions)
            {
                BuildTree(World.Definitions);
            }
        }

        private void BuildTree(GameDefinitions definitions)
        {
            _builtDefinitions = definitions;
            _nodeHost.Clear();
            _nodes.Clear();
            _lines.ClearCurves();
            ResearchTreeLayout layout = ResearchTreeLayout.Build(definitions);
            _edges = layout.Edges;
            foreach (ResearchTreeEdge edge in _edges)
            {
                _lines.Add(edge.Points);
            }

            foreach (ResearchTreeNode node in layout.Nodes)
            {
                TemplateContainer instance = _nodeAsset.Instantiate();
                _nodeHost.Add(instance);
                _nodes.Add(new ResearchNode(instance, Data, node.Definition, node.Position, research => _selected = research));
            }

            TreeView.SetContentSize(layout.Size);
        }

        public override void Open(Facility f)
        {
            TreeView.Fit();
            _selected = null;
            _builtFor.Reset();
        }

        public override void Close()
        {
            TreeView.EndDrag();
            _builtFor.Reset();
        }

        public override void Refresh(Facility f)
        {
            _now.UpdateCancel();
            if (!_builtFor.Change((World, World.Revision, f, _selected)))
            {
                return;
            }

            ShowTree();
            ShowDetail();
            _now.Show();
        }

        private void ShowTree()
        {
            for (int e = 0; e < _edges.Count; e++)
            {
                _lines.SetState(e, LineState(_edges[e]));
            }

            foreach (ResearchNode node in _nodes)
            {
                ResearchDefinition definition = node.Definition;
                ResearchStatus status = World.Research.StatusOf(definition);
                node.Show(status, definition == _selected, status == ResearchStatus.Current ? CurrentReady() : 0f);
            }
        }

        private ResearchLineState LineState(ResearchTreeEdge edge)
        {
            if (_selected != null && edge.To == _selected)
            {
                return ResearchLineState.Lit;
            }

            return World.Research.IsDone(edge.From) ? ResearchLineState.Done : ResearchLineState.Normal;
        }

        private float CurrentReady()
        {
            ResearchWork work = World.Research.FindWork();
            return work != null ? work.Facility.Work.ReadyShare(work.Task) : 0f;
        }

        private void ShowDetail()
        {
            ResearchDefinition definition = _selected;
            _detail.EnableInClassList("research-card--none", definition == null);
            if (definition == null)
            {
                return;
            }

            ResearchStatus status = World.Research.StatusOf(definition);
            _title.text = Data.NameOf(definition);
            _picture.Show(Data, definition);
            _picture.SetDim(status == ResearchStatus.Locked);
            _status.text = status.Text();
            for (int i = 0; i < PrerequisiteSlots; i++)
            {
                bool shown = definition.Requires.Count == 0 ? i == 0 : i < definition.Requires.Count;
                _prerequisiteTags[i].Show(shown);
                if (!shown)
                {
                    continue;
                }

                if (definition.Requires.Count == 0)
                {
                    SetTag(i, ResearchWords.RequiresNone, true);
                    continue;
                }

                ResearchDefinition required = definition.Requires[i].Definition;
                string name = Data.NameOf(required);
                bool done = World.Research.IsDone(required);
                SetTag(i, done ? ResearchWords.RequiresDone(name) : name, done);
            }

            _inputs.Show(Data, World, definition);
            _unlocks.Show(Data, World.Definitions, definition);

            CommandResult start = Session.Preview(World.Research.StartOf(definition)).Own;
            _actLabel.text = World.Research.ActText(definition);
            _act.SetUsable(start.IsOK, _start);
        }

        private void SetTag(int i, string text, bool done)
        {
            _prerequisiteTexts[i].text = text;
            _prerequisiteTags[i].EnableInClassList("research-pre__tag--done", done);
        }

    }
}
