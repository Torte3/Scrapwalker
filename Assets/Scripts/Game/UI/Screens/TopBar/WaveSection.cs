using System.Collections.Generic;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.TopBar
{
    internal sealed class WaveSection
    {
        private const float BlinkLowOpacity = 0.45f;
        private const float BlinkPeriodSeconds = 1f;

        private enum TitleLine
        {
            Squares,
            Cleared,
            Retreated,
            None,
        }

        private enum SquareState
        {
            None,
            Done,
            Now,
            Last,
            Rest,
        }

        private readonly Label _title;
        private readonly VisualElement _time;
        private readonly Label _seconds;
        private readonly VisualElement _squareRow;
        private readonly List<VisualElement> _squares;
        private readonly ShownValue<SquareState>[] _squareStates;
        private readonly VisualElement _gauge;
        private readonly FillBar _gaugeFill;
        private readonly ShownValue<(TitleLine, int, int, WavePhase)> _titleShown = new ShownValue<(TitleLine, int, int, WavePhase)>();
        private readonly ShownValue<bool> _squaresShown = new ShownValue<bool>();
        private readonly ShownValue<bool> _fightingShown = new ShownValue<bool>();
        private readonly ShownValue<(int, bool)> _secondsShown = new ShownValue<(int, bool)>();

        private float _blinkSeconds;

        public WaveSection(VisualElement root)
        {
            _title = root.Need<Label>("wave-title");
            _time = root.Need<VisualElement>("wave-time");
            _seconds = root.Need<Label>("wave-seconds");
            _squareRow = root.Need<VisualElement>("wave-squares");
            _squares = _squareRow.Query(className: "top-bar__square").ToList();
            _squareStates = new ShownValue<SquareState>[_squares.Count];
            for (int i = 0; i < _squares.Count; i++)
            {
                _squareStates[i] = new ShownValue<SquareState>();
            }

            _gauge = root.Need<VisualElement>("wave-gauge");
            _gaugeFill = new FillBar(root.Need<VisualElement>("wave-gauge-fill"));
        }

        public void Reset()
        {
            _blinkSeconds = 0f;
            _titleShown.Reset();
            _squaresShown.Reset();
            _fightingShown.Reset();
            _secondsShown.Reset();
            _gaugeFill.Reset();
            foreach (ShownValue<SquareState> state in _squareStates)
            {
                state.Reset();
            }
        }

        public void Refresh(World world, float gameDeltaSeconds)
        {
            _blinkSeconds += gameDeltaSeconds;
            Journey journey = world.Journey;
            RuinDefinition ruin = journey.CurrentRuin;
            WaveState wave = world.Wave;
            WavePhase phase = world.Visit.Phase;
            bool showsSquares = world.InCombat && wave.Number > 0;
            RefreshTitle(world, ruin, wave, showsSquares);
            if (_squaresShown.Change(showsSquares))
            {
                _squareRow.Show(showsSquares);
            }

            if (showsSquares)
            {
                RefreshSquares(ruin.WaveCount, wave, phase);
            }

            RefreshTime(world, world.InCombat && phase.IsFighting, wave);
        }

        private void RefreshTitle(World world, RuinDefinition ruin, WaveState wave, bool showsSquares)
        {
            TitleLine line = TitleLineOf(world, ruin, wave, showsSquares);
            int count = ruin != null ? ruin.WaveCount : 0;
            if (!_titleShown.Change((line, wave.Number, count, world.Visit.Phase)))
            {
                return;
            }

            switch (line)
            {
                case TitleLine.Squares:
                {
                    _title.text = world.Visit.Phase.Show(new WaveTitles(wave.Number, count));
                    break;
                }

                case TitleLine.Cleared:
                    _title.text = TopBarWords.RuinCleared;
                    break;
                case TitleLine.Retreated:
                    _title.text = TopBarWords.WaveLeftEarly(wave.Number);
                    break;
                default:
                    _title.text = TopBarWords.WaveNone;
                    break;
            }
        }

        private static TitleLine TitleLineOf(World world, RuinDefinition ruin, WaveState wave, bool showsSquares)
        {
            if (showsSquares)
            {
                return TitleLine.Squares;
            }

            if (ruin == null || wave.Number <= 0)
            {
                return TitleLine.None;
            }

            return world.Exploration.HasCleared(ruin) ? TitleLine.Cleared : TitleLine.Retreated;
        }

        private void RefreshTime(World world, bool isFighting, WaveState wave)
        {
            if (_fightingShown.Change(isFighting))
            {
                _time.Show(isFighting);
                _gauge.Show(isFighting);
                _secondsShown.Reset();
            }

            if (!isFighting)
            {
                return;
            }

            float limit = wave.RetreatSeconds;
            float left = wave.SecondsLeft(world.Configuration);
            int seconds = Mathf.CeilToInt(left);
            if (_secondsShown.Change((seconds, wave.HasBoss)))
            {
                _seconds.text = seconds >= 60
                    ? Texts.Monospace(Texts.Minutes(seconds))
                    : TopBarWords.TimeSeconds(Texts.Monospace(seconds));
                _seconds.EnableInClassList("top-bar__seconds--boss", wave.HasBoss);
                _gauge.EnableInClassList("top-bar__wave-gauge--boss", wave.HasBoss);
            }

            _gaugeFill.Set(limit > 0f ? left / limit : 0f);
        }

        private void RefreshSquares(int count, WaveState wave, WavePhase phase)
        {
            for (int i = 0; i < _squares.Count; i++)
            {
                VisualElement square = _squares[i];
                SquareState state = SquareStateOf(i, count, wave, phase);
                if (_squareStates[i].Change(state))
                {
                    square.SetVariant("top-bar__square--", state);
                    if (state != SquareState.Now)
                    {
                        square[0].style.opacity = StyleKeyword.Null;
                    }
                }

                if (state == SquareState.Now)
                {
                    square[0].style.opacity = Mathf.Lerp(BlinkLowOpacity, 1f, Mathf.PingPong(_blinkSeconds, BlinkPeriodSeconds));
                }
            }
        }

        private static SquareState SquareStateOf(int i, int count, WaveState wave, WavePhase phase)
        {
            if (i >= count)
            {
                return SquareState.None;
            }

            int current = wave.Number - 1;
            bool isFighting = phase.IsFighting;
            if (i < current || (i == current && !isFighting))
            {
                return SquareState.Done;
            }

            if (i == current)
            {
                return SquareState.Now;
            }

            return i == count - 1 ? SquareState.Last : SquareState.Rest;
        }
    }
}
