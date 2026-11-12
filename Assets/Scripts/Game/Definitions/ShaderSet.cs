using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    // コードで作る材質のシェーダー。名前で探すとビルドで外されるので、参照で持ってビルドに入れる
    [Serializable]
    public sealed class ShaderSet
    {
        // Sprites/Default だと URP のまとめ描きで別の画像が描かれた
        [SerializeField] private Shader _sprite;
        [SerializeField] private Shader _additive;
        [SerializeField] private Shader _flash;
        [SerializeField] private Shader _vertexColor;
        [SerializeField] private Shader _textSprite;

        public Shader Sprite => _sprite;
        public Shader Additive => _additive;
        public Shader Flash => _flash;
        public Shader VertexColor => _vertexColor;
        public Shader TextSprite => _textSprite;
    }
}
