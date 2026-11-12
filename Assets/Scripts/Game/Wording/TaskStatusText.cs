using System;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Game.UI;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.Wording
{
    public static class TaskStatusText
    {
        public static string RoomText(this TaskStatus status, int percent)
        {
            return status switch
            {
                TaskStatus.Upgrading => $"● 改造中 {percent}%",
                TaskStatus.Operating => "● 稼働中",
                TaskStatus.Advancing => "● 作業が進んでいる",
                TaskStatus.NoTasks => "● タスクが無い",
                TaskStatus.Paused => "● ボットを待っている",
                TaskStatus.AwaitingBot => "● ボットを待っている",
                TaskStatus.Ready => "● 待機中",
                _ => StopText(status),
            };
        }

        public static string RowText(this TaskStatus status)
        {
            return status switch
            {
                TaskStatus.Advancing => "実行中",
                TaskStatus.Paused => "実行中",
                TaskStatus.Ready => "順番待ち",
                TaskStatus.AwaitingBot => "順番待ち",
                _ => StopText(status),
            };
        }

        public static void Show(this Label label, TaskStatus status)
        {
            label.SetText(status.RowText());
            label.SetSeverity(status.SeverityOf());
        }

        public static Severity SeverityOf(this TaskStatus status)
        {
            if (!status.IsStop())
            {
                return Severity.Normal;
            }

            return status == TaskStatus.HaulingMaterials ? Severity.Caution : Severity.Warning;
        }

        private static string StopText(TaskStatus status)
        {
            return status switch
            {
                TaskStatus.Materials => "資材が足りない",
                TaskStatus.HaulingMaterials => "資材の運び込みを待っている",
                TaskStatus.NoEnergyRoom => "蓄電に空きがない",
                TaskStatus.NoFreeBot => "ボットが空いていない",
                TaskStatus.NotEnoughEnergy => "蓄電が足りない",
                TaskStatus.NoEnemyInRange => "射程に敵がいない",
                TaskStatus.NotInCombat => "戦闘中ではない",
                TaskStatus.BotLimit => "ボットの数が上限に達している",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, "タスクの様子の文が無い"),
            };
        }
    }
}
