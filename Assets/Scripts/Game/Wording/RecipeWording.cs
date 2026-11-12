using UnityEngine;

namespace Scrapwalker.Game.Wording
{
    [System.Serializable]
    public sealed class RecipeWording
    {
        [SerializeField] private string _recipe = string.Empty;
        [SerializeField] private string _name;

        public string Recipe => _recipe;
        public string Name => _name;
    }
}
