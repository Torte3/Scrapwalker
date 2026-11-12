namespace Scrapwalker.Core.Outpost.Construction
{
    public abstract class SiteJob
    {
        public static readonly SiteJob Build = new BuildJob();
        public static readonly SiteJob Rebuild = new RebuildJob();
        public static readonly SiteJob Reassemble = new ReassembleJob();
        public static readonly SiteJob Dismantle = new DismantleJob();
        public static readonly SiteJob Repair = new RepairJob();

        private SiteJob()
        {
        }

        public abstract T Show<T>(ISiteJobView<T> view);

        private sealed class BuildJob : SiteJob
        {
            public override T Show<T>(ISiteJobView<T> view) => view.Build();

            public override string ToString() => "Build";
        }

        private sealed class RebuildJob : SiteJob
        {
            public override T Show<T>(ISiteJobView<T> view) => view.Rebuild();

            public override string ToString() => "Rebuild";
        }

        private sealed class ReassembleJob : SiteJob
        {
            public override T Show<T>(ISiteJobView<T> view) => view.Reassemble();

            public override string ToString() => "Reassemble";
        }

        private sealed class DismantleJob : SiteJob
        {
            public override T Show<T>(ISiteJobView<T> view) => view.Dismantle();

            public override string ToString() => "Dismantle";
        }

        private sealed class RepairJob : SiteJob
        {
            public override T Show<T>(ISiteJobView<T> view) => view.Repair();

            public override string ToString() => "Repair";
        }
    }
}
