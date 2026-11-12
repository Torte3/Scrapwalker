using UnityEngine;
using static Scrapwalker.Game.Look.Pen;

namespace Scrapwalker.Game.MainView.Outpost.Limbs
{
    public static class TwoBoneLimb
    {
        public static Vector2 Joint(Vector2 root, Vector2 end, float upper, float lower, float bendSign)
        {
            Vector2 d = end - root;
            float distance = d.magnitude;
            float toEnd = distance < 1f ? Mathf.PI * 0.5f : Mathf.Atan2(d.y, d.x);
            float angle = toEnd + bendSign * TwoBoneIK(upper, lower, distance);
            return new Vector2(root.x + Mathf.Cos(angle) * upper, root.y + Mathf.Sin(angle) * upper);
        }

        public static void DrawCylinder(ArtParts parts, Look.Art.ArtLayer rod, Look.Art.ArtLayer tube, Vector2 root, Vector2 tip, DrawBand band)
        {
            float angle = Direction(root, tip);
            parts.PutAt(rod, tip, angle, band);
            parts.PutAt(tube, root, angle, band);
        }
    }
}
