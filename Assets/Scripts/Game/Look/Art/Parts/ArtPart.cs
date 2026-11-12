using System;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public abstract class ArtPart
    {
        public virtual bool DrawsItself => false;

        public virtual bool DrawsAboveFigures => false;

        public virtual bool IsLight => false;

        public virtual bool Place(IArtStage stage, ArtState state, ArtLayer layer)
        {
            return true;
        }

        public virtual float Alpha(float clock)
        {
            return 1f;
        }

        public virtual void Emit(IArtStage stage)
        {
        }

        public virtual void Draw(IArtStage stage, ArtState state, ArtLayer layer)
        {
        }
    }
}
