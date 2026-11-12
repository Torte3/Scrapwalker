using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Research
{
    internal sealed class ResearchNowPanel : UIScreen
    {
        private readonly Label _name;
        private readonly Label _count;
        private readonly JobBar _bar;
        private readonly ResearchInputs _inputs;
        private readonly Label _stop;
        private readonly Button _cancel;
        private readonly System.Action _cancelResearch;

        public ResearchNowPanel(VisualElement root) : base(root)
        {
            _name = Q<Label>("now-name");
            _count = Q<Label>("now-count");
            _bar = new JobBar(Q("now-bar"));
            _inputs = new ResearchInputs(Q("now-mats"), root.TakeTemplate("now-input-template"));
            _stop = Q<Label>("now-stop");
            _cancel = Q<Button>("cancel");
            root.WriteLabel("now-title", ResearchWords.Now);
            root.WriteLabel("cancel-label", ResearchWords.Cancel);
            _cancelResearch = () => Session.Execute(World.Research.CancelOf());
        }

        public void UpdateCancel()
        {
            RemoveWorkTask cancel = World.Research.CancelOf();
            _cancel.SetUsable(cancel != null && Session.Preview(cancel).Own.IsOK, _cancelResearch);
        }

        public void Show()
        {
            ResearchWork work = World.Research.FindWork();
            _name.text = work != null ? Data.NameOf(work.Research) : ResearchWords.NowNone;
            _name.EnableInClassList("research-now__name--none", work == null);
            _count.text = work != null ? ResearchWords.NowCount(work.DeliveredTotal, work.Task.Recipe.InputTotal) : string.Empty;
            if (work != null)
            {
                _bar.Show(World, work.Facility, work.Task);
            }
            else
            {
                _bar.Clear();
            }

            _inputs.Show(Data, World, work?.Research);

            TaskStatus? status = work != null ? work.Facility.StatusOf(World, work.Task) : (TaskStatus?)null;
            bool isStopped = status.HasValue && status.Value.IsStop();
            WorkTask ahead = work != null ? work.Facility.Work.CurrentTask : null;
            bool isWaitingBehind = !isStopped && ahead != null && ahead != work.Task;
            _stop.Show(isStopped || isWaitingBehind);
            if (isStopped)
            {
                _stop.Show(status.Value);
            }
            else if (isWaitingBehind)
            {
                _stop.SetText(WaitingText(ahead));
                _stop.SetSeverity(Severity.Caution);
            }

            _cancel.Show(work != null);
        }

        private string WaitingText(WorkTask ahead)
        {
            return Data.TryNameOf(ahead.Recipe, out string name) ? ResearchWords.NowWaitingFor(name) : ResearchWords.NowWaiting;
        }
    }
}
