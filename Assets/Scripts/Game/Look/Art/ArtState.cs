using Scrapwalker.Game.Look.Art.Parts;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art
{
    public abstract class ArtState
    {
        public abstract float Clock { get; }

        public virtual bool IsActing => true;
        public virtual bool IsWalking => false;
        public virtual bool IsAttacking => false;
        public virtual bool IsCharging => false;
        public virtual bool IsFeeding => false;
        public virtual bool Spins => false;
        public virtual Vector2 Origin => Vector2.zero;
        public virtual int ShotCount => 0;
        public virtual float MoveFrames => 0f;
        public virtual Color? Flash => null;
        protected virtual ArtCue? PlayingMove => null;

        public bool Holds(ArtCondition condition)
        {
            switch (condition)
            {
                case ArtCondition.Act:
                    return IsActing;
                case ArtCondition.NotAct:
                    return !IsActing;
                case ArtCondition.Charging:
                    return IsCharging;
                case ArtCondition.Feeding:
                    return IsFeeding;
                case ArtCondition.Attack:
                    return IsAttacking;
                default:
                    return true;
            }
        }

        public bool Runs(ArtCue cue)
        {
            switch (cue)
            {
                case ArtCue.Always:
                    return true;
                case ArtCue.Move:
                    return IsWalking;
                default:
                    return cue == PlayingMove;
            }
        }

        public bool Runs(ArtLayer layer)
        {
            return Holds(layer.Condition) && Runs(layer.Cue);
        }

        public virtual bool IsShown(ArtLayer layer)
        {
            return Holds(layer.Condition) && layer.Alpha(Clock) > 0f;
        }

        public virtual ArtFrame FrameOf(ArtLayer layer)
        {
            return layer.FrameIn(this);
        }

        public virtual float ValueOf(ArtLayer layer)
        {
            return 0f;
        }

        public virtual Color TintOf(ArtLayer layer)
        {
            return Color.white;
        }

        public virtual float AttackCycle(ArtLayer layer)
        {
            return 0f;
        }

        public virtual float SpunAngle(ArtLayer layer)
        {
            return 0f;
        }

        public virtual float Openness(ArtLayer layer, in ArtOpening opening)
        {
            return 0f;
        }

        public virtual float RecoilLeft(ArtLayer layer, float frames)
        {
            return 0f;
        }

        public virtual bool TryGetCurrentCell(out Vector2 cell)
        {
            cell = default;
            return false;
        }

        public virtual bool TryGetShotCell(int shot, out Vector2 cell)
        {
            cell = default;
            return false;
        }

        public virtual float LaunchFrame(int shot)
        {
            return 0f;
        }
    }
}
