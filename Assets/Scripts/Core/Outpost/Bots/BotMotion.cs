using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public static class BotMotion
    {
        private const float ArriveTolerance = 0.08f;

        internal static bool Step(World world, Bot bot)
        {
            return Step(world, bot, world.Configuration.BotWalkSpeed);
        }

        internal static bool Step(World world, Bot bot, float walkSpeed)
        {
            if (bot.PathIndex >= bot.Path.Count)
            {
                bot.Position = bot.Goal;
                return true;
            }

            GridPoint next = bot.Path[bot.PathIndex];
            GameConfiguration configuration = world.Configuration;
            float step = walkSpeed * bot.Speed(configuration) * configuration.TickSeconds;
            GridPoint at = Toward(bot.Position, next, step);
            if (System.Math.Abs(next.X - at.X) <= ArriveTolerance && System.Math.Abs(next.Y - at.Y) <= ArriveTolerance)
            {
                at = next;
                bot.PassCorner();
            }

            bot.Position = at;
            return bot.PathIndex >= bot.Path.Count;
        }

        private static GridPoint Toward(GridPoint at, GridPoint next, float step)
        {
            float dx = next.X - at.X;
            if (System.Math.Abs(dx) > ArriveTolerance)
            {
                return new GridPoint(at.X + System.Math.Clamp(dx, -step, step), at.Y);
            }

            return new GridPoint(next.X, at.Y + System.Math.Clamp(next.Y - at.Y, -step, step));
        }
    }
}
