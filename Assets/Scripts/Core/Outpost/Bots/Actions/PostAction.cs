using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public abstract class PostAction : AssignmentAction
    {
        protected PostAction(WorkPriority priority, Place post, GridPoint spot)
            : base(priority, post, spot)
        {
        }

        protected PostAction(AssignmentActionMemento memento)
            : base(memento)
        {
        }

        public override IEnumerable<PlaceClaim> PlaceClaims
        {
            get
            {
                yield return new PlaceClaim(Target, PlaceUse.Post);
            }
        }

        internal abstract bool IsValid(World world);

        internal bool IsFree(World world)
        {
            return !world.IsOccupied(Target);
        }

        internal bool IsOpen(World world)
        {
            return IsValid(world) && IsFree(world);
        }

        internal override bool CanContinue(World world)
        {
            return IsValid(world);
        }

        internal sealed override void Tick(World world, Bot bot)
        {
            if (!CanContinue(world))
            {
                bot.GoIdle();
                return;
            }

            if (BotMotion.Step(world, bot))
            {
                Work(world, bot);
            }
        }

        protected abstract void Work(World world, Bot bot);
    }
}
