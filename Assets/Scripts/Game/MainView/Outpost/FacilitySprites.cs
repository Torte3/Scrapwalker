using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.MainView.Compose;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class FacilitySprites : WorldPainter
    {
        private readonly ArtParts _parts;
        private readonly ArtPlayer _player;
        private readonly FacilityArtState _state = new FacilityArtState();

        public FacilitySprites(WorldScene scene, ArtParts parts, ArtPlayer player)
            : base(scene)
        {
            _parts = parts;
            _player = player;
        }

        public void DrawDamage(in FacilityVisual facility, Color tint)
        {
            ArtLayer damage = facility.Look.Damage;
            if (damage == null)
            {
                return;
            }

            Canvas.Save();
            Canvas.Translate(facility.Rect.Left, facility.Rect.Top);
            _parts.Put(damage, DrawBand.Facilities, tint);
            Canvas.Restore();
        }

        public void DrawBuildStripes(in FacilityRect rect, Color tint)
        {
            DrawStripes(_parts.Art.Outpost.BuildStripes, rect, tint);
        }

        public void DrawDismantleStripes(in FacilityRect rect, Color tint)
        {
            DrawStripes(_parts.Art.Outpost.DismantleStripes, rect, tint);
        }

        private void DrawStripes(ArtLayer tile, in FacilityRect rect, Color tint)
        {
            foreach (Vector2Int cell in rect.Cells)
            {
                Canvas.Save();
                Canvas.Translate(CellLeft(cell.x), FloorY(cell.y + 1));
                _parts.Put(tile, DrawBand.Facilities, tint);
                Canvas.Restore();
            }
        }

        public void DrawStopped(in FacilityVisual facility, float recoilFrames, float wake)
        {
            Draw(facility, recoilFrames, wake, false, null);
        }

        public void DrawRunning(in FacilityVisual facility, float recoilFrames, float wake, StationBot? bot)
        {
            Draw(facility, recoilFrames, wake, facility.IsWorking, bot);
        }

        private void Draw(in FacilityVisual facility, float recoilFrames, float wake, bool isWorking, StationBot? bot)
        {
            FacilityLook look = facility.Look;
            Canvas.Save();
            Canvas.Translate(facility.Rect.Left, facility.Rect.Top);
            if (!TryDrawWholePicture(look, recoilFrames, wake))
            {
                _state.Begin(facility, isWorking, bot, Scene.ElapsedFrames);
                _player.Draw(look.Layers, _state, DrawBand.Facilities);
            }

            Canvas.Restore();
        }

        private bool TryDrawWholePicture(FacilityLook look, float recoilFrames, float wake)
        {
            ArtEntry fireArt = look.Weapon?.Fire;
            if (recoilFrames > 0f && fireArt != null)
            {
                ArtLayer fire = fireArt.FirstLayer;
                _parts.Put(fire, fire.At(fire.Length - recoilFrames), DrawBand.Facilities, Color.white);
                return true;
            }

            if (look.Wake != null && wake < 1f)
            {
                ArtLayer waking = look.Wake.FirstLayer;
                _parts.Put(waking, waking.At(wake * waking.Length), DrawBand.Facilities, Color.white);
                return true;
            }

            return false;
        }
    }
}
