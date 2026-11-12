using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using TMPro;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Bots
{
    public sealed class BotPainter : WorldPainter
    {
        private const float NameWidth = 26f;
        private const float NameHeight = 10f;
        private const float NameRadius = 3f;
        private const float NameTop = 38f;
        private const float NameBaseline = 29.5f;
        private const float NameFontPixels = 8f;
        private const float BatteryGap = 3f;
        private const float BatteryTop = 36.5f;
        private const float BatteryCellWidth = 6f;
        private const float BatteryMinFill = 1f;
        private const float HeldY = 23f;
        private const float HeldScale = 0.55f;
        private const float HeldLift = 14f;

        private readonly BotSprites _sprites;
        private readonly ItemIcons _icons;
        private readonly Dictionary<int, BotFacing> _facings = new Dictionary<int, BotFacing>();
        private readonly List<NameTag> _names = new List<NameTag>();
        private readonly WorldUI _ui;

        public BotPainter(WorldScene scene, BotSprites sprites, ItemIcons icons, WorldUI ui)
            : base(scene)
        {
            _sprites = sprites;
            _icons = icons;
            _ui = ui;
        }

        private sealed class BotFacing
        {
            public float LastX;
            public int Direction = 1;
        }

        private readonly struct NameTag
        {
            public NameTag(Vector2 at, Bot bot, bool isCharging, GameConfiguration configuration)
            {
                X = at.x;
                Y = at.y;
                Label = bot.Label();
                BatteryFill = bot.BatteryFill(configuration);
                IsBelowRecharge = bot.NeedsCharge(configuration);
                IsCharging = isCharging;
                Lift = bot.IsCarrying ? HeldLift : 0f;
            }

            public float X { get; }
            public float Y { get; }
            public float BatteryFill { get; }
            public float Lift { get; }
            public string Label { get; }
            public bool IsCharging { get; }
            public bool IsBelowRecharge { get; }
        }

        public void Clear()
        {
            _facings.Clear();
        }

        public void Step(List<BotVisual> bots, GameConfiguration configuration)
        {
            _names.Clear();
            foreach (BotVisual visual in bots)
            {
                Bot bot = visual.Bot;
                float x = bot.Position.X;
                if (!_facings.TryGetValue(bot.ID, out BotFacing facing))
                {
                    facing = new BotFacing { LastX = x };
                    _facings[bot.ID] = facing;
                }

                if (visual.Activity.TurnsWithMoves && Mathf.Abs(x - facing.LastX) > 0.001f)
                {
                    facing.Direction = x > facing.LastX ? 1 : -1;
                }

                facing.LastX = x;
                Vector2 at = visual.Activity.StandOf(bot);
                _names.Add(new NameTag(at, bot, visual.Activity.IsCharging, configuration));
            }
        }

        public void Draw(List<BotVisual> bots)
        {
            foreach (BotVisual visual in bots)
            {
                Bot bot = visual.Bot;
                visual.Activity.Draw(this, bot, visual.Activity.StandOf(bot), _facings[bot.ID].Direction);
            }
        }

        internal void DrawWelding(Bot bot, Vector2 at, int direction)
        {
            _sprites.DrawWelding(at.x, at.y, direction, new BotBody(bot));
            _sprites.DrawLights();
        }

        internal void DrawMoving(Bot bot, float x, float y, int direction)
        {
            var body = new BotBody(bot);
            float phase = 0f;
            if (bot.IsMoving)
            {
                phase = (bot.IsClimbing ? y : x) * 0.35f;
            }

            if (bot.IsClimbing)
            {
                _sprites.DrawClimbing(x, y, direction, phase, body);
            }
            else
            {
                _sprites.DrawWalking(x, y, direction, phase, body);
            }

            if (bot.IsCarrying)
            {
                _icons.Draw(bot.Cargo.Type, x, y - HeldY, HeldScale, DrawBand.Bots);
            }

            _sprites.DrawLights();
        }

        internal void DrawCharging(Bot bot, Vector2 at)
        {
            if (bot.IsLowPower)
            {
                _sprites.DrawLowPowerEyes(at.x, at.y, -1, DrawBand.Bots);
            }
        }

        internal void DrawStoking(Bot bot, in FacilityRect facility, FacilityLook look, float progress, Vector2 at)
        {
            _sprites.DrawStoking(facility, look, progress, new BotBody(bot));
            if (bot.IsLowPower)
            {
                _sprites.DrawLowPowerEyes(at.x, at.y, 1, DrawBand.Bots);
            }
        }

        public void DrawNames()
        {
            _ui.LayerDefault(LabelOrder);
            foreach (NameTag tag in _names)
            {
                float y = tag.Y - tag.Lift;
                _ui.FillRoundRect(tag.X - NameWidth * 0.5f, y - NameTop, NameWidth, NameHeight, NameRadius, Palette.LabelBack);
                _ui.Text(tag.X, y - NameBaseline, tag.Label, NameFontPixels, Palette.Amber, FontStyles.Bold, TextAlignmentOptions.Bottom, LabelOrder + 1);
                BatteryTag(tag.X, y, tag);
            }
        }

        private void BatteryTag(float x, float y, in NameTag tag)
        {
            float bx = x + NameWidth * 0.5f + BatteryGap;
            float by = y - BatteryTop;
            _ui.FillRoundRect(bx - 2f, by - 1.5f, 15f, 10f, 3f, Palette.LabelBack);
            Color frame = BatteryFrameColor(tag);
            var cell = new WorldBar(_ui, new Rect(bx + 2f, by + 2f, BatteryCellWidth, 3f));
            cell.Edge(frame, 1.5f, 1f);
            _ui.FillRect(bx + 10f, by + 2f, 1.5f, 3f, frame);
            if (tag.BatteryFill > 0f)
            {
                cell.Fill(Mathf.Max(BatteryMinFill / BatteryCellWidth, tag.BatteryFill), BatteryFillColor(tag));
            }
        }

        private Color BatteryFrameColor(in NameTag tag)
        {
            bool isEmpty = !tag.IsCharging && tag.BatteryFill <= 0f;
            if (!isEmpty)
            {
                return Palette.BatteryFrame;
            }

            bool isBlinkOn = Scene.ElapsedFrames % 40f < 24f;
            return isBlinkOn ? Palette.Red : Palette.BatteryEmptyDim;
        }

        private static Color BatteryFillColor(in NameTag tag)
        {
            if (tag.IsCharging)
            {
                return Palette.Cyan;
            }

            if (tag.BatteryFill > 0.5f)
            {
                return Palette.OK;
            }

            return tag.IsBelowRecharge ? Palette.Red : Palette.Amber;
        }
    }
}
