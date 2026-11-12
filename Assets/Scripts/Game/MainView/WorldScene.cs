using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class WorldScene
    {
        public const float ScreenWidth = 1600f;
        public const float ScreenHeight = 900f;
        public const float Ground = 686f;
        public const float CellSize = FacilityArt.CellSize;
        public const float HullFloorY = Ground - 10f;
        public const float HullLeft = 54f;
        public const float Armor = 14f;

        public const float FramesPerSecond = 60f;

        public const int LabelOrder = 8;
        public const float BotFootLift = 4f;

        public static readonly Vector2 GashOffset = new Vector2(-254f, -50f);
        public const float GashLeft = 1018f;

        public static readonly Vector2 GashTarget = new Vector2(1066f, 562f) + GashOffset;

        public const float EnemySpawnX = 1300f;

        public const float CoverLeft = -2000f;
        public const float CoverWidth = 6000f;

        private const float ShakeDecayPerFrame = 0.9f;

        private readonly Jitter _jitter;
        private float _lastFrameWorldX;

        public WorldScene(CanvasStack canvas, Jitter jitter)
        {
            Canvas = canvas;
            _jitter = jitter;
        }

        public CanvasStack Canvas { get; }

        public int Columns { get; private set; }
        public int Rows { get; private set; }
        public float SpawnDistance { get; private set; }

        public float ElapsedFrames { get; private set; }
        public float StepFrames { get; private set; }
        public float StepSeconds { get; private set; }

        public float BaseX { get; set; }
        public float BaseY { get; set; }
        public float WalkDistance { get; set; }
        public float WorldX { get; set; }
        public float CameraLag { get; set; }
        public float Rise { get; set; }

        public float Tilt { get; set; }

        public float Shake { get; private set; }

        public float RuinOff { get; set; }

        public float RuinShift => WorldX + RuinOff;

        public float ScrollPixels => WorldX - _lastFrameWorldX;

        public float HullWidth => CellSize * Columns;
        public float HullTop => HullFloorY - CellSize * Rows;
        public float WallL => HullLeft - Armor;
        public float WallR => HullLeft + HullWidth + Armor;

        public float PixelsPerDistance => (EnemySpawnX - WallR) / Mathf.Max(1f, SpawnDistance);

        public float FrontXOf(float distance)
        {
            return WallR + Mathf.Max(0f, distance) * PixelsPerDistance;
        }

        public void SetGrid(int columns, int rows, float spawnDistance)
        {
            Columns = columns;
            Rows = rows;
            SpawnDistance = spawnDistance;
        }

        public static Vector2 UnitsOfPixels(Vector2 pixels)
        {
            return new Vector2((pixels.x - HullLeft) / CellSize, (HullFloorY - pixels.y) / CellSize);
        }

        public static Vector2 PixelsOfUnits(Vector2 units)
        {
            return new Vector2(HullLeft + units.x * CellSize, HullFloorY - units.y * CellSize);
        }

        public static float CellLeft(float column)
        {
            return HullLeft + column * CellSize;
        }

        public static float FloorY(float row)
        {
            return HullFloorY - row * CellSize;
        }

        public Vector2 CellCenter(Cell cell)
        {
            return new Vector2(CellLeft(cell.Column + 0.5f) + BaseX, FloorY(cell.Row + 0.5f) + BaseY);
        }

        public void Advance(float seconds)
        {
            StepSeconds = seconds;
            StepFrames = seconds * FramesPerSecond;
            ElapsedFrames += StepFrames;
            Shake *= Mathf.Pow(ShakeDecayPerFrame, StepFrames);
        }

        public void EndFrame()
        {
            _lastFrameWorldX = WorldX;
        }

        public void Quake(float amount)
        {
            Shake = Mathf.Max(Shake, amount);
        }

        public void ApplyTilt()
        {
            if (Mathf.Approximately(Tilt, 0f))
            {
                return;
            }

            Canvas.RotateAbout(new Vector2(WallL + BaseX, Ground + BaseY), Tilt);
        }

        public float Random()
        {
            return _jitter.Next();
        }

        public bool IsEvery(int frames)
        {
            return Mathf.FloorToInt(ElapsedFrames / frames) != Mathf.FloorToInt((ElapsedFrames - StepFrames) / frames);
        }
    }
}
