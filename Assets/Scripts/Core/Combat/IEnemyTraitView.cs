namespace Scrapwalker.Core.Combat
{
    public interface IEnemyTraitView
    {
        void Healing(float reach);

        void Barrier(int layers, int maxLayers, int brokenCount);
    }
}
