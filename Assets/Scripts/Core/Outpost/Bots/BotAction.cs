using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public abstract class BotAction
    {
        protected BotAction(Place target, GridPoint spot)
        {
            Target = target;
            Spot = spot;
        }

        protected BotAction(BotActionMemento memento)
            : this(Place.Restore(memento.Target), GridPoint.Restore(memento.Spot))
        {
        }

        public Place Target { get; }
        public GridPoint Spot { get; }
        public virtual bool IsIdle => false;
        public virtual IEnumerable<PlaceClaim> PlaceClaims => Array.Empty<PlaceClaim>();
        public virtual IEnumerable<MaterialClaim> MaterialClaims => Array.Empty<MaterialClaim>();
        internal virtual bool GivesWayToCharging => true;

        internal abstract BotActionMemento CreateMemento();

        protected T Written<T>(T memento) where T : BotActionMemento
        {
            memento.Target = Target.CreateMemento();
            memento.Spot = Spot.CreateMemento();
            return memento;
        }

        public abstract T Show<T>(IBotActionView<T> view);

        public bool Holds(Place place, PlaceUse use)
        {
            return PlaceClaims.Any(claim => claim.Place == place && claim.Use == use);
        }

        public int Bringing(Place destination, MaterialDefinition type, int? task)
        {
            return MaterialClaims
                .Where(claim => claim.Flow == MaterialFlow.Bring && claim.Place == destination && claim.Type == type && (!task.HasValue || claim.Task == task))
                .Sum(claim => claim.Count);
        }

        internal virtual GridPoint SpotFor(World world, Bot bot)
        {
            return Spot;
        }

        internal virtual bool CanContinue(World world)
        {
            return true;
        }

        internal abstract void Tick(World world, Bot bot);

        internal virtual bool UsesBattery(World world, Bot bot)
        {
            return true;
        }

        internal virtual void OnFacilityMoved(World world, Bot bot, Facility facility)
        {
            if (Target.IsAt(facility))
            {
                bot.GoIdle();
            }
        }

        public virtual bool IsHandingOver(World world, Bot bot, out Facility facility, out float progress)
        {
            facility = null;
            progress = 0f;
            return false;
        }

        internal virtual string LogText(World world)
        {
            return GetType().Name + " " + Target.LogName(world);
        }
    }
}
