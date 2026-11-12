using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Wording;

namespace Scrapwalker.Game.Look
{
    public class BotVerb
    {
        public static readonly BotVerb None = new BotVerb("待機", "作っている", true, false);
        public static readonly BotVerb Haul = new HaulVerb();
        public static readonly BotVerb Tidy = new BotVerb("資材を片付ける", "作っている");
        public static readonly BotVerb Fire = new BotVerb("撃つ", "撃っている");
        public static readonly BotVerb Make = new BotVerb("作る", "作っている");
        public static readonly BotVerb Wait = new BotVerb("待機", "持ち場で待っている");
        public static readonly BotVerb Build = new BotVerb("建てる", "作っている");
        public static readonly BotVerb Rebuild = new BotVerb("組み直す", "作っている");
        public static readonly BotVerb Repair = new BotVerb("直す", "作っている");
        public static readonly BotVerb Dismantle = new BotVerb("解体する", "作っている");
        public static readonly BotVerb Charge = new BotVerb("充電する", "作っている", false, true);

        private readonly string _doing;

        private BotVerb(string text, string doing, bool isIdle = false, bool showsCharging = false)
        {
            Text = text;
            _doing = doing;
            IsIdle = isIdle;
            ShowsCharging = showsCharging;
        }

        public string Text { get; }
        public bool IsIdle { get; }
        public bool ShowsCharging { get; }

        public virtual string Sentence(BotDoing doing, Bot bot, GameData data)
        {
            string label = bot.Label();
            return bot.IsHeading ? $"{label} が持ち場へ向かっている" : $"{label} が{_doing}";
        }

        private sealed class HaulVerb : BotVerb
        {
            public HaulVerb()
                : base("資材を運ぶ", "作っている")
            {
            }

            public override string Sentence(BotDoing doing, Bot bot, GameData data)
            {
                string label = bot.Label();
                string item = (bot.IsCarrying ? bot.Cargo.Type : doing.Material).Name(data);
                return bot.IsCarrying ? $"{label} が{item}を運んでいる" : $"{label} が{item}を取りに向かっている";
            }
        }
    }
}
