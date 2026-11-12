using System.Collections.Generic;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView.Backdrop;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemyPainter : WorldPainter
    {
        private const float DecorationHome = 1750f;
        private const float DecorationScale = 0.8f;
        private const float DecorationLift = 10f;
        private const float NumberLift = 20f;
        private static readonly SparkBurst ArmorSparks = new SparkBurst(5, new Vector2(0f, 10f), new Vector2(-1f, -3.5f), new Vector2(0f, -2.5f), 14f, 0f, Palette.ArmorSpark, 2f, 0f, 0.15f);

        private readonly EnemyLooks _looks;
        private readonly EnemySprites _sprites;
        private readonly ArtParts _parts;
        private readonly EffectsPainter _effects;
        private readonly DropPainter _drops;
        private readonly EnemyHUD _hud;
        private readonly HealAreaPainter _healAreas;
        private readonly BarrierPainter _barriers;
        private readonly StunPainter _stun;
        private readonly Dictionary<int, EnemyFigure> _alive = new Dictionary<int, EnemyFigure>();
        private readonly List<EnemyFigure> _fading = new List<EnemyFigure>();
        private readonly List<DecorationEnemy> _decoration = new List<DecorationEnemy>();
        private readonly HashSet<int> _killed = new HashSet<int>();
        private readonly HashSet<int> _seen = new HashSet<int>();
        private readonly List<int> _gone = new List<int>();
        private readonly List<int> _drawOrder = new List<int>();
        private readonly EnemyTraitReader _traits = new EnemyTraitReader();
        private readonly EnemyDefinition _decorationEnemy;

        public EnemyPainter(WorldScene scene, EnemyLooks looks, ArtParts parts, ArtPlayer player, ArcMesh arcs, EffectsPainter effects, DropPainter drops, WorldUI ui, EnemyDefinition decorationEnemy)
            : base(scene)
        {
            _looks = looks;
            _sprites = new EnemySprites(scene, player, looks);
            _decorationEnemy = decorationEnemy;
            _parts = parts;
            _effects = effects;
            _drops = drops;
            _hud = new EnemyHUD(ui);
            _healAreas = new HealAreaPainter(scene, parts);
            _barriers = new BarrierPainter(arcs);
            _stun = new StunPainter(scene, parts);
        }

        private sealed class DecorationEnemy
        {
            public EnemyFigure Figure;
            public float Home;
            public float Scale;
        }

        public void Clear()
        {
            _alive.Clear();
            _fading.Clear();
            _hud.Numbers.Clear();
            _sprites.ForgetAll();
        }

        public void OnLanded()
        {
            foreach (EnemyFigure figure in _alive.Values)
            {
                _sprites.Forget(figure);
            }

            _alive.Clear();
            _killed.Clear();
        }

        public void OnKilled(int enemyID)
        {
            _killed.Add(enemyID);
        }

        public void ClearRuinDecoration()
        {
            _decoration.Clear();
        }

        public void LeaveRuin()
        {
            foreach (EnemyFigure figure in _alive.Values)
            {
                figure.StartFading();
                _fading.Add(figure);
            }

            _alive.Clear();
            _decoration.Clear();
            var decorationFigure = new EnemyFigure(_decorationEnemy, 0f, WorldScene.Ground - DecorationLift, DecorationHome, System.Array.Empty<MaterialAmount>());
            _decoration.Add(new DecorationEnemy { Figure = decorationFigure, Home = DecorationHome, Scale = DecorationScale });
        }

        public void ShowImpact(in EnemyImpact impact)
        {
            if (!_alive.TryGetValue(impact.EnemyID, out EnemyFigure figure))
            {
                return;
            }

            if (impact.Damage > 0f)
            {
                figure.FlashHit();
            }

            if (impact.Absorbed > 0f)
            {
                SparkArmor(figure);
            }

            _hud.Numbers.Add(impact, HeadOf(figure));
        }

        public void ShowHeal(EnemiesHealed healed)
        {
            if (_alive.TryGetValue(healed.HealerID, out EnemyFigure healer))
            {
                healer.PulseHeal();
            }

            foreach (EnemyHeal heal in healed.Heals)
            {
                if (_alive.TryGetValue(heal.EnemyID, out EnemyFigure figure))
                {
                    figure.ShowHealed(heal.Amount);
                    _hud.Numbers.AddHeal(heal, HeadOf(figure));
                }
            }
        }

        private void SparkArmor(EnemyFigure figure)
        {
            Vector2 at = OnBody(figure, -0.35f, 0.55f);
            _effects.Emit(ArmorSparks, at);
        }

        private Vector2 HeadOf(EnemyFigure figure)
        {
            return OnBody(figure, 0f, 1f) - new Vector2(0f, NumberLift);
        }

        public void Strike(int enemyID, Vector2 hit, ShotPainter shots)
        {
            if (!_alive.TryGetValue(enemyID, out EnemyFigure figure))
            {
                return;
            }

            EnemyLook look = _looks.Of(figure.Definition);
            Vector2 offset = look.StrikeFrom;
            look.Strike(shots, new Vector2(ShownX(figure) + offset.x, figure.Y + offset.y), hit);
        }

        public float CenterPixels
        {
            get
            {
                if (_alive.Count == 0)
                {
                    return WorldScene.EnemySpawnX + Scene.RuinShift;
                }

                float near = float.MaxValue;
                float far = float.MinValue;
                foreach (EnemyFigure figure in _alive.Values)
                {
                    float x = OnBody(figure, 0f, 0f).x;
                    near = Mathf.Min(near, x);
                    far = Mathf.Max(far, x);
                }

                return (near + far) * 0.5f;
            }
        }

        private float XOf(EnemyDefinition definition, float distance)
        {
            return Scene.FrontXOf(distance) - _looks.Of(definition).Front;
        }

        public void Step(Battle battle, float tickSeconds, float deltaFrames, float realSeconds)
        {
            FollowAlive(battle, tickSeconds, deltaFrames, realSeconds);
            RetireGone();
            FadeOut(deltaFrames);
            _hud.Numbers.Step(deltaFrames);
            foreach (KeyValuePair<int, EnemyFigure> pair in _alive)
            {
                _hud.Numbers.Follow(pair.Key, HeadOf(pair.Value));
            }

            foreach (DecorationEnemy decoration in _decoration)
            {
                decoration.Figure.Wander(decoration.Home, deltaFrames);
            }
        }

        private void FollowAlive(Battle battle, float tickSeconds, float deltaFrames, float realSeconds)
        {
            _seen.Clear();
            Enemy lockedOn = battle.LockedOn;
            IReadOnlyList<Enemy> enemies = battle.Enemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                if (!enemy.IsAlive)
                {
                    continue;
                }

                _seen.Add(enemy.ID);
                float target = XOf(enemy.Definition, enemy.Distance);
                if (!_alive.TryGetValue(enemy.ID, out EnemyFigure figure))
                {
                    figure = new EnemyFigure(enemy.Definition, Scene.Random() * 100f, WorldScene.Ground, target, enemy.Drops);
                    _alive[enemy.ID] = figure;
                }

                figure.Follow(enemy, _traits.Read(enemy), tickSeconds, target, deltaFrames);
                figure.FollowLockOn(enemy == lockedOn, realSeconds);
            }
        }

        private void RetireGone()
        {
            _gone.Clear();
            foreach (KeyValuePair<int, EnemyFigure> pair in _alive)
            {
                if (!_seen.Contains(pair.Key))
                {
                    _gone.Add(pair.Key);
                }
            }

            foreach (int id in _gone)
            {
                EnemyFigure figure = _alive[id];
                _alive.Remove(id);
                if (_killed.Contains(id))
                {
                    Explode(figure);
                }
                else
                {
                    figure.StartFading();
                    _fading.Add(figure);
                }
            }

            _killed.Clear();
        }

        private void FadeOut(float deltaFrames)
        {
            for (int i = _fading.Count - 1; i >= 0; i--)
            {
                if (_fading[i].FadeOut(deltaFrames))
                {
                    _sprites.Forget(_fading[i]);
                    _fading.RemoveAt(i);
                }
            }
        }

        private void Explode(EnemyFigure figure)
        {
            float x = OnBody(figure, 0f, 0f).x;
            _looks.Of(figure.Definition).Explode(_effects, x, figure.Y);
            _drops.Spawn(x, figure.Y, figure.Drops);

            _sprites.Forget(figure);
        }

        public void DrawDecoration()
        {
            foreach (DecorationEnemy decoration in _decoration)
            {
                DrawBody(decoration.Figure, EnemyBodyStyle.FarAway(decoration.Scale), DrawBand.Backdrop);
            }
        }

        public void DrawFading()
        {
            for (int i = 0; i < _fading.Count; i++)
            {
                DrawBody(_fading[i], EnemyBodyStyle.Fading, DrawBand.Ground);
            }
        }

        public void DrawAlive(IReadOnlyList<Enemy> enemies)
        {
            DrawHealAreas(enemies, true);
            _drawOrder.Clear();
            _drawOrder.AddRange(_alive.Keys);
            _drawOrder.Sort();
            for (int i = 0; i < _drawOrder.Count; i++)
            {
                DrawBody(_alive[_drawOrder[i]], EnemyBodyStyle.Alive, DrawBand.Enemies);
            }

            foreach (EnemyFigure figure in _alive.Values)
            {
                if (figure.BarrierMaxLayers > 0)
                {
                    _barriers.Draw(figure, _looks.Of(figure.Definition), OnBody(figure, 0f, 0.5f));
                }
            }

            foreach (int id in _drawOrder)
            {
                EnemyFigure figure = _alive[id];
                InFigure(figure, EnemyBodyStyle.Alive, () => _sprites.DrawAboveFigures(figure, EnemyBodyStyle.Alive, DrawBand.Enemies));
            }


            _hud.BeginBars();
            foreach (EnemyFigure figure in _alive.Values)
            {
                _hud.DrawHPBar(figure, OnBody(figure, 0f, 1f), _looks.Of(figure.Definition).Size.x);
            }

            foreach (EnemyFigure figure in _alive.Values)
            {
                if (figure.LockOnShown.HasValue)
                {
                    _hud.DrawLockOnMark(BodyRect(figure), figure.LockOnShown.Value);
                }
            }

            _hud.DrawNumbers();
        }

        public void DrawHealFronts(IReadOnlyList<Enemy> enemies)
        {
            DrawHealAreas(enemies, false);
        }

        private void DrawHealAreas(IReadOnlyList<Enemy> enemies, bool isBack)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                if (!enemy.IsAlive || !enemy.IsInPosition || !_alive.TryGetValue(enemy.ID, out EnemyFigure figure))
                {
                    continue;
                }

                float healReach = _traits.Read(enemy).HealReach;
                if (healReach <= 0f)
                {
                    continue;
                }

                float x0 = Scene.FrontXOf(enemy.Distance - healReach) + Scene.RuinShift;
                float x1 = Scene.FrontXOf(enemy.Distance + healReach) + Scene.RuinShift;
                if (isBack)
                {
                    _healAreas.DrawBack(x0, x1, figure.Y, figure.FramesSinceHeal);
                    continue;
                }

                EnemyLook look = _looks.Of(figure.Definition);
                Vector2 eye = new Vector2(ShownX(figure) + look.Eye.x, figure.Y + look.Eye.y + look.BobAt(figure.Clock));
                _healAreas.DrawFront(x0, x1, figure.Y, OnBody(figure, 0f, 0f).x, eye, figure.FramesSinceHeal);
            }
        }

        private void DrawBody(EnemyFigure figure, EnemyBodyStyle style, DrawBand band)
        {
            EnemyLook look = _looks.Of(figure.Definition);
            float scale = style.Scale;
            Vector2 size = look.Size * scale;
            Canvas.Save();
            Canvas.Translate(ShownX(figure), figure.Y);
            Canvas.Alpha = style.Opacity * Mathf.Max(0f, figure.Opacity);
            _parts.Shadow(look.Middle * scale, 0f, size.x * 0.6f, 3f * scale, Color.white, band);
            Canvas.Scale(scale, scale);
            _sprites.Draw(figure, style, band);
            if (figure.IsSlowed && style.ShowsStun)
            {
                _stun.Draw(look, figure.Clock, band);
            }

            Canvas.Restore();
        }

        private void InFigure(EnemyFigure figure, in EnemyBodyStyle style, System.Action draw)
        {
            Canvas.Save();
            Canvas.Translate(ShownX(figure), figure.Y);
            Canvas.Alpha = style.Opacity * Mathf.Max(0f, figure.Opacity);
            Canvas.Scale(style.Scale, style.Scale);
            draw();
            Canvas.Restore();
        }

        public int EnemyAt(Vector2 pixels)
        {
            int found = 0;
            foreach (KeyValuePair<int, EnemyFigure> pair in _alive)
            {
                if (pair.Key > found && BodyRect(pair.Value).Contains(pixels))
                {
                    found = pair.Key;
                }
            }

            return found;
        }

        private Rect BodyRect(EnemyFigure figure)
        {
            Vector2 size = _looks.Of(figure.Definition).Size;
            Vector2 topLeft = OnBody(figure, -0.5f, 1f);
            return new Rect(topLeft.x, topLeft.y, size.x, size.y);
        }

        private float ShownX(EnemyFigure figure)
        {
            return figure.X + Scene.RuinShift;
        }

        private Vector2 OnBody(EnemyFigure figure, float across, float up)
        {
            return _looks.Of(figure.Definition).PointOn(ShownX(figure), figure.Y, across, up);
        }
    }
}
