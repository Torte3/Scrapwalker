using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Outpost.Roles;

namespace Scrapwalker.Game.Wording.Words
{
    public static class EffectWords
    {
        public const string Bot = "ボットが1体増える";

        public static string Note(EffectNote note)
        {
            return note switch
            {
                EffectNote.ShotArea => "当たった所の周りの敵にまとめて当たる",
                EffectNote.ShotKnockback => "当たった敵を押し戻す",
                EffectNote.RuinsFound => "新しい廃墟を見つけることができる",
                EffectNote.EscapeSequence => "この星からの脱出シーケンスを実行できる",
                _ => throw new System.ArgumentOutOfRangeException(nameof(note), note, null),
            };
        }

        public static string Change(FacilityStat stat, float from, float to)
        {
            return stat switch
            {
                FacilityStat.StorageCapacity => $"容量 {(int)from} → {(int)to}",
                FacilityStat.EnergyCapacity => $"容量 {Texts.Decimal(from)} → {Texts.Decimal(to)}",
                FacilityStat.HullBonus => $"外壁の最大 +{(int)from} → +{(int)to}",
                FacilityStat.BarrierCapacity => $"バリアの耐久値 {Texts.Decimal(from)} → {Texts.Decimal(to)}",
                _ => throw new System.ArgumentOutOfRangeException(nameof(stat), stat, null),
            };
        }

        public static string Unlock(string name)
        {
            return $"「{name}」を実行できるようになる";
        }

        public static string UnlockAfterResearch(string research)
        {
            return $"研究「{research}」の後";
        }

        public static string Power(string shot, string from, string to)
        {
            return $"{shot}の威力 {from} → {to}";
        }

        public static string Slow(string radius, string seconds, int percent)
        {
            return $"当たった所から半径 {radius} の敵の歩きと攻撃を\n{seconds} 秒、{percent}% の速さにする";
        }

        public static string Shot(string damage)
        {
            return $"威力 {damage} の弾を撃つ";
        }

        public static string ShotRounds(string damage, int rounds)
        {
            return $"威力 {damage} の弾を {rounds} 発撃つ";
        }

        public static string ShotPierce(int count)
        {
            return $"奥へ貫いて {count} 体まで当たる";
        }

        public static string ShotArmorPiercing(string armor)
        {
            return $"装甲を {armor} まで貫く";
        }

        public static string BotLimit(int from, int to)
        {
            return $"ボットの上限 {from} → {to}";
        }

        public static string BotCount(int now, int limit)
        {
            return $"いま {now} 体（上限 {limit}）";
        }
    }
}
