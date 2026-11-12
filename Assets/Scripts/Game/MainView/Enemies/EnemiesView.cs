using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class EnemiesView
    {
        private readonly WorldScene _scene;
        private readonly DropPainter _drops;

        public EnemiesView(WorldScene scene, ArtParts parts, ArtPlayer player, ArcMesh arcs, EffectsPainter effects, WorldUI ui, GameData data)
        {
            _scene = scene;
            _drops = new DropPainter(scene, parts, data);
            Enemies = new EnemyPainter(scene, new EnemyLooks(data), parts, player, arcs, effects, _drops, ui, data.DecorationEnemy);
            Shots = new ShotPainter(scene, effects, parts);
        }

        public EnemyPainter Enemies { get; }

        public ShotPainter Shots { get; }

        public void Clear()
        {
            _drops.Clear();
            Enemies.Clear();
        }

        public void Strike(CellAttacked attacked)
        {
            Enemies.Strike(attacked.EnemyID, _scene.CellCenter(attacked.Cell), Shots);
        }

        public void LandShot(ShotLanded landed, ShotLook look)
        {
            Shots.Land(landed, look);
            foreach (EnemyImpact impact in landed.Impacts)
            {
                Enemies.ShowImpact(impact);
            }
        }

        public void Step(FrameVisual frame, float deltaFrames, float realDelta)
        {
            World world = frame.World;
            Enemies.Step(world.Battle, world.Configuration.TickSeconds, deltaFrames, realDelta);
            Shots.RemoveVanished(world.Battle.Projectiles);
            Shots.Step(deltaFrames);
            _drops.Step(deltaFrames, IntakeRowOf(world));
        }

        private static int IntakeRowOf(World world)
        {
            return (int)world.Intake.Y;
        }

        public void Draw(FrameVisual frame)
        {
            World world = frame.World;
            Enemies.DrawAlive(world.Battle.Enemies);
            _drops.DrawBelow(IntakeRowOf(world));
            Enemies.DrawHealFronts(world.Battle.Enemies);
            _drops.DrawAbove(IntakeRowOf(world));
            Shots.Draw();
        }
    }
}
