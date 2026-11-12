using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class ScreenArt
    {
        [SerializeField] private Sprite _titleLogo;
        [SerializeField] private Sprite _titleVignette;
        [SerializeField] private Sprite _energy;

        public Sprite TitleLogo => _titleLogo;
        public Sprite TitleVignette => _titleVignette;
        public Sprite Energy => _energy;
    }
}
