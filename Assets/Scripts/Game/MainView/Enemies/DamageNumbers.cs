using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Wording;
using TMPro;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class DamageNumbers
    {
        private const float LifeFrames = 48f;
        private const float FadeFrames = 16f;
        private const float JitterPixels = 16f;
        private const float FontPixels = 13f;
        private const float HealRowPixels = 13f;

        private readonly List<Number> _numbers = new List<Number>();
        private readonly List<ShownNumber> _shown = new List<ShownNumber>();
        private readonly System.Random _jitter = new System.Random(17);

        private sealed class Number
        {
            public int EnemyID;
            public Color Color;
            public string Text;
            public float Frames;
            public Vector2 At;
            public Vector2 Offset;
        }

        public readonly struct ShownNumber
        {
            public ShownNumber(string text, Vector2 at, Color color)
            {
                Text = text;
                At = at;
                Color = color;
            }

            public string Text { get; }
            public Vector2 At { get; }
            public Color Color { get; }
        }

        public void Clear()
        {
            _numbers.Clear();
        }

        public void Add(in EnemyImpact impact, Vector2 at)
        {
            Color color = impact.IsBlocked ? Palette.BlockedNumber : impact.Absorbed > 0f ? Palette.Steel : Palette.DamageNumber;
            float x = ((float)_jitter.NextDouble() * 2f - 1f) * JitterPixels;
            _numbers.Add(new Number { EnemyID = impact.EnemyID, Color = color, Text = Format(impact.Damage), At = at, Offset = new Vector2(x, 0f) });
        }

        public void AddHeal(in EnemyHeal heal, Vector2 at)
        {
            _numbers.Add(new Number { EnemyID = heal.EnemyID, Color = Palette.HealNumber, Text = "+" + Format(heal.Amount), At = at, Offset = new Vector2(0f, -HealRowPixels) });
        }

        public void Follow(int enemyID, Vector2 at)
        {
            foreach (Number number in _numbers)
            {
                if (number.EnemyID == enemyID)
                {
                    number.At = at;
                }
            }
        }

        public void Step(float deltaFrames)
        {
            foreach (Number number in _numbers)
            {
                number.Frames += deltaFrames;
            }

            _numbers.RemoveAll(number => number.Frames >= LifeFrames);
        }

        public IReadOnlyList<ShownNumber> Shown()
        {
            _shown.Clear();
            foreach (Number number in _numbers)
            {
                float alpha = Mathf.Clamp01((LifeFrames - number.Frames) / FadeFrames);
                _shown.Add(new ShownNumber(number.Text, number.At + number.Offset, number.Color.WithAlpha(alpha)));
            }

            return _shown;
        }

        public void Draw(WorldUI ui)
        {
            foreach (ShownNumber shown in Shown())
            {
                ui.Text(shown.At.x, shown.At.y, shown.Text, FontPixels, shown.Color, FontStyles.Bold, TextAlignmentOptions.Bottom);
            }
        }

        private static string Format(float value)
        {
            float shown = value >= 10f ? Mathf.Round(value) : Mathf.Round(value * 10f) / 10f;
            return Texts.Decimal(shown);
        }
    }
}
