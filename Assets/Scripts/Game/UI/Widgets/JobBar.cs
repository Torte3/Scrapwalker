using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Root;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class JobBar
    {
        private readonly VisualElement _bar;
        private readonly FillBar _ready;
        private readonly FillBar _progress;

        public JobBar(VisualElement bar)
        {
            _bar = bar;
            _ready = new FillBar(bar.NeedClass("job-bar__ready"));
            _progress = new FillBar(bar.NeedClass("job-bar__progress"));
        }

        public void Show(World world, Facility f, WorkTask task)
        {
            Set(f.Work.ReadyShare(task), f.Work.ProgressOf(task), f.StatusOf(world, task) == TaskStatus.Advancing);
        }

        public void Clear()
        {
            Set(0f, 0f, false);
        }

        public void Set(float ready, float progress, bool active)
        {
            _bar.EnableInClassList("job-bar--active", active);
            _ready.Set(ready);
            _progress.Set(progress);
        }
    }
}
