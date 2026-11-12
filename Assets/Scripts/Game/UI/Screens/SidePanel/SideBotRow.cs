using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.SidePanel
{
    internal sealed class SideBotRow
    {
        private const string BatteryVariant = "side-bot-row__battery-fill--";

        private readonly Label _id;
        private readonly Label _place;
        private readonly VisualElement _batteryFill;
        private readonly Label _job;
        private readonly VisualElement _heading;
        private readonly VisualElement _lost;
        private readonly Label _cargo;
        private readonly VisualElement _bar;
        private readonly JobBar _progress;
        private readonly ShownValue<int> _idShown = new ShownValue<int>();
        private readonly ShownValue<BotDoing> _jobShown = new ShownValue<BotDoing>();
        private readonly ShownValue<(float, string)> _batteryShown = new ShownValue<(float, string)>();
        private readonly ShownValue<bool> _headingShown = new ShownValue<bool>();
        private readonly ShownValue<bool> _lostShown = new ShownValue<bool>();
        private readonly ShownValue<ItemStack> _cargoShown = new ShownValue<ItemStack>();
        private readonly ShownValue<bool> _barShown = new ShownValue<bool>();

        public SideBotRow(VisualElement element)
        {
            Element = element;
            _id = element.Need<Label>("id");
            _place = element.Need<Label>("place");
            _batteryFill = element.Need<VisualElement>("battery-fill");
            _job = element.Need<Label>("job");
            _heading = element.Need<VisualElement>("heading");
            _lost = element.Need<VisualElement>("lost");
            element.WriteLabel("heading", SideWords.Heading);
            element.WriteLabel("lost", SideWords.Lost);
            _cargo = element.Need<Label>("cargo");
            _bar = element.Need<VisualElement>("progress");
            _progress = new JobBar(_bar);
        }

        public VisualElement Element { get; }

        public void Refresh(GameData data, World world, Bot bot)
        {
            if (_idShown.Change(bot.ID))
            {
                _id.text = bot.Label();
                _jobShown.Reset();
            }

            BotDoing job = BotDoingReader.Read(bot);
            if (_jobShown.Change(job))
            {
                _place.text = job.Verb.IsIdle ? string.Empty : job.Place.Show(world, new PlaceName(data, world));
                _job.text = job.Verb.Text;
                _job.EnableInClassList("side-bot-row__job--idle", job.Verb.IsIdle);
            }

            RefreshBattery(world, bot, job.Verb.ShowsCharging && !bot.IsHeading);

            bool isHeading = bot.IsHeading;
            if (_headingShown.Change(isHeading))
            {
                _heading.Display(isHeading);
            }

            if (_lostShown.Change(bot.IsLost))
            {
                _lost.Display(bot.IsLost);
            }

            if (_cargoShown.Change(bot.Cargo))
            {
                _cargo.text = bot.IsCarrying ? SideWords.Cargo(bot.Cargo.Type.Name(data), bot.Cargo.Count) : string.Empty;
            }

            BotProgress progress = job.Progress;
            if (_barShown.Change(progress.HasProgress))
            {
                _bar.Show(progress.HasProgress);
            }

            if (progress.HasProgress)
            {
                _progress.Set(progress.Ready, progress.Value, true);
            }
        }

        private void RefreshBattery(World world, Bot bot, bool isCharging)
        {
            float fill = bot.BatteryFill(world.Configuration);
            string variant = isCharging ? "charging" : fill > 0.5f ? "full" : bot.NeedsCharge(world.Configuration) ? "low" : "half";
            if (_batteryShown.Change((fill, variant)))
            {
                _batteryFill.style.width = Length.Percent(fill * 100f);
                _batteryFill.SetVariant(BatteryVariant, variant);
            }
        }
    }
}
