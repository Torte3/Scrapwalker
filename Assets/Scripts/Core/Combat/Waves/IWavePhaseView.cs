namespace Scrapwalker.Core.Combat.Waves
{
    public interface IWavePhaseView<out T>
    {
        T Idle();

        T Reaching();

        T Fighting();

        T Haul();

        T Resting();
    }
}
