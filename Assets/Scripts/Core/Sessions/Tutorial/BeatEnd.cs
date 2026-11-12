namespace Scrapwalker.Core.Sessions.Tutorial
{
    public abstract class BeatEnd
    {
        public static readonly BeatEnd Await = new AwaitEnd();
        public static readonly BeatEnd Blackout = new BlackoutEnd();
        public static readonly BeatEnd FreePlay = new FreePlayEnd();

        internal abstract void Begin(TutorialFlow flow);

        internal virtual bool CompleteIntroduction(TutorialProgress progress) => false;

        private sealed class AwaitEnd : BeatEnd
        {
            internal override void Begin(TutorialFlow flow)
            {
                flow.AwaitGoal();
            }
        }

        private sealed class BlackoutEnd : BeatEnd
        {
            internal override void Begin(TutorialFlow flow)
            {
                flow.StartBlackout();
            }

            internal override bool CompleteIntroduction(TutorialProgress progress)
            {
                progress.ReplaceWrecks();
                return true;
            }
        }

        private sealed class FreePlayEnd : BeatEnd
        {
            internal override void Begin(TutorialFlow flow)
            {
                flow.Progress.Finish();
            }
        }
    }
}
