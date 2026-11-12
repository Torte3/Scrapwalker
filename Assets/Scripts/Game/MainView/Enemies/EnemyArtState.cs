using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    internal sealed class EnemyArtState : ArtState
    {
        private const float PastFrameEdge = 1e-4f;

        private readonly WorldScene _scene;
        private EnemyFigure _figure;
        private ArtEntry _body;
        private ArtEntry _attack;
        private ArtCue _moveCue;
        private float[] _turns;
        private float _flyFrames;
        private float _artFireFrames;
        private bool _isCalm;

        public EnemyArtState(WorldScene scene)
        {
            _scene = scene;
        }

        public ArtEntry Overlay { get; private set; }
        public bool IsInMove { get; private set; }
        public int Launched { get; private set; }

        public override float Clock => _figure.Clock;
        public override bool IsWalking => !_isCalm && !_figure.IsAttacking;
        public override bool IsAttacking => _figure.IsAttacking;
        public override bool Spins => true;
        public override Vector2 Origin => _body.Origin;
        public override int ShotCount => _figure.Move.ShotCount;
        public override float MoveFrames => _figure.MoveFrames;
        public override Color? Flash => _figure.IsFlashing ? _figure.FlashTint : (Color?)null;
        protected override ArtCue? PlayingMove => IsInMove ? _moveCue : (ArtCue?)null;

        private bool IsHeld => _figure.Move.HasOneTarget;

        private float FramesSinceLaunch => _figure.MoveFrames - LaunchFrame(IsHeld ? 0 : Launched - 1);

        public void Begin(EnemyFigure figure, ArtEntry body, ArtEntry attack, ArtCue moveCue, float[] turns, bool isCalm, bool advances)
        {
            _figure = figure;
            _body = body;
            _attack = attack;
            _moveCue = moveCue;
            _turns = turns;
            _isCalm = isCalm;
            ArtEntry candidate = isCalm || !figure.HasMove ? null : attack;
            _flyFrames = candidate != null ? FlyOf(candidate) : 0f;
            _artFireFrames = candidate != null ? ArtFireOf(candidate) : 0f;
            IsInMove = candidate != null && figure.MoveFrames < AttackEnd(candidate);
            Overlay = IsInMove ? candidate : null;
            Launched = IsInMove ? CountLaunched() : 0;
            if (advances)
            {
                AdvanceTurns();
            }
        }

        public override bool IsShown(ArtLayer layer)
        {
            if (!IsOverlay(layer))
            {
                return base.IsShown(layer);
            }

            if (!IsInMove)
            {
                return false;
            }

            bool isRunning = Runs(layer.Cue);
            return isRunning && (layer.Find<ArtPathShot>() != null || PhaseOf(layer).HasValue);
        }

        public override ArtFrame FrameOf(ArtLayer layer)
        {
            if (IsOverlay(layer))
            {
                float? phase = PhaseOf(layer);
                return layer.At((Mathf.Max(0f, phase ?? 0f) + PastFrameEdge) * layer.Period);
            }

            if (layer.Condition == ArtCondition.Attack)
            {
                return layer.At(AttackCycle(layer) * layer.Period);
            }

            if (layer.Play == ArtPlay.Value)
            {
                return layer.ByValue(ValueOf(layer));
            }

            bool restsByCue = layer.Cue != ArtCue.Always && layer.Find<ArtTurn>() == null && layer.Find<ArtLean>() == null;
            if (!restsByCue)
            {
                return layer.At(Clock);
            }

            if (!Runs(layer.Cue))
            {
                return layer.First;
            }

            if (layer.Play == ArtPlay.Trigger)
            {
                return ArtFrames < layer.Length ? layer.At(ArtFrames) : layer.First;
            }

            return layer.At(Clock);
        }

        public override float ValueOf(ArtLayer layer)
        {
            ArtPods pods = layer.Find<ArtPods>();
            int podCount = pods != null ? pods.Pods : 0;
            int steps = layer.Values.Length - 1 - podCount;
            int fired = 0;
            if (podCount > 0 && Runs(layer.Cue))
            {
                int firstPod = _figure.Move.ShotCount - podCount;
                fired = Mathf.Clamp(Launched - firstPod, 0, podCount);
            }

            float openness = pods != null ? Openness(layer, pods.Opening) : 0f;
            return fired > 0 ? steps + fired : Mathf.Round(openness * steps);
        }

        public override float AttackCycle(ArtLayer layer)
        {
            return _figure.AttackCycle(layer.FirePhase);
        }

        public override float SpunAngle(ArtLayer layer)
        {
            int i = System.Array.IndexOf(_body.Layers, layer);
            return i >= 0 ? _turns[i] : 0f;
        }

        public override float Openness(ArtLayer layer, in ArtOpening opening)
        {
            if (!opening.Opens || !Runs(layer.Cue))
            {
                return 0f;
            }

            return opening.At(_figure.MoveFrames, _figure.Move.Duration * WorldScene.FramesPerSecond);
        }

        public override float RecoilLeft(ArtLayer layer, float frames)
        {
            if (!Runs(layer.Cue) || Launched <= 0)
            {
                return 0f;
            }

            return 1f - Mathf.Clamp01(FramesSinceLaunch / frames);
        }

        public override bool TryGetCurrentCell(out Vector2 cell)
        {
            return TryGetShotCell(Mathf.Max(0, Launched - 1), out cell);
        }

        public override bool TryGetShotCell(int shot, out Vector2 cell)
        {
            bool isKnown = _figure.MoveCells != null && shot < _figure.MoveCells.Count;
            cell = isKnown ? _scene.CellCenter(_figure.MoveCells[shot]) : default;
            return isKnown;
        }

        public override float LaunchFrame(int shot)
        {
            return _figure.Move.ImpactAt(shot) * WorldScene.FramesPerSecond - _flyFrames;
        }

        private float ArtFrames
        {
            get
            {
                float t = _figure.MoveFrames;
                float first = LaunchFrame(0);
                if (_artFireFrames <= 0f || first <= 0f)
                {
                    return t;
                }

                if (t < first)
                {
                    return t * _artFireFrames / first;
                }

                return _artFireFrames + Mathf.Max(0f, t - LaunchFrame(_figure.Move.ShotCount - 1));
            }
        }

        private bool IsOverlay(ArtLayer layer)
        {
            return _attack != null && System.Array.IndexOf(_attack.Layers, layer) >= 0;
        }

        private bool IsForCurrentMove(ArtLayer layer)
        {
            return layer.Cue == ArtCue.Always || layer.Cue == _moveCue;
        }

        private float? PhaseOf(ArtLayer layer)
        {
            float fire = layer.FirePhase;
            float period = Mathf.Max(1f, layer.Period);
            float t = _figure.MoveFrames;
            float first = LaunchFrame(0);
            float last = LaunchFrame(_figure.Move.ShotCount - 1);
            if (t < first)
            {
                return first > 0f ? fire * Mathf.Max(0f, t) / first : fire;
            }

            float firing = (1f - fire) * period;
            if (firing <= 0f)
            {
                return t <= last ? fire : (float?)null;
            }

            float held = IsHeld ? 0f : Mathf.Repeat(Mathf.Min(t, last) - first, firing);
            float u = held + Mathf.Max(0f, t - last);
            return u < firing ? fire + u / period : (float?)null;
        }

        private int CountLaunched()
        {
            int n = _figure.Move.ShotCount;
            int launched = 0;
            while (launched < n && LaunchFrame(launched) <= _figure.MoveFrames)
            {
                launched++;
            }

            return launched;
        }

        private void AdvanceTurns()
        {
            ArtLayer[] layers = _body.Layers;
            for (int i = 0; i < layers.Length; i++)
            {
                ArtTurn turn = layers[i].Find<ArtTurn>();
                if (turn != null)
                {
                    _turns[i] += turn.SpeedWhile(IsInMove) * _figure.ClockStep;
                }
            }
        }

        private float FlyOf(ArtEntry attack)
        {
            float fly = 0f;
            foreach (ArtLayer layer in attack.Layers)
            {
                ArtPathShot path = layer.Find<ArtPathShot>();
                if (path != null && IsForCurrentMove(layer))
                {
                    fly = Mathf.Max(fly, path.FlyFrames);
                }
            }

            return fly;
        }

        private float ArtFireOf(ArtEntry attack)
        {
            float fire = 0f;
            foreach (ArtLayer layer in attack.Layers)
            {
                if (layer.IsLoop && IsForCurrentMove(layer))
                {
                    fire = Mathf.Max(fire, layer.FirePhase * layer.Period);
                }
            }

            return fire;
        }

        private float AttackEnd(ArtEntry attack)
        {
            float end = _figure.Move.Duration * WorldScene.FramesPerSecond;
            float first = LaunchFrame(0);
            float last = LaunchFrame(_figure.Move.ShotCount - 1);
            foreach (ArtLayer layer in attack.Layers)
            {
                if (!layer.IsLoop)
                {
                    continue;
                }

                float firing = (1f - layer.FirePhase) * Mathf.Max(1f, layer.Period);
                if (firing > 0f)
                {
                    end = Mathf.Max(end, last + firing - (IsHeld ? 0f : Mathf.Repeat(last - first, firing)));
                }
            }

            return end;
        }
    }
}
