using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Outpost.Facilities.Requirements;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using UnityEngine;

namespace Scrapwalker.Core.Outpost.Facilities
{
    [Serializable]
    public sealed class RecipeDefinition
    {
        public string ID = string.Empty;
        public List<MaterialAmount> Inputs = new List<MaterialAmount>();
        [SerializeReference] public InputSource Source = new DeliveredInputs();
        [SerializeReference] public RecipeResult Result;
        public float Seconds = 1f;
        public float Energy;
        [SerializeReference] public List<Requirement> Requires = new List<Requirement>();
        public bool IsOneOff;
        public TaskWindow When;
        public bool IsUnmanned;

        public IReadOnlyList<MaterialAmount> DeliveredInputs => Source.Delivered(Inputs);

        public WorkPriority Priority => Result.Priority;

        public bool Makes(MaterialDefinition material) => Result.Yield(material) > 0;

        public int InputTotal => MaterialAmount.Total(Inputs);

        public bool NeedsStation => !Result.WorksOnRoom && !IsUnmanned;

        public bool CanRunNow(bool inCombat)
        {
            return When != TaskWindow.InCombat || inCombat;
        }

        public bool IsUnlockedBy(ResearchDefinition research)
        {
            return Requires.Exists(requirement => requirement.Research == research);
        }

        public bool IsOpenedBy(RecipeDefinition upgrade)
        {
            return Requires.Exists(requirement => requirement.IsOpenedBy(upgrade));
        }

        public ResearchDefinition PendingResearch(World world)
        {
            foreach (Requirement requirement in Requires)
            {
                ResearchDefinition pending = requirement.PendingResearch(world);
                if (pending != null)
                {
                    return pending;
                }
            }

            return null;
        }

        public RecipeDefinition PreviousIn(FacilityDefinition facility)
        {
            foreach (Requirement requirement in Requires)
            {
                RecipeDefinition previous = requirement.PreviousIn(facility);
                if (previous != null)
                {
                    return previous;
                }
            }

            return null;
        }

        public RecipeOffer Offer(World world, Facility facility, int times = 1)
        {
            foreach (Requirement requirement in Requires)
            {
                if (requirement.HidesUntilMet && !requirement.IsMet(world, facility))
                {
                    return RecipeOffer.Hidden(requirement.Refusal);
                }
            }

            if (Result.IsSpent(world, facility, this))
            {
                return RecipeOffer.Hidden(CommandResult.Fail(CommandError.AlreadyDone));
            }

            CommandResult block = Result.CheckOffer(world, facility, this, times);
            if (!block.IsOK)
            {
                return RecipeOffer.Listed(block);
            }

            foreach (Requirement requirement in Requires)
            {
                if (!requirement.HidesUntilMet && !requirement.IsMet(world, facility))
                {
                    return RecipeOffer.Listed(requirement.Refusal);
                }
            }

            return RecipeOffer.Listed(block);
        }
    }
}
