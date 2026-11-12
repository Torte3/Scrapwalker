using System;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;

namespace Scrapwalker.Game.Wording
{
    public static class RejectionText
    {
        public static string Text(this CommandResult result, GameData data)
        {
            switch (result.Error)
            {
                case CommandError.OverLimit:
                    return OverLimit(data.NameOf(data.Definitions.ByID<FacilityDefinition>(result.SubjectID)));
                case CommandError.PreviousNotDone:
                    return PreviousNotDone(data.NameOf(data.Definitions.FindRecipe(result.SubjectID)));
                case CommandError.Shortage:
                    return Shortage(result.Shortage.Shortage(data));
                default:
                    return Fixed(result.Error);
            }
        }

        private static string Fixed(CommandError error)
        {
            return error switch
            {
                CommandError.None => string.Empty,
                CommandError.GameOver => "ゲームは終了している",
                CommandError.NoFacility => "設備が存在しない",
                CommandError.OutOfGrid => "マスの外",
                CommandError.Overlaps => "他の設備と重なる",
                CommandError.OverlapsRelocation => "移設先と重なる",
                CommandError.BadRange => "下限が上限より高い",
                CommandError.NotBuilt => "完成していない",
                CommandError.Locked => "研究が済んでいない",
                CommandError.BeingDismantled => "解体中",
                CommandError.AlreadyDone => "済んでいる",
                CommandError.AlreadyQueued => "もうタスクに入っている",
                CommandError.NoSuchRecipe => "そのレシピは無い",
                CommandError.NeedsUpgrade => "改造が済んでいない",
                CommandError.BadCount => "回数は 1 以上",
                CommandError.NoSuchTask => "そのタスクは無い",
                CommandError.CannotMoveFurther => "これ以上は動かせない",
                CommandError.OnceOnly => "1 回きりの作業",
                CommandError.NotPlanned => "建設予約ではない",
                CommandError.NotDismantling => "解体中ではない",
                CommandError.AlreadyDismantling => "すでに解体中",
                CommandError.CancelPlanInstead => "建設予約はキャンセルする",
                CommandError.DismantleFirst => "解体が終わってから",
                CommandError.NoStation => "ボットの持ち場が無い部屋",
                CommandError.NotLanded => "接地していない",
                CommandError.CannotChooseDestination => "移動中でなければ行先を選べない",
                CommandError.AlreadyHeading => "すでに行先へ向かっている",
                CommandError.NoDestination => "取り消す行き先が無い",
                CommandError.NoSuchRuin => "廃墟が存在しない",
                CommandError.Unreachable => "届かない",
                CommandError.NoSuchResearch => "その研究は無い",
                CommandError.ResearchDone => "研究は済んでいる",
                CommandError.ResearchLocked => "前提の研究が済んでいない",
                CommandError.ResearchBusy => "研究は1つずつ",
                CommandError.NotResearching => "研究中の研究が無い",
                CommandError.Refused => string.Empty,
                CommandError.NotCleared => "まだ完走していない",
                CommandError.BotLimit => "ボットの数が上限に達している",
                CommandError.TakingOff => "離陸している",
                CommandError.RepairFirst => "修理が終わってから",
                CommandError.NoSuchEnemy => "その敵はいない",
                CommandError.NoLockOn => "狙いを付けていない",
                _ => throw new ArgumentOutOfRangeException(nameof(error), error, "断る理由の文が無い"),
            };
        }

        private static string OverLimit(string facility)
        {
            return $"{facility}の建築可能上限を超えています";
        }

        private static string PreviousNotDone(string name)
        {
            return $"「{name}」が実行されていない";
        }

        private static string Shortage(string items)
        {
            return $"資材が足りない：{items}";
        }
    }
}
