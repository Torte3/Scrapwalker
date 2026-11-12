using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Compose
{
    public sealed class WorldComposer
    {
        private readonly GameData _data;
        private readonly FacilityJobReader _jobs = new FacilityJobReader();
        private readonly BotActivityReader _activities = new BotActivityReader();

        public WorldComposer(GameData data)
        {
            _data = data;
        }

        public void Compose(World world, in WorldInput input, FrameVisual frame)
        {
            frame.Begin(world);
            foreach (Facility facility in world.Facilities)
            {
                FacilityVisual visual = FacilityOf(world, facility);
                frame.Facilities.Add(visual);
                frame.Occupy(visual.Rect);
            }

            foreach (Bot bot in world.Bots)
            {
                frame.Bots.Add(new BotVisual { Bot = bot, Activity = ActivityOf(world, frame, bot) });
            }

            frame.RadarLook = _data.RadarLookOf(world);
            frame.Wake = input.AIWake;
            frame.Travel = TravelOf(world, input.TickShare);
            frame.Arm = ArmOf(world, input.TickShare);
            frame.Launch = LaunchOf(world, input.TickShare);
            frame.Overlay = OverlayOf(input.Selected, input.Marked, input.Ghost);
        }

        private FacilityVisual FacilityOf(World world, Facility facility)
        {
            FacilityLook look = _data.Asset(facility.Definition).Look;
            var visual = new FacilityVisual
            {
                Facility = facility,
                Look = look,
                Rect = new FacilityRect(facility.Rect),
                Job = _jobs.Read(facility),
                IsWorking = facility.IsWorking(world),
                IsOverLimit = facility.Condition.IsUnderConstruction && !facility.BuildAllowed(world),
                IsRecovering = facility.IsRecovering(world),
                HasChargingBot = facility.IsInUse(world),
                GaugeFill = facility.GaugeFill(world),
            };
            visual.Task = facility.IsOperational && !visual.Job.IsUnderWay ? facility.Work.CurrentTask : null;
            visual.Worker = WorkerOf(world, facility, look, visual.Rect);
            return visual;
        }

        private static FacilityWorkerVisual? WorkerOf(World world, Facility facility, FacilityLook look, in FacilityRect rect)
        {
            Bot worker = facility.WorkerInPicture(world);
            if (worker == null)
            {
                return null;
            }

            return new FacilityWorkerVisual { Bot = worker, Stand = StandOf(rect, look) };
        }

        private static Vector2 StandOf(in FacilityRect facility, FacilityLook look)
        {
            StationLook station = look.Station;
            float floor = station.OnDeck ? facility.Top + WorldScene.CellSize : facility.Floor;
            return new Vector2(facility.CenterX + station.Dx, floor - WorldScene.BotFootLift);
        }

        private BotActivity ActivityOf(World world, FrameVisual frame, Bot bot)
        {
            if (bot.Action.IsHandingOver(world, bot, out Facility facility, out float handOver))
            {
                return BotActivity.Stoking(new FacilityRect(facility.Rect), _data.Asset(facility.Definition).Look, handOver);
            }

            return _activities.Read(frame, bot);
        }

        private static TravelVisual TravelOf(World world, float tickShare)
        {
            Journey journey = world.Journey;
            var visual = new TravelVisual { HasRoute = journey.Route.Total > 0 };
            if (visual.HasRoute)
            {
                visual.RouteFrames = journey.Route.Total * world.Configuration.TickSeconds * WorldScene.FramesPerSecond;
                visual.RouteProgress = TravelVisual.RouteProgressOf(journey.Route.Remaining, journey.Route.Total, tickShare);
            }

            return visual;
        }

        private static LaunchVisual LaunchOf(World world, float tickShare)
        {
            RuinDefinition ruin = world.Journey.CurrentRuin;
            GameConfiguration configuration = world.Configuration;
            Takeoff takeoff = world.Takeoff;
            float framesPerTick = configuration.TickSeconds * WorldScene.FramesPerSecond;
            bool isCharging = ruin != null && ruin.Plan.IsCharging(world);
            return new LaunchVisual
            {
                IsCharging = isCharging,
                ChargeFrames = isCharging ? (world.Visit.SinceLanding.Ticks + tickShare) * framesPerTick : 0f,
                ChargeEndFrames = isCharging ? ruin.Plan.ChargeTicks(configuration, ruin) * framesPerTick : 0f,
                IsLaunching = takeoff.IsActive,
                LaunchFrames = (takeoff.ElapsedTicks + (takeoff.IsActive && !takeoff.IsOver ? tickShare : 0f)) * framesPerTick,
                IsLaunchOver = takeoff.IsOver,
            };
        }

        private static ArmVisual ArmOf(World world, float tickShare)
        {
            return new ArmVisual
            {
                Stage = world.Visit.Phase.Show(new ArmStages(world.Journey.CurrentRuin)),
                StageProgress = world.Visit.ArmProgress(world.Configuration, tickShare),
            };
        }

        private static OverlayVisual OverlayOf(Facility selected, Facility marked, BuildGhost? ghost)
        {
            var visual = new OverlayVisual { Ghost = ghost };
            if (selected != null)
            {
                visual.Selected = new FacilityRect(selected.Rect);
            }

            if (marked != null && marked != selected)
            {
                visual.Marked = new FacilityRect(marked.Rect);
            }

            return visual;
        }
    }
}
