using Scrapwalker.Game.MainView.Outpost.Limbs;

namespace Scrapwalker.Game.MainView.Compose
{
    public abstract class ArmStage
    {
        public static readonly ArmStage Rest = new RestStage();
        public static readonly ArmStage Reaching = new ReachingStage();
        public static readonly ArmStage Digging = new DiggingStage();
        public static readonly ArmStage Hauling = new HaulingStage();

        private ArmStage()
        {
        }

        public virtual bool IsExtending => false;
        public virtual bool IsHauling => false;

        internal abstract ArmPose PoseOf(ArmMotion motion);

        private sealed class RestStage : ArmStage
        {
            internal override ArmPose PoseOf(ArmMotion motion)
            {
                return motion.RestPose();
            }
        }

        private sealed class ReachingStage : ArmStage
        {
            public override bool IsExtending => true;

            internal override ArmPose PoseOf(ArmMotion motion)
            {
                return motion.ReachingPose();
            }
        }

        private sealed class DiggingStage : ArmStage
        {
            internal override ArmPose PoseOf(ArmMotion motion)
            {
                return motion.DiggingPose();
            }
        }

        private sealed class HaulingStage : ArmStage
        {
            public override bool IsHauling => true;

            internal override ArmPose PoseOf(ArmMotion motion)
            {
                return motion.HaulingPose();
            }
        }
    }
}
