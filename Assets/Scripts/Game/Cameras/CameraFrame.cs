using UnityEngine;

namespace Scrapwalker.Game.Cameras
{
    public readonly struct CameraFrame
    {
        public CameraFrame(in CameraCue cue, bool isScrollable, Vector2 cursor, float deltaSeconds, float realDeltaSeconds, float aside, Vector2? panelSize)
        {
            RealDeltaSeconds = realDeltaSeconds;
            Cue = cue;
            PanelSize = panelSize;
            Aside = aside;
            IsScrollable = isScrollable;
            Cursor = cursor;
            DeltaSeconds = deltaSeconds;
        }

        public CameraCue Cue { get; }
        public float Aside { get; }
        public Vector2? PanelSize { get; }

        public bool IsScrollable { get; }
        public Vector2 Cursor { get; }
        public float DeltaSeconds { get; }
        public float RealDeltaSeconds { get; }
    }
}
