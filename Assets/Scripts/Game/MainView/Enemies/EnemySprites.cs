using System.Collections.Generic;
using Scrapwalker.Game.Look.Art;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemySprites
    {
        private readonly ArtPlayer _player;
        private readonly EnemyLooks _looks;
        private readonly EnemyArtState _state;
        private readonly Dictionary<EnemyFigure, float[]> _turns = new Dictionary<EnemyFigure, float[]>();

        public EnemySprites(WorldScene scene, ArtPlayer player, EnemyLooks looks)
        {
            _player = player;
            _looks = looks;
            _state = new EnemyArtState(scene);
        }

        public void Forget(EnemyFigure figure)
        {
            _turns.Remove(figure);
        }

        public void ForgetAll()
        {
            _turns.Clear();
        }

        public void Draw(EnemyFigure figure, in EnemyBodyStyle style, DrawBand band)
        {
            if (Begin(figure, style, true, out ArtEntry body))
            {
                _player.Draw(body, _state.Overlay, _state, band);
            }
        }

        public void DrawAboveFigures(EnemyFigure figure, in EnemyBodyStyle style, DrawBand band)
        {
            if (Begin(figure, style, false, out _) && _state.Overlay != null)
            {
                _player.DrawAboveFigures(_state.Overlay, _state, band);
            }
        }

        private bool Begin(EnemyFigure figure, in EnemyBodyStyle style, bool advances, out ArtEntry body)
        {
            EnemyLook look = _looks.Of(figure.Definition);
            body = style.IsCalm ? look.Calm : look.Alert;
            if (body == null)
            {
                return false;
            }

            _state.Begin(figure, body, look.Attack, _looks.CueOf(figure.Move.Style), TurnsOf(figure, body), style.IsCalm, advances);
            return true;
        }

        private float[] TurnsOf(EnemyFigure figure, ArtEntry body)
        {
            if (!_turns.TryGetValue(figure, out float[] turns) || turns.Length != body.Layers.Length)
            {
                turns = new float[body.Layers.Length];
                _turns[figure] = turns;
            }

            return turns;
        }
    }
}
