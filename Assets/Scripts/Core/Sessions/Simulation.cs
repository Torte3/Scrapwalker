using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Sessions
{
    public sealed class Simulation
    {
        public World World { get; }

        public Simulation(World world)
        {
            World = world;
            DefeatSystem.Listen(world);
        }

        public void Tick()
        {
            World.TickCount++;
            World.Revision++;
            bool isDefeated = World.IsDefeated;
            if (World.DefeatBlackout.IsActive)
            {
                DefeatSystem.Tick(World);
            }

            if (isDefeated)
            {
                return;
            }

            World.Energy.Tick();
            World.Barrier.Recharge();
            foreach (Facility facility in World.Facilities)
            {
                facility.Regenerate();
            }

            BotSystem.Tick(World);
            WorkSystem.Tick(World);

            World.Takeoff.Tick();
            JourneySystem.Tick(World);
            RuinVisitSystem.Tick(World);
            WeaponSystem.Tick(World);
            EnemySystem.Tick(World);
        }

        public CommandResult Execute(Command command)
        {
            CommandResult gate = CheckRule(command);
            if (!gate.IsOK)
            {
                return gate;
            }

            CommandResult result = command.Apply(World);
            if (result.IsOK)
            {
                World.Revision++;
                World.Log.Write(() => "コマンド: " + command.LogText);
            }

            return result;
        }

        public CommandPreview Preview(Command command)
        {
            return new CommandPreview(CheckRule(command), command.Check(World));
        }

        private CommandResult CheckRule(Command command)
        {
            if (World.IsDefeated)
            {
                return CommandResult.Fail(CommandError.GameOver);
            }

            if (World.Takeoff.IsActive)
            {
                return CommandResult.Fail(CommandError.TakingOff);
            }

            CommandRule rule = World.CommandRule;
            if (!command.IsAllowedBy(rule, World))
            {
                return CommandResult.Fail(rule.Refusal);
            }

            return CommandResult.Success();
        }
    }
}
