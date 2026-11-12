using Scrapwalker.Game.Look.Art.Sets;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art
{
    public sealed class ArtCatalog : ScriptableObject
    {
        [SerializeField] private ArtEntry[] _entries;
        [SerializeField] private EffectArt _effects = new EffectArt();
        [SerializeField] private BackdropArt _backdrop = new BackdropArt();
        [SerializeField] private OutpostArt _outpost = new OutpostArt();
        [SerializeField] private ArmArt _arm = new ArmArt();
        [SerializeField] private LegArt _farLeg = new LegArt();
        [SerializeField] private LegArt _nearLeg = new LegArt();
        [SerializeField] private BotArt _bots = new BotArt();
        [SerializeField] private TakeoffArt _takeoff = new TakeoffArt();
        [SerializeField] private ScreenArt _screen = new ScreenArt();

        public ArtEntry[] Entries => _entries;
        public EffectArt Effects => _effects;
        public BackdropArt Backdrop => _backdrop;
        public OutpostArt Outpost => _outpost;
        public ArmArt Arm => _arm;
        public LegArt FarLeg => _farLeg;
        public LegArt NearLeg => _nearLeg;
        public BotArt Bots => _bots;
        public TakeoffArt Takeoff => _takeoff;
        public ScreenArt Screen => _screen;

#if UNITY_EDITOR
        public void Fill(ArtEntry[] entries)
        {
            _entries = entries;
        }
#endif
    }
}
