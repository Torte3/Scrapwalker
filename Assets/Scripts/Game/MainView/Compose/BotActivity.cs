using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.MainView.Bots;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Compose
{
    public abstract class BotActivity
    {
        public static readonly BotActivity None = new RoamingActivity(false);
        public static readonly BotActivity ChargingAway = new RoamingActivity(true);
        public static readonly BotActivity Welding = new WeldingActivity(true, false);
        public static readonly BotActivity Repairing = new WeldingActivity(false, true);

        private BotActivity()
        {
        }

        public virtual bool SoundsWelding => false;
        public virtual bool SoundsRepairing => false;
        internal virtual bool IsCharging => false;
        internal virtual bool TurnsWithMoves => true;

        public static BotActivity InPicture(Vector2 stand)
        {
            return new InPictureActivity(stand);
        }

        public static BotActivity Charging(FacilityRect charger)
        {
            return new ChargingActivity(charger);
        }

        public static BotActivity Stoking(FacilityRect facility, FacilityLook look, float progress)
        {
            return new StokingActivity(facility, look, progress);
        }

        internal virtual Vector2 StandOf(Bot bot)
        {
            return new Vector2(CellLeft(bot.Position.X), FloorY(bot.Position.Y) - BotFootLift);
        }

        internal abstract void Draw(BotPainter painter, Bot bot, Vector2 at, int direction);

        private sealed class RoamingActivity : BotActivity
        {
            private readonly bool _isCharging;

            public RoamingActivity(bool isCharging)
            {
                _isCharging = isCharging;
            }

            internal override bool IsCharging => _isCharging;

            internal override void Draw(BotPainter painter, Bot bot, Vector2 at, int direction)
            {
                painter.DrawMoving(bot, at.x, at.y, direction);
            }
        }

        private sealed class WeldingActivity : BotActivity
        {
            private readonly bool _soundsWelding;
            private readonly bool _soundsRepairing;

            public WeldingActivity(bool soundsWelding, bool soundsRepairing)
            {
                _soundsWelding = soundsWelding;
                _soundsRepairing = soundsRepairing;
            }

            public override bool SoundsWelding => _soundsWelding;
            public override bool SoundsRepairing => _soundsRepairing;

            internal override void Draw(BotPainter painter, Bot bot, Vector2 at, int direction)
            {
                painter.DrawWelding(bot, at, direction);
            }
        }

        private sealed class InPictureActivity : BotActivity
        {
            private readonly Vector2 _stand;

            public InPictureActivity(Vector2 stand)
            {
                _stand = stand;
            }

            internal override Vector2 StandOf(Bot bot)
            {
                return _stand;
            }

            internal override void Draw(BotPainter painter, Bot bot, Vector2 at, int direction)
            {
            }
        }

        private sealed class ChargingActivity : BotActivity
        {
            private const float ChargerTop = 5f;
            private const float ChargerStandX = 4f;

            private readonly FacilityRect _charger;

            public ChargingActivity(FacilityRect charger)
            {
                _charger = charger;
            }

            internal override bool IsCharging => true;

            internal override Vector2 StandOf(Bot bot)
            {
                return new Vector2(_charger.CenterX - ChargerStandX, _charger.Floor - BotFootLift - ChargerTop);
            }

            internal override void Draw(BotPainter painter, Bot bot, Vector2 at, int direction)
            {
                painter.DrawCharging(bot, at);
            }
        }

        private sealed class StokingActivity : BotActivity
        {
            private const float StokerStandX = 4f;

            private readonly FacilityRect _facility;
            private readonly FacilityLook _look;
            private readonly float _progress;

            public StokingActivity(FacilityRect facility, FacilityLook look, float progress)
            {
                _facility = facility;
                _look = look;
                _progress = progress;
            }

            internal override bool TurnsWithMoves => false;

            internal override Vector2 StandOf(Bot bot)
            {
                return new Vector2(_facility.Left + StokerStandX, _facility.Top + CellSize - BotFootLift);
            }

            internal override void Draw(BotPainter painter, Bot bot, Vector2 at, int direction)
            {
                painter.DrawStoking(bot, _facility, _look, _progress, at);
            }
        }
    }
}
