using UnityEngine;

namespace Scrapwalker.Game.MainView.Compose
{
    public struct FacilityCellWalk
    {
        private readonly FacilityRect _rect;
        private int _index;

        public FacilityCellWalk(FacilityRect rect)
        {
            _rect = rect;
            _index = -1;
        }

        public Vector2Int Current => new Vector2Int(_rect.Column + _index % _rect.Width, _rect.Row + _index / _rect.Width);

        public FacilityCellWalk GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _rect.Width * _rect.Height;
        }
    }
}
