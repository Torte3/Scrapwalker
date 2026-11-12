using Scrapwalker.Game.Definitions.Flights;

namespace Scrapwalker.Game.Definitions
{
    public interface IShotFlightView<out T>
    {
        T Tracer(TracerFlight tracer);

        T Bolt(BoltFlight bolt);

        T Shell(ShellFlight shell);

        T Lob(LobFlight lob);
    }
}
