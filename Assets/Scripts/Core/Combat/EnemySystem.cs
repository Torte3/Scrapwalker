using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Outpost.Hulls;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat
{
    internal static class EnemySystem
    {
        internal static void Tick(World world)
        {
            if (!world.InCombat)
            {
                return;
            }

            float deltaTime = world.Configuration.TickSeconds;
            foreach (Enemy enemy in world.Battle.Enemies)
            {
                if (enemy.IsAlive)
                {
                    TickEnemy(world, enemy, deltaTime);
                }
            }

            RemoveDead(world);
        }

        private static void TickEnemy(World world, Enemy enemy, float deltaTime)
        {
            enemy.Recover(world.Configuration);
            enemy.Walk(deltaTime);

            if (!enemy.HasBegunAttacking && enemy.IsInPosition)
            {
                Arrive(world, enemy);
                return;
            }

            if (enemy.HasBegunAttacking && enemy.Definition.HasAttacks)
            {
                TickMoves(world, enemy);
            }

            if (enemy.HasBegunAttacking)
            {
                enemy.Act(world);
            }
        }

        private static void RemoveDead(World world)
        {
            foreach (Enemy enemy in world.Battle.Enemies)
            {
                if (!enemy.IsAlive)
                {
                    world.Emit(new EnemyKilled(enemy));
                }
            }

            world.Battle.RemoveDeadEnemies();
        }

        private static void Arrive(World world, Enemy enemy)
        {
            enemy.Distance = System.Math.Max(0f, enemy.Definition.Type.StopDistance);
            enemy.HasBegunAttacking = true;
            enemy.Arrive();
            if (!enemy.Definition.HasAttacks)
            {
                return;
            }

            enemy.Attack.Restart(enemy.Definition.NextMove(), 0);
            PickTargets(world, enemy);
        }

        private static void PickTargets(World world, Enemy enemy)
        {
            AttackMove move = enemy.Definition.AttackMoves[enemy.Attack.Index];
            var cells = new Cell[move.ShotCount];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = move.HasOneTarget && i > 0 ? cells[0] : RandomCell(world);
            }

            enemy.Attack.SetTargets(cells);
            world.Emit(new AttackBegun(enemy, move));
        }

        private static Cell RandomCell(World world)
        {
            int index = GameRandom.Range(0, world.Columns * world.Rows);
            return new Cell(index % world.Columns, index / world.Columns);
        }

        internal static void Strike(World world, Enemy enemy, AttackMove move, Cell hit)
        {
            enemy.StrikeCount++;
            int reached = OutpostDamage.Take(world, move.Damage, hit);
            world.Emit(new CellAttacked(enemy, move, hit, move.Damage - reached, reached));
        }

        private static void TickMoves(World world, Enemy enemy)
        {
            AttackRoutine attack = enemy.Attack;
            if (!attack.RestTime.IsDone)
            {
                if (!attack.RestTime.Advance(enemy.Pace))
                {
                    return;
                }

                PickTargets(world, enemy);
            }

            float tickSeconds = world.Configuration.TickSeconds;
            AttackMove move = enemy.Definition.AttackMoves[attack.Index];
            for (int shot = attack.Advance(move, tickSeconds); shot < attack.Fired; shot++)
            {
                Strike(world, enemy, move, attack.Cells[shot]);
            }

            if (attack.IsDone(move, tickSeconds))
            {
                attack.Restart(enemy.Definition.NextMove(), world.Configuration.TicksAtLeastOne(move.RestSeconds));
            }
        }
    }
}
