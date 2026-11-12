using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    internal sealed class IdleAction : BotAction
    {
        private const float EdgeMargin = 0.5f;

        private readonly Countdown _rest = new Countdown();

        public IdleAction()
            : base(Place.Storage, default)
        {
        }

        internal IdleAction(IdleMemento memento)
            : base(memento)
        {
            _rest.Restore(memento.Rest);
        }

        public override bool IsIdle => true;

        internal override BotActionMemento CreateMemento()
        {
            return Written(new IdleMemento { Rest = _rest.CreateMemento() });
        }

        public override T Show<T>(IBotActionView<T> view)
        {
            return view.Idle();
        }

        internal override void Tick(World world, Bot bot)
        {
            GameConfiguration configuration = world.Configuration;
            if (bot.IsMoving)
            {
                BotMotion.Step(world, bot, configuration.IdleWalkSpeed);
                return;
            }

            if (!_rest.IsDone)
            {
                _rest.CountDownOne();
                return;
            }

            bot.TryHeadTo(world, this, NextSpot(world, bot));
            float restSeconds = configuration.IdleRestMinSeconds
                + GameRandom.Value() * (configuration.IdleRestMaxSeconds - configuration.IdleRestMinSeconds);
            _rest.Start(configuration.TicksFor(restSeconds));
        }

        private static GridPoint NextSpot(World world, Bot bot)
        {
            float share = GameRandom.Value();
            if (bot.Station != null)
            {
                return bot.Station.Front(share);
            }

            float range = world.Configuration.IdleWanderCells;
            float x = System.Math.Clamp(bot.Position.X + (share * 2f - 1f) * range, EdgeMargin, world.Columns - EdgeMargin);
            return new GridPoint(x, (float)System.Math.Round(bot.Position.Y));
        }

        internal override bool UsesBattery(World world, Bot bot)
        {
            return false;
        }
    }
}
