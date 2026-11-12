using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView.Compose;
using TMPro;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class BuildOverlayPainter : WorldPainter
    {
        private const float NotePadding = 6f;
        private const float OutsideMargin = 400f;

        private readonly WorldUI _ui;
        private readonly GameData _data;

        public BuildOverlayPainter(WorldScene scene, WorldUI ui, GameData data)
            : base(scene)
        {
            _ui = ui;
            _data = data;
        }

        public void Draw(FrameVisual frame)
        {
            OverlayVisual marks = frame.Overlay;
            if (!marks.Ghost.HasValue && !marks.Selected.HasValue && !marks.Marked.HasValue)
            {
                return;
            }

            _ui.Layer(DrawBand.BuildOverlay);
            Canvas.Save();
            if (marks.Ghost.HasValue)
            {
                DarkenOutside();
            }

            Canvas.Translate(Scene.BaseX, Scene.BaseY);
            if (marks.Ghost.HasValue)
            {
                DrawGhost(frame, marks.Ghost.Value);
            }
            else if (marks.Selected.HasValue)
            {
                MarkFacility(marks.Selected.Value);
            }

            if (marks.Marked.HasValue)
            {
                MarkFacility(marks.Marked.Value);
            }

            Canvas.Restore();
        }

        private void DarkenOutside()
        {
            float left = Scene.WallL + Scene.BaseX;
            float right = Scene.WallR + Scene.BaseX;
            float top = Scene.HullTop - Armor + Scene.BaseY;
            float bottom = HullFloorY + 10f + Scene.BaseY;
            _ui.FillRect(CoverLeft, -OutsideMargin, CoverWidth, top + OutsideMargin, Palette.BuildOutside);
            _ui.FillRect(CoverLeft, bottom, CoverWidth, ScreenHeight - bottom + OutsideMargin, Palette.BuildOutside);
            _ui.FillRect(CoverLeft, top, left - CoverLeft, bottom - top, Palette.BuildOutside);
            _ui.FillRect(right, top, CoverLeft + CoverWidth - right, bottom - top, Palette.BuildOutside);
        }

        private void DrawGhost(FrameVisual frame, in BuildGhost ghost)
        {
            for (int c = 0; c <= Scene.Columns; c++)
            {
                _ui.VerticalLine(CellLeft(c) + 0.5f, Scene.HullTop, HullFloorY, 1f, Palette.BuildGrid);
            }

            for (int r = 0; r <= Scene.Rows; r++)
            {
                _ui.HorizontalLine(HullLeft, HullLeft + Scene.HullWidth, FloorY(r) + 0.5f, 1f, Palette.BuildGrid);
            }

            for (int r = 0; r < Scene.Rows; r++)
            {
                for (int c = 0; c < Scene.Columns; c++)
                {
                    if (!frame.IsOccupied(c, r))
                    {
                        Rect free = new FacilityRect(c, r, 1, 1).Inner;
                        _ui.FillRect(free.x, free.y, free.width, free.height, Palette.BuildFreeCell);
                    }
                }
            }

            var rect = new FacilityRect(ghost.Rect);
            Rect inner = rect.Inner;
            Color color = ghost.IsPlaceable ? Palette.OK : Palette.Red;
            _ui.FillRect(inner.x, inner.y, inner.width, inner.height, color.WithAlpha(0.25f));
            _ui.DashedRect(inner.x, inner.y, inner.width, inner.height, 2f, color, 5f, 3f);
            _ui.Text(rect.Left + 6f, rect.Top + 17f, _data.NameOf(ghost.Definition), 12f, Color.white, FontStyles.Bold);
            _ui.FillRect(rect.Left, rect.Top - 24f, _ui.TextWidth(ghost.Note, 12f) + NotePadding * 2f, 20f, Palette.BuildNoteBack);
            _ui.Text(rect.Left + NotePadding, rect.Top - 9f, ghost.Note, 12f, color);
        }

        private void MarkFacility(in FacilityRect facility)
        {
            Rect inner = facility.Inner;
            _ui.StrokeRect(inner.x, inner.y, inner.width, inner.height, 2f, Palette.Cyan);
        }
    }
}
