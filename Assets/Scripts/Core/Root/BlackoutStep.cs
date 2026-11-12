namespace Scrapwalker.Core.Root
{
    public abstract class BlackoutStep
    {
        public static readonly BlackoutStep None = new NoneStep();
        public static readonly BlackoutStep Darkening = new DarkeningStep();
        public static readonly BlackoutStep Dark = new DarkStep();
        public static readonly BlackoutStep Brightening = new BrighteningStep();

        private BlackoutStep()
        {
        }

        public virtual bool IsActive => true;
        public virtual bool IsCovering => false;
        public virtual bool IsDark => false;
        internal virtual bool SwapsSceneAfter => false;

        public abstract float Darkness(float progress);

        private sealed class NoneStep : BlackoutStep
        {
            public override bool IsActive => false;

            public override float Darkness(float progress)
            {
                return 0f;
            }
        }

        private sealed class DarkeningStep : BlackoutStep
        {
            public override bool IsCovering => true;

            public override float Darkness(float progress)
            {
                return progress;
            }
        }

        private sealed class DarkStep : BlackoutStep
        {
            public override bool IsCovering => true;
            public override bool IsDark => true;
            internal override bool SwapsSceneAfter => true;

            public override float Darkness(float progress)
            {
                return 1f;
            }
        }

        private sealed class BrighteningStep : BlackoutStep
        {
            public override float Darkness(float progress)
            {
                return 1f - progress;
            }
        }
    }
}
