using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtTurn : ArtPart
    {
        [SerializeField] private Vector2 _pivot;
        [SerializeField] private float _rest;
        [SerializeField] private ArtWave _wave;
        [SerializeField] private float _speed;
        [SerializeField] private float _attackSpeed;
        [SerializeField] private ArtOpening _opening;
        [SerializeField] private float _openAngle;

        public ArtTurn(Vector2 pivot, float rest, ArtWave wave, float speed, float attackSpeed, ArtOpening opening, float openAngle)
        {
            _pivot = pivot;
            _rest = rest;
            _wave = wave;
            _speed = speed;
            _attackSpeed = attackSpeed;
            _opening = opening;
            _openAngle = openAngle;
        }

        public Vector2 Pivot => _pivot;

        public float SpeedWhile(bool isAttacking)
        {
            return isAttacking ? _attackSpeed : _speed;
        }

        public float Angle(ArtState state, ArtLayer layer)
        {
            if (_opening.Opens)
            {
                return _openAngle * state.Openness(layer, _opening);
            }

            return state.Spins ? state.SpunAngle(layer) : _wave.At(state.IsActing, state.Clock) - _rest;
        }

        public override bool Place(IArtStage stage, ArtState state, ArtLayer layer)
        {
            float angle = Angle(state, layer);
            ArtTurn parent = layer.Parent != null ? layer.Parent.Find<ArtTurn>() : null;
            if (parent != null)
            {
                angle -= parent.Angle(state, layer.Parent);
            }

            stage.RotateAbout(_pivot, angle);
            return true;
        }
    }
}
