using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemyLook
    {
        private readonly ArtDrift _drift;
        private readonly Vector2[] _booms;
        private Rect? _box;

        public EnemyLook(EnemyDefinition definition, EnemyArt art)
        {
            Definition = definition;
            Calm = art.Calm;
            Alert = art.Alert;
            Attack = art.Attack;
            _booms = art.Booms;
            _drift = art.Body != null ? art.Body.Find<ArtDrift>() : null;
            ArtRig rig = art.Body != null ? art.Body.Find<ArtRig>() : null;
            Eye = rig != null ? rig.At : Vector2.zero;
            Bolt = Alert != null ? Alert.Find<ArtBolt>() : null;
        }

        public EnemyDefinition Definition { get; }
        public ArtEntry Calm { get; }
        public ArtEntry Alert { get; }
        public ArtEntry Attack { get; }
        public Vector2 Eye { get; }
        public ArtBolt Bolt { get; }

        public Vector2 StrikeFrom => Bolt != null ? Bolt.From : new Vector2(Front, 0f);

        public void Strike(ShotPainter shots, Vector2 from, Vector2 hit)
        {
            if (Attack == null)
            {
                shots.AddStrikeLine(from, hit, Bolt);
            }

            shots.AddStrikeSparks(hit);
        }

        public void Explode(EffectsPainter effects, float x, float groundY)
        {
            foreach (Vector2 boom in _booms)
            {
                effects.Boom(x + boom.x, groundY + boom.y);
            }
        }

        public float BobAt(float clock)
        {
            return _drift != null ? _drift.Bob(clock) : 0f;
        }

        public Vector2 Size => Box.size;

        public float Front => Box.xMin;
        public float Middle => Box.center.x;

        public Vector2 PointOn(float x, float groundY, float across, float up)
        {
            return new Vector2(x + Middle + across * Size.x, groundY - up * Size.y);
        }

        private Rect Box
        {
            get
            {
                if (!_box.HasValue)
                {
                    _box = BoxOf(Calm ?? throw new KeyNotFoundException("敵の絵が無い: " + Definition.Key));
                }

                return _box.Value;
            }
        }

        private static Rect BoxOf(ArtEntry calm)
        {
            float left = float.MaxValue;
            float right = float.MinValue;
            float top = 0f;
            foreach (ArtLayer layer in calm.Layers)
            {
                Sprite sprite = layer.First?.Sprite;
                if (layer.IsAdditive || sprite == null)
                {
                    continue;
                }

                Bounds bounds = sprite.bounds;
                left = Mathf.Min(left, bounds.min.x);
                right = Mathf.Max(right, bounds.max.x);
                top = Mathf.Max(top, bounds.max.y);
            }

            return new Rect(left, 0f, right - left, top);
        }
    }
}
