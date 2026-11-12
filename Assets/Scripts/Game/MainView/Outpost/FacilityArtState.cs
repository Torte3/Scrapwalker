using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using Scrapwalker.Game.MainView.Compose;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Outpost
{
    internal sealed class FacilityArtState : ArtState
    {
        private FacilityVisual _facility;
        private StationBot? _bot;
        private bool _isActing;
        private int _stored;
        private float _clock;

        public override float Clock => _clock;
        public override bool IsActing => _isActing;
        public override bool IsCharging => _facility.IsRecovering;
        public override bool IsFeeding => _bot.HasValue;

        public void Begin(in FacilityVisual facility, bool isWorking, StationBot? bot, float clock)
        {
            _facility = facility;
            _bot = bot;
            _clock = clock;
            _isActing = facility.Look.Act.IsActing(facility.Facility, isWorking, facility.HasChargingBot);
            _stored = facility.Look.Gauge.CountStored(facility.Facility);
        }

        public override bool IsShown(ArtLayer layer)
        {
            return layer == _facility.Look.BotLayer ? _bot.HasValue : base.IsShown(layer);
        }

        public override ArtFrame FrameOf(ArtLayer layer)
        {
            return layer.IsLoop ? layer.At(_clock * LoopRate(layer)) : base.FrameOf(layer);
        }

        public override Color TintOf(ArtLayer layer)
        {
            return layer == _facility.Look.BotLayer && _bot.HasValue ? _bot.Value.Body.Tint : Color.white;
        }

        public override float ValueOf(ArtLayer layer)
        {
            return _facility.Look.Gauge.Step(_stored, _facility.GaugeFill, _isActing);
        }

        private float LoopRate(ArtLayer layer)
        {
            ArtCalm calm = layer.Find<ArtCalm>();
            return calm != null && !IsCharging ? calm.Rate : 1f;
        }
    }
}
