namespace Scrapwalker.Game.MainView.Compose
{
    public struct ArmVisual
    {
        private ArmStage _stage;

        public ArmStage Stage
        {
            get => _stage ?? ArmStage.Rest;
            set => _stage = value;
        }

        public float StageProgress;
    }
}
