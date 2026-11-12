using System.Collections.Generic;
using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Outpost.Facilities.Recipes.Results;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class RecipeOutcome : IRecipeResultView, IRecipeEffects
    {
        private readonly World _world;
        private readonly GameData _data;
        private readonly Facility _facility;
        private readonly FacilityRecipeRow _row;
        private readonly int _times;
        private readonly List<FacilityEffect> _effects;
        private bool _hasYield;

        public RecipeOutcome(World world, GameData data, Facility facility, FacilityRecipeRow row, int times, List<FacilityEffect> effects)
        {
            _world = world;
            _data = data;
            _facility = facility;
            _row = row;
            _times = times;
            _effects = effects;
        }

        public void Show(RecipeDefinition recipe)
        {
            _hasYield = false;
            recipe.Result.Show(this);
            if (!_hasYield)
            {
                _row.SetYield(_data, null, 0, 0);
            }

            _effects.Clear();
            recipe.Result.Describe(_world, _facility, recipe, this);
            _row.SetEffects(_effects);
        }

        public void Product(MaterialDefinition material, int count)
        {
            _hasYield = true;
            _row.SetYield(_data, material, count * _times, _world.Ledger.HeldMaterial(material));
        }

        public void Generate(EnergyResult energy)
        {
            _hasYield = true;
            _row.SetEnergyYield(_data, energy.Amount(_world.Configuration) * _times, _world.Energy.Stored, _world.Energy.Capacity);
        }

        public void Unlock(RecipeDefinition recipe, ResearchDefinition pendingResearch)
        {
            Add(EffectWords.Unlock(_data.NameOf(recipe)),
                pendingResearch == null ? string.Empty : EffectWords.UnlockAfterResearch(_data.NameOf(pendingResearch)));
        }

        public void Change(FacilityStat stat, float from, float to)
        {
            Add(EffectWords.Change(stat, from, to));
        }

        public void Power(RecipeDefinition shot, float from, float to)
        {
            Add(EffectWords.Power(_data.NameOf(shot), Texts.Decimal(from), Texts.Decimal(to)));
        }

        public void Shot(float damage, int rounds)
        {
            Add(rounds > 1
                ? EffectWords.ShotRounds(Texts.Decimal(damage), rounds)
                : EffectWords.Shot(Texts.Decimal(damage)));
        }

        public void ShotPierce(int maxTargets)
        {
            Add(EffectWords.ShotPierce(maxTargets));
        }

        public void ShotArmorPiercing(float armor)
        {
            Add(EffectWords.ShotArmorPiercing(Texts.Decimal(armor)));
        }

        public void Slow(SlowEffect slow, float radius)
        {
            Add(EffectWords.Slow(Texts.Decimal(radius), Texts.Seconds(slow.Seconds), Mathf.RoundToInt(slow.Factor * 100f)));
        }

        public void Note(EffectNote note)
        {
            Add(EffectWords.Note(note));
        }

        public void BotLimit(int from, int to)
        {
            Add(EffectWords.BotLimit(from, to));
        }

        public void BotMade(int now, int limit)
        {
            Add(EffectWords.Bot, EffectWords.BotCount(now, limit));
        }

        private void Add(string text, string note = "")
        {
            _effects.Add(new FacilityEffect(text, note));
        }
    }
}
