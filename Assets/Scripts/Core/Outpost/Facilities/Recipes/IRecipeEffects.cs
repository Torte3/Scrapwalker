using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Research;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    public interface IRecipeEffects
    {
        void Unlock(RecipeDefinition recipe, ResearchDefinition pendingResearch);

        void Change(FacilityStat stat, float from, float to);

        void Power(RecipeDefinition shot, float from, float to);

        void Shot(float damage, int rounds);

        void ShotPierce(int maxTargets);

        void ShotArmorPiercing(float armor);

        void Slow(SlowEffect slow, float radius);

        void Note(EffectNote note);

        void BotLimit(int from, int to);

        void BotMade(int now, int limit);
    }
}
