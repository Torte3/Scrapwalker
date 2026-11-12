using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Facilities.Storage;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Outpost.Hulls;
using Scrapwalker.Core.Outpost.Power;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Root
{
    public sealed class World
    {
        private readonly List<Bot> _bots = new List<Bot>();
        private int _nextFacilityID = 1;
        private int _nextBotID = 1;

        public GameDefinitions Definitions { get; }
        public GameConfiguration Configuration { get; }
        internal GameEvents Events { get; } = new GameEvents();
        public int TickCount { get; internal set; }
        public int Revision { get; internal set; }
        public int Columns { get; }
        public int Rows { get; }
        public Stock Stock { get; }
        public EnergyStore Energy { get; }
        public Barrier Barrier { get; }
        public Journey Journey { get; } = new Journey();
        public WaveState Wave { get; } = new WaveState();
        public RuinVisit Visit { get; } = new RuinVisit();
        public Battle Battle { get; }
        public Blackout DefeatBlackout { get; } = new Blackout();
        public bool IsDefeated => DefeatBlackout.Step.IsCovering;
        public Takeoff Takeoff { get; }
        public OutpostUpgrades Upgrades { get; }
        public Exploration Exploration { get; }
        public Progress Progress { get; }
        public ResearchState Research { get; }
        public bool InCombat => Journey.IsAtRuin && !IsDefeated;
        public TimeOfDay TimeOfDay => Upgrades.Traits.TimeOfDay;
        public bool CanSave => Journey.IsTraveling && CommandRule.AllowsSaving;
        public CommandRule CommandRule { get; internal set; } = CommandRule.All;
        public BotAssigner Assigner { get; } = new BotAssigner();
        public Floor Floor { get; }
        public Hull Hull { get; }
        public Navigator Paths { get; }
        public Ledger Ledger { get; }
        public GameLog Log { get; }
        public FacilityList Facilities { get; } = new FacilityList();
        public IReadOnlyList<Bot> Bots => _bots;

        public CellRect Grid => new CellRect(new Cell(0, 0), Columns, Rows);
        public GridPoint Intake => new GridPoint(Columns - 0.5f, 0f);

        public World(GameDefinitions definitions)
        {
            Definitions = definitions;
            Configuration = definitions.Configuration;
            Upgrades = new OutpostUpgrades(Configuration.StartEffects);
            Exploration = new Exploration(Upgrades);
            Progress = new Progress(this);
            Stock = new Stock(this);
            Energy = new EnergyStore(this);
            Barrier = new Barrier(this);
            Battle = new Battle(this);
            Research = new ResearchState(this);
            Columns = Configuration.GridColumns;
            Rows = Configuration.GridRows;
            Floor = new Floor(this);
            Hull = new Hull(this);
            Paths = new Navigator(this);
            Ledger = new Ledger(this);
            Log = new GameLog(() => TickCount);
            SalvageSystem.Listen(this);
            RuinVisitSystem.Listen(this);
            JourneySystem.Listen(this);
            Takeoff = new Takeoff(this);
        }

        internal Facility AddFacility(FacilityDefinition definition, Cell cell)
        {
            var facility = new Facility(_nextFacilityID++, definition, cell, Configuration);
            Facilities.Add(facility);
            return facility;
        }

        internal void RemoveFacility(Facility facility)
        {
            Facilities.Remove(facility);
            facility.IsRemoved = true;
        }

        internal WorldMemento CreateMemento()
        {
            return new WorldMemento
            {
                Tick = TickCount,
                NextFacilityID = _nextFacilityID,
                NextBotID = _nextBotID,
                Journey = Journey.CreateMemento(),
                Exploration = Exploration.CreateMemento(),
                Progress = Progress.CreateMemento(),
                Research = Research.CreateMemento(),
                Upgrades = Upgrades.CreateMemento(),
                Energy = Energy.CreateMemento(),
                Barrier = Barrier.CreateMemento(),
                Hull = Hull.CreateMemento(),
                Facilities = Facilities.Select(f => f.CreateMemento()).ToList(),
                Floor = Floor.CreateMemento(),
                Bots = _bots.Select(bot => bot.CreateMemento()).ToList(),
            };
        }

        // ボットの行動は床の資材を指す。床の後に戻す
        internal void Restore(WorldMemento memento, LoadContext load)
        {
            TickCount = memento.Tick;
            _nextFacilityID = memento.NextFacilityID;
            _nextBotID = memento.NextBotID;
            Journey.Restore(memento.Journey, load);
            Exploration.Restore(memento.Exploration, load);
            Progress.Restore(memento.Progress);
            Research.Restore(memento.Research, load);
            Upgrades.Restore(memento.Upgrades, load);
            Energy.Restore(memento.Energy);
            Barrier.Restore(memento.Barrier);
            Hull.Restore(memento.Hull, load);
            foreach (FacilityMemento facility in memento.Facilities)
            {
                Facilities.Add(Facility.Restore(facility, Configuration, load));
            }

            Floor.Restore(memento.Floor, load);
            foreach (BotMemento bot in memento.Bots)
            {
                _bots.Add(Bot.Restore(bot, this, load));
            }
        }

        public Bot BotOperating(Facility f)
        {
            return _bots.Find(b => !b.IsMoving && b.Action.Holds(Place.At(f), PlaceUse.Post));
        }

        public Bot StationedAt(Facility f)
        {
            return _bots.Find(b => b.Station == f);
        }

        public Bot BotCharging(Facility charger)
        {
            return _bots.Find(b => !b.IsMoving && b.Action.Holds(Place.At(charger), PlaceUse.Charge));
        }

        public bool IsOccupied(Place post)
        {
            return _bots.Any(other => other.Action.Holds(post, PlaceUse.Post));
        }

        public int CountCharging(Facility charger)
        {
            return _bots.Count(other => other.Action.Holds(Place.At(charger), PlaceUse.Charge));
        }

        public bool IsTaking(Place source)
        {
            return Takes(source).Any();
        }

        public int CountTaking(Place source, MaterialDefinition type)
        {
            return Takes(source).Where(claim => claim.Type == type).Sum(claim => claim.Count);
        }

        public int CountInFlight(Place destination, MaterialDefinition type, int? task, Bot except)
        {
            return _bots.Where(other => other != except).Sum(other => other.Action.Bringing(destination, type, task));
        }

        private IEnumerable<MaterialClaim> Takes(Place source)
        {
            return _bots.SelectMany(bot => bot.Action.MaterialClaims).Where(claim => claim.Flow == MaterialFlow.Take && claim.Place == source);
        }

        internal Bot AddBot(GridPoint spot)
        {
            int id = _nextBotID++;
            var bot = new Bot(id, spot) { BatterySeconds = Configuration.BatterySeconds };
            _bots.Add(bot);
            return bot;
        }

        internal void Emit(GameEvent gameEvent)
        {
            Events.Emit(gameEvent);
        }
    }
}
