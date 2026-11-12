using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.MainView.Bots;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class OutpostView
    {
        private readonly FacilityTagPainter _facilityTags;
        private readonly PilePainter _piles;
        private readonly CrackPainter _cracks;

        public OutpostView(WorldScene scene, ArtParts parts, ArtPlayer player, EffectsPainter effects, WorldUI ui, WorldUI labelUI, GameData data)
        {
            Build = new BuildOverlayPainter(scene, ui, data);
            Hull = new HullPainter(scene, parts, player);
            var botSprites = new BotSprites(scene, parts, player);
            var icons = new ItemIcons(scene, parts, data);
            Facilities = new FacilityPainter(scene, parts, new FacilitySprites(scene, parts, player), botSprites, effects);
            _facilityTags = new FacilityTagPainter(scene, ui, data);
            _piles = new PilePainter(scene, icons, ui);
            _cracks = new CrackPainter(scene, parts);
            Bots = new BotPainter(scene, botSprites, icons, labelUI);
        }

        public HullPainter Hull { get; }

        public FacilityPainter Facilities { get; }

        public BotPainter Bots { get; }

        public BuildOverlayPainter Build { get; }

        public void Clear()
        {
            Bots.Clear();
            Facilities.Clear();
        }

        public void Step(FrameVisual frame, float deltaFrames)
        {
            Facilities.Step(deltaFrames);
            Bots.Step(frame.Bots, frame.World.Configuration);
        }

        public void DrawFacilities(FrameVisual frame, bool showsQuality)
        {
            Facilities.Draw(frame);
            _facilityTags.DrawRelocateTargets(frame.Facilities);
            _cracks.Draw(frame.World.Hull.Cracks);
            _piles.Draw(frame.World.Floor.Piles);
            Bots.Draw(frame.Bots);
            _facilityTags.Draw(frame.Facilities, showsQuality);
        }
    }
}
