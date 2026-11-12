using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class SpriteBatch : MonoBehaviour
    {
        private RendererPool _pool;
        private CanvasStack _canvas;
        private DrawOrder _order;
        private Material _normal;
        private Material _additive;
        private Material _flash;

        public void Initialize(CanvasStack canvas, DrawOrder order, ShaderSet shaders)
        {
            _canvas = canvas;
            _order = order;
            _pool = new RendererPool(transform, "Sprite");
            _normal = new Material(shaders.Sprite);
            _additive = new Material(shaders.Additive);
            _flash = new Material(shaders.Flash);
        }

        public void Begin()
        {
            _pool.Begin();
        }

        public void Draw(ArtLayer layer, ArtFrame frame, DrawBand band, Color color)
        {
            Place(frame?.Sprite, band, color, layer.IsAdditive ? _additive : _normal);
        }

        public void DrawFlash(ArtFrame frame, DrawBand band, Color tint)
        {
            Place(frame?.Sprite, band, tint, _flash);
        }

        private void Place(Sprite sprite, DrawBand band, Color color, Material material)
        {
            if (sprite == null)
            {
                return;
            }

            SpriteRenderer spriteRenderer = _pool.Next();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sharedMaterial = material;
            color.a *= _canvas.Alpha;
            spriteRenderer.color = color;
            spriteRenderer.sortingLayerID = band.LayerID();
            spriteRenderer.sortingOrder = _order.Next(band);
            CanvasTransform canvasTransform = _canvas.Transform;
            Vector2 at = canvasTransform.Map(0f, 0f);
            float scaleX = canvasTransform.ScaleX;
            Transform rendererTransform = spriteRenderer.transform;
            rendererTransform.localPosition = new Vector3(at.x, at.y, 0f);
            rendererTransform.localRotation = Quaternion.Euler(0f, 0f, canvasTransform.Angle * Mathf.Rad2Deg);
            rendererTransform.localScale = new Vector3(scaleX, -canvasTransform.Determinant / scaleX, 1f);
        }

        public void End()
        {
            _pool.End();
        }

        private void OnDestroy()
        {
            if (_normal != null)
            {
                Destroy(_normal);
            }

            if (_additive != null)
            {
                Destroy(_additive);
            }

            if (_flash != null)
            {
                Destroy(_flash);
            }
        }
    }
}
