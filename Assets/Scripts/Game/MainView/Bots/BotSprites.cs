using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Sets;
using Scrapwalker.Game.MainView.Compose;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Bots
{
    public sealed class BotSprites : WorldPainter
    {
        private const float StepAngle = Mathf.PI / 4f;
        private const int WalkSteps = 8;
        private const int ClimbSteps = 4;

        private readonly ArtParts _parts;
        private readonly ArtPlayer _player;
        private readonly BotArtState _state = new BotArtState();
        private ArtEntry _pose;
        private Vector2 _at;
        private int _direction;

        public BotSprites(WorldScene scene, ArtParts parts, ArtPlayer player)
            : base(scene)
        {
            _parts = parts;
            _player = player;
        }

        public void DrawWalking(float x, float y, int direction, float phase, BotBody body)
        {
            DrawPose(_parts.Art.Bots.Walk, body, x, y, direction, StepOf(phase, WalkSteps));
        }

        public void DrawClimbing(float x, float y, int direction, float phase, BotBody body)
        {
            DrawPose(_parts.Art.Bots.Climb, body, x, y, direction, StepOf(phase, ClimbSteps));
        }

        public void DrawWelding(float x, float y, int direction, BotBody body)
        {
            DrawPose(_parts.Art.Bots.Weld, body, x, y, direction, null);
        }

        public void DrawLights()
        {
            InPose(true);
        }

        private static int StepOf(float phase, int steps)
        {
            return (int)Mathf.Repeat(Mathf.RoundToInt(phase / StepAngle), steps);
        }

        private void DrawPose(BotPoseArt pose, BotBody body, float x, float y, int direction, int? step)
        {
            _pose = body.In(pose);
            _at = new Vector2(x, y);
            _direction = direction;
            _state.Begin(Scene.ElapsedFrames, step);
            InPose(false);
        }

        private void InPose(bool isLight)
        {
            _state.ShowsLights = isLight;
            Canvas.Save();
            Canvas.Translate(_at.x, _at.y);
            Canvas.Scale(_direction, 1f);
            _player.Draw(_pose, _state, DrawBand.Bots);
            Canvas.Restore();
        }

        public void DrawStoking(in FacilityRect facility, FacilityLook look, float progress, BotBody body)
        {
            ArtLayer layer = look.BotLayer;
            if (layer == null)
            {
                return;
            }

            Canvas.Save();
            Canvas.Translate(facility.Left, facility.Top);
            _parts.Put(layer, layer.At(progress * layer.Period), DrawBand.Bots, body.Tint);
            Canvas.Restore();
        }

        public void DrawLowPowerEyes(float x, float y, int direction, DrawBand band)
        {
            ArtLayer layer = _parts.Art.Bots.LowPowerEyes;
            Canvas.Save();
            Canvas.Translate(x, y);
            Canvas.Scale(direction, 1f);
            _parts.Put(layer, layer.ByValue(0f), band, new Color(1f, 1f, 1f, layer.Alpha(Scene.ElapsedFrames)));
            Canvas.Restore();
        }
    }
}
