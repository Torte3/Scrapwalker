using System.Collections.Generic;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView.Bots;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class FacilityPainter : WorldPainter
    {
        private const float DismantleShadeGain = 0.6f;
        private static readonly SparkBurst RepairSparks = new SparkBurst(4, Vector2.zero, new Vector2(-1.5f, 1.5f), new Vector2(0f, -2f), 18f, 0f, Palette.Spark, 2f, 0f, 0.15f);
        private static readonly SparkBurst JobSparks = new SparkBurst(1, Vector2.zero, new Vector2(-1f, 1f), new Vector2(0f, -2f), 14f, 0f, Palette.Spark, 2f, 0f, 0.15f);
        private static readonly PuffBurst DismantleSteam = new PuffBurst(1, Vector2.zero, new Vector2(-0.3f, 0.3f), new Vector2(-0.2f, -0.6f), 34f, 2f, 2f, PuffKind.Steam);
        private static readonly SparkBurst DismantleDebris = new SparkBurst(1, Vector2.zero, new Vector2(-0.4f, 0.4f), Vector2.zero, 22f, 0f, Palette.Debris, 3f, 0f, 0.22f);

        private readonly ArtParts _parts;
        private readonly FacilitySprites _sprites;
        private readonly BotSprites _bots;
        private readonly EffectsPainter _effects;
        private readonly Dictionary<int, float> _recoil = new Dictionary<int, float>();
        private readonly HashSet<int> _justFired = new HashSet<int>();
        private readonly List<int> _recoilFacilities = new List<int>();
        private float _wake;

        public FacilityPainter(WorldScene scene, ArtParts parts, FacilitySprites sprites, BotSprites bots, EffectsPainter effects)
            : base(scene)
        {
            _parts = parts;
            _sprites = sprites;
            _bots = bots;
            _effects = effects;
        }

        public void Clear()
        {
            _recoil.Clear();
            _justFired.Clear();
        }

        public void Recoil(int facilityID, float frames)
        {
            _recoil[facilityID] = frames;
            _justFired.Add(facilityID);
        }

        public void Step(float deltaFrames)
        {
            _recoilFacilities.Clear();
            _recoilFacilities.AddRange(_recoil.Keys);
            foreach (int id in _recoilFacilities)
            {
                float left = _recoil[id];
                if (!_justFired.Contains(id) && left > 0f)
                {
                    _recoil[id] = left - deltaFrames;
                }
            }

            _justFired.Clear();
        }

        public void Draw(FrameVisual frame)
        {
            _wake = frame.Wake;
            DrawEmptyCells(frame);
            foreach (FacilityVisual facility in frame.Facilities)
            {
                _recoil.TryGetValue(facility.Facility.ID, out float recoil);
                facility.Job.Draw(this, facility, recoil);
            }

            DrawRivets();
        }

        private void DrawEmptyCells(FrameVisual frame)
        {
            for (int r = 0; r < Scene.Rows; r++)
            {
                for (int c = 0; c < Scene.Columns; c++)
                {
                    if (!frame.IsOccupied(c, r))
                    {
                        EmptyCell(c, r);
                    }
                }
            }

            foreach (FacilityVisual facility in frame.Facilities)
            {
                if (facility.Job.HasPicture)
                {
                    continue;
                }

                foreach (Vector2Int cell in facility.Rect.Cells)
                {
                    EmptyCell(cell.x, cell.y);
                }
            }
        }

        private void EmptyCell(int c, int r)
        {
            Canvas.Save();
            Canvas.Translate(CellLeft(c), FloorY(r + 1));
            _parts.Put(_parts.Art.Outpost.EmptyCell, DrawBand.Facilities);
            Canvas.Restore();
        }

        internal void DrawStopped(in FacilityVisual facility, float recoil)
        {
            _sprites.DrawStopped(facility, recoil, _wake);
        }

        internal void DrawDamage(in FacilityVisual facility)
        {
            _sprites.DrawDamage(facility, Palette.OverPicture);
        }

        internal void DrawBuildStripes(in FacilityVisual facility, Color tint)
        {
            _sprites.DrawBuildStripes(facility.Rect, tint);
        }

        internal void DrawDismantleShade(in FacilityVisual facility)
        {
            Rect inner = facility.Rect.Inner;
            Color shade = Palette.DismantleShade.WithAlpha(Palette.DismantleShade.a + facility.Job.ProgressOf(facility.Facility) * DismantleShadeGain);
            _parts.FillRect(inner.x, inner.y, inner.width, inner.height, shade, DrawBand.Facilities);
            _sprites.DrawDismantleStripes(facility.Rect, Color.white);
        }

        internal void DrawWorking(in FacilityVisual facility, float recoil)
        {
            if (facility.HasChargingBot)
            {
                _sprites.DrawRunning(facility, recoil, _wake, new StationBot(BotBody.Normal));
                return;
            }

            if (!facility.Worker.HasValue)
            {
                _sprites.DrawRunning(facility, recoil, _wake, null);
                return;
            }

            FacilityWorkerVisual worker = facility.Worker.Value;
            var body = new BotBody(worker.Bot);
            _sprites.DrawRunning(facility, recoil, _wake, new StationBot(body));
            if (body.IsLowPower)
            {
                _bots.DrawLowPowerEyes(worker.Stand.x, worker.Stand.y, facility.Look.Station.Dx < 0f ? 1 : -1, DrawBand.Facilities);
            }
        }

        private void DrawRivets()
        {
            for (int c = 0; c <= Scene.Columns; c++)
            {
                for (int r = 0; r <= Scene.Rows; r++)
                {
                    Canvas.Save();
                    Canvas.Translate(CellLeft(c), FloorY(r));
                    _parts.Put(_parts.Art.Outpost.Rivet, DrawBand.Facilities);
                    Canvas.Restore();
                }
            }
        }

        public void EmitParticles(List<FacilityVisual> facilities)
        {
            foreach (FacilityVisual facility in facilities)
            {
                facility.Job.Emit(this, facility);
            }
        }

        internal void EmitRepairSparks(in FacilityVisual facility)
        {
            if (Scene.IsEvery(9))
            {
                float x = facility.Rect.Left + 30f + Scene.Random() * 40f;
                float y = facility.Rect.Top + 26f;
                _effects.Emit(RepairSparks, new Vector2(x, y));
            }
        }

        internal void EmitJobSparks(in FacilityVisual facility)
        {
            Rect inner = facility.Rect.Inner;
            if (Scene.IsEvery(6))
            {
                _effects.Emit(JobSparks.Spreading(new Vector2(inner.width - 20f, 0f)), new Vector2(inner.center.x, inner.yMax - 14f));
            }
        }

        internal void EmitDismantleDust(in FacilityVisual facility)
        {
            Rect inner = facility.Rect.Inner;
            if (Scene.IsEvery(3))
            {
                _effects.Emit(DismantleSteam.Spreading(new Vector2(inner.width - 12f, -inner.height * 0.5f)), new Vector2(inner.center.x, inner.yMax - 8f - inner.height * 0.25f));
            }

            if (Scene.IsEvery(7))
            {
                _effects.Emit(DismantleDebris.Spreading(new Vector2(inner.width - 16f, 10f)), new Vector2(inner.center.x, inner.y + 13f));
            }
        }
    }
}
