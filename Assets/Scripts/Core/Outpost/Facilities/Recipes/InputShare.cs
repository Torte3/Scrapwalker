using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    public readonly struct InputShare
    {
        public InputShare(WorkTask task, MaterialAmount need, int fed)
        {
            Task = task;
            Need = need;
            Fed = fed;
        }

        public WorkTask Task { get; }
        public MaterialAmount Need { get; }
        public int Fed { get; }
    }
}
