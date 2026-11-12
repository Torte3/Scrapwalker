using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemyFigure
    {
        private const float KnockbackPixelsPerFrame = 6f;
        private const float BarrierFlashFrames = 12f;
        private const float FadePerFrame = 0.025f;
        private const float WanderPixels = 26f;
        private const float WanderRate = 0.006f;
        private const float LockOnShrinkSeconds = 0.18f;
        private const float HitWhite = 0.65f;
        private const float HitFrames = 3f;
        private const float HealGreen = 0.45f;
        private const float HealFlashFrames = 21f;
        private const float HealBarFrames = 42f;
        private const float HealBarFadeFrames = 15f;

        private float _groundX;
        private float _flashFrames;
        private float _flashTotalFrames;
        private Color _flashTint;
        private bool _isFlashFading;
        private float _healBarFrames;
        private float _attackPeriodFrames = 60f;
        private float _framesSinceStrike;
        private int _lastStrikeCount;
        private int _lastBarrierBrokenCount;

        public EnemyFigure(EnemyDefinition definition, float clock, float y, float groundX, IReadOnlyList<MaterialAmount> drops)
        {
            Definition = definition;
            Clock = clock;
            Y = y;
            _groundX = groundX;
            X = groundX;
            Drops = drops;
        }

        public EnemyDefinition Definition { get; }
        public IReadOnlyList<MaterialAmount> Drops { get; }
        public float X { get; private set; }
        public float Y { get; }
        public float Clock { get; private set; }
        public float ClockStep { get; private set; }
        public float? FramesSinceHeal { get; private set; }
        public float HealedHP { get; private set; }
        public float HealedShown => _healBarFrames <= 0f ? 0f : Mathf.Clamp01(_healBarFrames / HealBarFadeFrames);
        public float? LockOnShown { get; private set; }
        public bool IsFlashing => _flashFrames > 0f;
        public Color FlashTint => _isFlashFading ? new Color(_flashTint.r, _flashTint.g, _flashTint.b, _flashTint.a * Mathf.Clamp01(_flashFrames / _flashTotalFrames)) : _flashTint;
        public bool IsSlowed { get; private set; }
        public bool IsAttacking { get; private set; }
        public bool IsFading { get; private set; }
        public float Opacity { get; private set; } = 1f;
        public int BarrierLayers { get; private set; }
        public int BarrierMaxLayers { get; private set; }
        public float BarrierFlash { get; private set; }
        public float HP { get; private set; }
        public float MaxHP { get; private set; }
        public bool HasArmor { get; private set; }
        public bool HasMove { get; private set; }
        public AttackMove Move { get; private set; }
        public IReadOnlyList<Cell> MoveCells { get; private set; }
        public float MoveFrames { get; private set; }

        internal void Follow(Enemy enemy, EnemyTraitReader traits, float tickSeconds, float targetX, float deltaFrames)
        {
            _groundX = targetX > _groundX ? Mathf.MoveTowards(_groundX, targetX, KnockbackPixelsPerFrame * deltaFrames) : targetX;
            X = _groundX;
            IsSlowed = enemy.SlowTicks > 0;
            IsAttacking = enemy.IsInPosition && enemy.Definition.HasAttacks;
            _attackPeriodFrames = enemy.AttackPeriodSeconds * WorldScene.FramesPerSecond;
            bool hasJustStruck = IsAttacking && enemy.StrikeCount != _lastStrikeCount;
            _framesSinceStrike = hasJustStruck ? 0f : _framesSinceStrike + deltaFrames;
            _lastStrikeCount = enemy.StrikeCount;
            Tick(deltaFrames * enemy.Pace);
            FollowBarrier(traits, deltaFrames);
            FollowMove(enemy, tickSeconds, deltaFrames);
            HP = enemy.HP;
            MaxHP = enemy.MaxHP;
            HasArmor = enemy.Definition.Armor > 0f;
            if (_flashFrames > 0f)
            {
                _flashFrames -= deltaFrames;
            }

            if (FramesSinceHeal.HasValue)
            {
                FramesSinceHeal += deltaFrames;
            }

            _healBarFrames = Mathf.Max(0f, _healBarFrames - deltaFrames);
            if (_healBarFrames <= 0f)
            {
                HealedHP = 0f;
            }

            if (IsAttacking)
            {
                X += Mathf.Sin(Clock * 0.3f) * 0.5f;
            }
        }

        private void FollowMove(Enemy enemy, float tickSeconds, float deltaFrames)
        {
            bool isInMove = enemy.Definition.HasAttacks && enemy.Attack.IsInMove;
            float moveFrames = enemy.Attack.Time.Seconds(tickSeconds) * WorldScene.FramesPerSecond;
            if (isInMove && !ReferenceEquals(enemy.Attack.Cells, MoveCells))
            {
                HasMove = true;
                Move = enemy.Definition.AttackMoves[enemy.Attack.Index];
                MoveCells = enemy.Attack.Cells;
                MoveFrames = moveFrames;
                return;
            }

            MoveFrames += deltaFrames;
            if (isInMove)
            {
                MoveFrames = Mathf.Max(MoveFrames, moveFrames);
            }
        }

        private void FollowBarrier(EnemyTraitReader traits, float deltaFrames)
        {
            BarrierFlash = Mathf.Max(0f, BarrierFlash - deltaFrames / BarrierFlashFrames);
            if (traits.BarrierBrokenCount != _lastBarrierBrokenCount)
            {
                BarrierFlash = 1f;
                _lastBarrierBrokenCount = traits.BarrierBrokenCount;
            }

            BarrierLayers = traits.BarrierLayers;
            BarrierMaxLayers = traits.BarrierMaxLayers;
        }

        public void FollowLockOn(bool isLockedOn, float realSeconds)
        {
            LockOnShown = !isLockedOn ? (float?)null : !LockOnShown.HasValue ? 0f : Mathf.Min(1f, LockOnShown.Value + realSeconds / LockOnShrinkSeconds);
        }

        public void FlashHit()
        {
            StartFlash(new Color(1f, 1f, 1f, HitWhite), HitFrames, false);
        }

        public void ShowHealed(float amount)
        {
            StartFlash(Palette.HealNumber.WithAlpha(HealGreen), HealFlashFrames, true);
            HealedHP = _healBarFrames > 0f ? HealedHP + amount : amount;
            _healBarFrames = HealBarFrames;
        }

        public void PulseHeal()
        {
            FramesSinceHeal = 0f;
        }

        private void Tick(float step)
        {
            ClockStep = step;
            Clock += step;
        }

        private void StartFlash(Color tint, float frames, bool isFading)
        {
            _flashTint = tint;
            _flashFrames = frames;
            _flashTotalFrames = frames;
            _isFlashFading = isFading;
        }

        public void StartFading()
        {
            IsFading = true;
            Opacity = 1f;
        }

        public bool FadeOut(float deltaFrames)
        {
            Tick(deltaFrames);
            Opacity -= FadePerFrame * deltaFrames;
            return Opacity <= 0f;
        }

        public void Wander(float home, float deltaFrames)
        {
            Tick(deltaFrames);
            X = home + Mathf.Sin(Clock * WanderRate) * WanderPixels;
        }

        public float AttackCycle(float firePhase)
        {
            return (Mathf.Repeat(_framesSinceStrike / Mathf.Max(1f, _attackPeriodFrames), 1f) + firePhase) % 1f;
        }
    }
}
