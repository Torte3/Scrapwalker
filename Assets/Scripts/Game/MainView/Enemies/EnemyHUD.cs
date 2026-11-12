using Scrapwalker.Core.Combat;
using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemyHUD
    {
        private const float HPBarHeight = 4f;
        private const float HPBarWidthShare = 0.7f;
        private const float HPBarMinWidth = 28f;
        private const float HPBarLift = 10f;
        private const float LockOnPadding = 9f;
        private const float LockOnCornerShare = 0.32f;
        private const float LockOnLine = 3.4f;
        private const float LockOnEdgeLine = 7f;
        private const float LockOnGrow = 0.45f;

        private readonly WorldUI _ui;

        public EnemyHUD(WorldUI ui)
        {
            _ui = ui;
        }

        public DamageNumbers Numbers { get; } = new DamageNumbers();

        public void BeginBars()
        {
            _ui.Layer(DrawBand.Enemies);
        }

        public void DrawHPBar(EnemyFigure figure, Vector2 top, float bodyWidth)
        {
            if (figure.MaxHP <= 0f || (figure.HP >= figure.MaxHP && figure.HealedShown <= 0f))
            {
                return;
            }

            float w = Mathf.Max(HPBarMinWidth, bodyWidth * HPBarWidthShare);
            float x = top.x - w * 0.5f;
            float y = top.y - HPBarLift - HPBarHeight;
            var bar = new WorldBar(_ui, new Rect(x, y, w, HPBarHeight));
            float hp = figure.HP / figure.MaxHP;
            bar.Back(Palette.HPBarBack, 1f);
            bar.Fill(hp, Palette.HPBarFill);
            float healed = figure.HealedShown;
            if (healed > 0f)
            {
                float from = (figure.HP - figure.HealedHP) / figure.MaxHP;
                bar.Fill(from, hp, Palette.HealNumber.WithAlpha(healed));
                new WorldBar(_ui, new Rect(x, y - 1f, w, 1f)).Fill(from, hp, Palette.HealCore.WithAlpha(healed * 0.6f));
            }

            if (figure.HasArmor)
            {
                bar.Edge(Palette.Steel, 1f, 1f);
            }
        }

        public void DrawLockOnMark(Rect body, float shown)
        {
            float t = Mathf.Clamp01(shown);
            float scale = 1f + LockOnGrow * Mathf.Pow(1f - t, 3f);
            float alpha = Mathf.Clamp01(t * 2f);
            Vector2 center = body.center;
            float w = (body.width + LockOnPadding * 2f) * scale;
            float h = (body.height + LockOnPadding * 2f) * scale;
            var box = new Rect(center.x - w * 0.5f, center.y - h * 0.5f, w, h);
            float corner = Mathf.Min(w, h) * LockOnCornerShare;
            DrawCorners(box, corner, LockOnEdgeLine, Palette.LockOnEdge.WithAlpha(Palette.LockOnEdge.a * alpha));
            DrawCorners(box, corner, LockOnLine, Palette.Amber.WithAlpha(alpha));
        }

        private void DrawCorners(Rect box, float length, float width, Color color)
        {
            float half = width * 0.5f;
            for (int i = 0; i < 4; i++)
            {
                float x = i % 2 == 0 ? box.xMin : box.xMax;
                float y = i < 2 ? box.yMin : box.yMax;
                float dx = i % 2 == 0 ? 1f : -1f;
                float dy = i < 2 ? 1f : -1f;
                _ui.HorizontalLine(x - dx * half, x + dx * length, y, width, color);
                _ui.VerticalLine(x, y, y + dy * length, width, color);
            }
        }

        public void DrawNumbers()
        {
            Numbers.Draw(_ui);
        }
    }
}
