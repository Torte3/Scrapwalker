using Scrapwalker.Game.MainView.Backdrop;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using Scrapwalker.Game.MainView.Outpost.Limbs;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Launch
{
    public sealed class LaunchView
    {
        private readonly ArmMotion _arm;
        private readonly SpacePainter _space;

        public LaunchView(WorldScene scene, ArtParts parts, EffectsPainter effects, ArmMotion arm)
        {
            _arm = arm;
            _space = new SpacePainter(scene, parts);
            BoosterMotion = new BoosterMotion(scene);
            Booster = new BoosterPainter(scene, BoosterMotion, parts, effects);
            Motion = new LaunchMotion(scene);
            Painter = new LaunchPainter(scene, parts, effects, Motion, BoosterMotion, Booster);
        }

        public BoosterMotion BoosterMotion { get; }
        public BoosterPainter Booster { get; }
        public LaunchMotion Motion { get; }
        public LaunchPainter Painter { get; }
        public Vector2 Pan => Motion.Pan + Vector2.left * (BoosterMotion.BackViewPixels * Motion.BackViewWeight);

        public void Clear()
        {
            BoosterMotion.Clear();
            Painter.Clear();
            Motion.End();
        }

        public void StepMotion(in LaunchVisual launch, float realFrames)
        {
            if (launch.IsLaunching && !Motion.IsActive)
            {
                Motion.Begin();
                _arm.BeginStow();
            }
            else if (!launch.IsLaunching && Motion.IsActive)
            {
                Motion.End();
            }

            if (Motion.IsActive)
            {
                Motion.Step(launch.LaunchFrames);
                if (launch.IsLaunchOver)
                {
                    Motion.Coast(realFrames);
                }
            }
        }

        public void DrawSpace(Rect view)
        {
            if (Motion.IsActive)
            {
                _space.Draw(view, Motion.StarX, Motion.StarLift);
            }
        }

        public void DrawBooster()
        {
            Booster.Draw();
            Painter.DrawWheels();
        }
    }
}
