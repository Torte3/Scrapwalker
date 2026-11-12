using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    public sealed class Ledger
    {
        private readonly World _world;

        public Ledger(World world)
        {
            _world = world;
        }

        public int HeldMaterial(MaterialDefinition type)
        {
            return HeldMaterial(type, QualityFilter.Any);
        }

        public int HeldMaterial(MaterialDefinition type, QualityFilter filter)
        {
            return Sources(type).Where(source => filter.Accepts(source.Items.Quality)).Sum(source => source.Items.Count);
        }

        public int SupplyInRange(MaterialDefinition type, QualityFilter filter)
        {
            return Sources(type).Where(source => source.IsAtRest && filter.Accepts(source.Items.Quality)).Sum(source => source.Items.Count);
        }

        internal int Available(MaterialDefinition type, QualityFilter filter)
        {
            return Sources(type).Where(source => filter.Accepts(source.Items.Quality)).Sum(source => System.Math.Max(0, source.Free(_world)));
        }

        public bool CanAfford(FacilityDefinition definition, QualityFilter filter)
        {
            return Shortage(definition, filter).Count == 0;
        }

        public List<MaterialAmount> Shortage(FacilityDefinition definition, QualityFilter filter)
        {
            return Shortage(definition.Cost, filter);
        }

        public List<MaterialAmount> Shortage(IReadOnlyList<MaterialAmount> needs, QualityFilter filter, System.Func<MaterialDefinition, int> delivered = null, int times = 1)
        {
            var lacking = new List<MaterialAmount>();
            foreach (MaterialAmount need in MaterialAmount.PerMaterial(needs))
            {
                int left = need.Count * times - (delivered?.Invoke(need.Type) ?? 0) - HeldMaterial(need.Type, filter);
                if (left > 0)
                {
                    lacking.Add(new MaterialAmount(need.Type, left));
                }
            }

            return lacking;
        }

        public Quality? PreviewQuality(FacilityDefinition definition, QualityFilter filter)
        {
            var picked = new StackList();
            foreach (MaterialAmount cost in definition.Cost)
            {
                int need = cost.Count;
                foreach (Quality quality in filter.InOrder)
                {
                    int take = System.Math.Min(need, HeldMaterial(cost.Type, QualityFilter.Exactly(quality)));
                    picked.Add(ItemStack.Of(cost.Type, quality, take));
                    need -= take;
                }

                if (need > 0)
                {
                    return null;
                }
            }

            return picked.AverageQuality;
        }

        internal AssignmentAction HaulFor(WorkPriority priority, IReadOnlyList<Lack> lacks, int requested, Place destination, GridPoint from)
        {
            Lack lack = lacks[requested];
            MaterialSource source = Choose(lack.Type, lack.Filter, from);
            return source?.HaulFor(_world, priority, lack, Lack.PerTrip(lacks, requested, source.Items.Quality), destination);
        }

        internal bool TryTakeOne(MaterialDefinition type, QualityFilter filter, GridPoint from, out Quality quality)
        {
            MaterialSource source = Choose(type, filter, from);
            quality = source?.Items.Quality ?? default;
            return source != null && source.TakeOne(_world);
        }

        private MaterialSource Choose(MaterialDefinition type, QualityFilter filter, GridPoint from)
        {
            foreach (Quality quality in filter.InOrder)
            {
                MaterialSource best = null;
                float bestCost = float.MaxValue;
                foreach (MaterialSource source in Sources(type))
                {
                    if (source.Items.Quality != quality || source.Free(_world) <= 0)
                    {
                        continue;
                    }

                    float cost = _world.Paths.PathCost(from, source.Spot);
                    if (cost < bestCost)
                    {
                        best = source;
                        bestCost = cost;
                    }
                }

                if (best != null)
                {
                    return best;
                }
            }

            return null;
        }

        private IEnumerable<MaterialSource> Sources(MaterialDefinition type)
        {
            foreach (Facility store in _world.Facilities)
            {
                if (!store.IsStorageAccessible)
                {
                    continue;
                }

                foreach (ItemStack stack in store.Inventory.Stacks)
                {
                    if (stack.Type == type)
                    {
                        yield return new StoredSource(store, stack);
                    }
                }
            }

            foreach (Pile pile in _world.Floor.Piles)
            {
                if (pile.Holds(type))
                {
                    yield return new PileSource(pile);
                }
            }

            foreach (Bot bot in _world.Bots)
            {
                if (bot.IsCarrying && bot.Cargo.Type == type)
                {
                    yield return new CargoSource(bot);
                }
            }
        }

        private abstract class MaterialSource
        {
            protected MaterialSource(ItemStack items)
            {
                Items = items;
            }

            public ItemStack Items { get; }

            public virtual bool IsAtRest => true;

            public virtual GridPoint Spot => default;

            public abstract int Free(World world);

            public virtual AssignmentAction HaulFor(World world, WorkPriority priority, Lack lack, int perTrip, Place destination)
            {
                return null;
            }

            public virtual bool TakeOne(World world)
            {
                return false;
            }
        }

        private sealed class StoredSource : MaterialSource
        {
            private readonly Facility _store;

            public StoredSource(Facility store, ItemStack items)
                : base(items)
            {
                _store = store;
            }

            public override GridPoint Spot => _store.Door;

            public override int Free(World world)
            {
                return Items.Count - world.CountTaking(Place.InStore(_store), Items.Type);
            }

            public override AssignmentAction HaulFor(World world, WorkPriority priority, Lack lack, int perTrip, Place destination)
            {
                return new PickupAction(priority, _store.StoreRoom, destination, lack.Type, lack.Task, System.Math.Min(perTrip, Free(world)));
            }

            public override bool TakeOne(World world)
            {
                return _store.Inventory.TryTakeMaterial(Items.Type, Items.Quality, 1);
            }
        }

        private sealed class PileSource : MaterialSource
        {
            private readonly Pile _pile;

            public PileSource(Pile pile)
                : base(pile.Items)
            {
                _pile = pile;
            }

            public override GridPoint Spot => _pile.Spot;

            public override int Free(World world)
            {
                return world.IsTaking(Place.AtPile(_pile)) ? 0 : Items.Count;
            }

            public override AssignmentAction HaulFor(World world, WorkPriority priority, Lack lack, int perTrip, Place destination)
            {
                return new PickupAction(priority, _pile, destination, _pile.Type, lack.Task, System.Math.Min(perTrip, _pile.Count));
            }

            public override bool TakeOne(World world)
            {
                return world.Floor.TryTake(_pile, 1, out _);
            }
        }

        private sealed class CargoSource : MaterialSource
        {
            public CargoSource(Bot bot)
                : base(bot.Cargo)
            {
            }

            public override bool IsAtRest => false;

            public override int Free(World world)
            {
                return 0;
            }
        }
    }
}
