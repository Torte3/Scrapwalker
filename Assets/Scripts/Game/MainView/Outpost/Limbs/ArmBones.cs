using Scrapwalker.Game.Look.Art.Parts;
using Scrapwalker.Game.Look.Art.Sets;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Outpost.Limbs
{
    public sealed class ArmBones
    {
        public ArmBones(ArmArt arm)
        {
            ArtRig forearm = arm.Fore.Find<ArtRig>();
            ArtRig tube = arm.CylinderTube.Find<ArtRig>();
            ArtRig rod = arm.CylinderRod.Find<ArtRig>();
            UpperArm = arm.Upper.Find<ArtRig>().Length;
            Forearm = forearm.Length;
            Grip = forearm.Grip;
            ClawTop = arm.ClawTop.Find<ArtRig>().At;
            ClawBottom = arm.ClawBottom.Find<ArtRig>().At;
            Bundle = arm.Bundle.Find<ArtRig>().At;
            LiftRoot = tube.Points[0];
            BendRoot = tube.Points[1];
            LiftTip = rod.Points[0];
            BendTip = rod.Points[1];
        }

        public float UpperArm { get; }
        public float Forearm { get; }
        public float Grip { get; }
        public Vector2 ClawTop { get; }
        public Vector2 ClawBottom { get; }
        public Vector2 Bundle { get; }
        public Vector2 LiftRoot { get; }
        public Vector2 LiftTip { get; }
        public Vector2 BendRoot { get; }
        public Vector2 BendTip { get; }
    }
}
