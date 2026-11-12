using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Audio;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Wording;
using TMPro;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Scrapwalker/Game Data")]
    public sealed class GameData : ScriptableObject
    {
        [SerializeField] private GameConfiguration _configuration = new GameConfiguration();
        [SerializeField] private TutorialConfiguration _tutorial = new TutorialConfiguration();
        [SerializeField] private List<MaterialAsset> _materials = new List<MaterialAsset>();
        [SerializeField] private List<FacilityAsset> _facilities = new List<FacilityAsset>();
        [SerializeField] private FacilityAsset _storage;
        [SerializeField] private List<EnemyTypeAsset> _enemyTypes = new List<EnemyTypeAsset>();
        [SerializeField] private List<EnemyAsset> _enemies = new List<EnemyAsset>();
        [SerializeField] private List<RuinAsset> _ruins = new List<RuinAsset>();
        [SerializeField] private List<ResearchAsset> _research = new List<ResearchAsset>();
        [SerializeField] private List<ItemTableAsset> _tables = new List<ItemTableAsset>();
        [SerializeField] private List<OutpostUpgradeAsset> _upgrades = new List<OutpostUpgradeAsset>();
        [SerializeField] private OutpostUpgradeAsset _radarRepair;
        [SerializeField] private List<OutpostUpgradeAsset> _radarLooks = new List<OutpostUpgradeAsset>();
        [SerializeField] private EnemyAsset _decorationEnemy;
        [SerializeField] private List<TimeOfDayLook> _timesOfDay = new List<TimeOfDayLook>();
        [SerializeField] private ShaderSet _shaders = new ShaderSet();
        [SerializeField] private ArtCatalog _art;
        [SerializeField] private SoundTable _sounds;
        [SerializeField] private ParticleSet _particles = new ParticleSet();
        [SerializeField] private TMP_FontAsset _worldFont;
        [SerializeField] private int _energyTextSprite;

        private Catalog _catalog;

        public GameDefinitions Definitions => EnsureBuilt().Definitions;
        public TutorialConfiguration Tutorial
        {
            get
            {
                EnsureBuilt();
                return _tutorial;
            }
        }

        public int RadarLookOf(World world)
        {
            for (int look = _radarLooks.Count; look > 0; look--)
            {
                if (world.Upgrades.IsDone(_radarLooks[look - 1].Value))
                {
                    return look;
                }
            }

            return 0;
        }

        public ShaderSet Shaders => _shaders;
        public ArtCatalog Art => _art;
        public SoundTable Sounds => _sounds;
        public ParticleSet Particles => _particles;
        public TMP_FontAsset WorldFont => _worldFont;
        public int EnergyTextSprite => _energyTextSprite;

        public string StorageName => _storage.Name;

        public EnemyDefinition DecorationEnemy => _decorationEnemy != null ? _decorationEnemy.Value : null;

        public TimeOfDayLook LookOf(TimeOfDay time)
        {
            return _timesOfDay[(int)time];
        }

        public MaterialAsset Asset(MaterialDefinition definition)
        {
            return Find<MaterialAsset>(definition);
        }

        public FacilityAsset Asset(FacilityDefinition definition)
        {
            return Find<FacilityAsset>(definition);
        }

        public EnemyTypeAsset Asset(EnemyTypeDefinition definition)
        {
            return Find<EnemyTypeAsset>(definition);
        }

        public AttackStyleAsset Asset(AttackStyleDefinition definition)
        {
            return Find<AttackStyleAsset>(definition);
        }

        public EnemyAsset Asset(EnemyDefinition definition)
        {
            return Find<EnemyAsset>(definition);
        }

        public RuinAsset Asset(RuinDefinition definition)
        {
            return Find<RuinAsset>(definition);
        }

        public ResearchAsset Asset(ResearchDefinition definition)
        {
            return Find<ResearchAsset>(definition);
        }

        public string NameOf(Definition definition)
        {
            return Find<DefinitionAsset>(definition).Name;
        }

        public string NameOf(RecipeDefinition recipe)
        {
            return EnsureBuilt().Recipes.TryGetValue(recipe.ID, out RecipeWording wording) ? wording.Name : throw new KeyNotFoundException("作業の名前が無い: " + recipe.ID);
        }

        public bool TryNameOf(RecipeDefinition recipe, out string name)
        {
            name = EnsureBuilt().Recipes.TryGetValue(recipe.ID, out RecipeWording wording) ? wording.Name : null;
            return !string.IsNullOrEmpty(name);
        }

        private T Find<T>(Definition definition) where T : DefinitionAsset
        {
            return EnsureBuilt().Assets.TryGetValue(definition, out DefinitionAsset asset) ? (T)asset : throw new KeyNotFoundException("定義のアセットが無い: " + definition);
        }

        private IEnumerable<AttackStyleAsset> Styles => _enemyTypes.Where(type => type != null).SelectMany(type => type.Styles);

        private Catalog EnsureBuilt()
        {
            if (_catalog == null)
            {
                _catalog = Build();
            }

            return _catalog;
        }

        private Catalog Build()
        {
            var catalog = new Catalog(new GameDefinitions { Configuration = _configuration });
            GameDefinitions definitions = catalog.Definitions;
            Add(catalog, definitions.Materials, _materials);
            Add(catalog, definitions.Tables, _tables);
            Add(catalog, definitions.Research, _research);
            Add(catalog, definitions.Facilities, _facilities);
            Add(catalog, definitions.AttackStyles, Styles.ToList());
            Add(catalog, definitions.EnemyTypes, _enemyTypes);
            Add(catalog, definitions.Enemies, _enemies);
            Add(catalog, definitions.Ruins, _ruins);
            Add(catalog, definitions.Upgrades, _upgrades);
            definitions.Link(_tutorial);
            _tutorial.RadarRepair = _radarRepair != null ? _radarRepair.Value : null;
            foreach (FacilityAsset facility in _facilities.Where(facility => facility != null))
            {
                foreach (RecipeWording wording in facility.Recipes)
                {
                    catalog.Recipes[wording.Recipe] = wording;
                }
            }

            return catalog;
        }

        private void Add<TDefinition, TAsset>(Catalog catalog, List<TDefinition> into, List<TAsset> assets) where TDefinition : Definition, new() where TAsset : DefinitionAsset<TDefinition>
        {
            foreach (TAsset asset in assets)
            {
                if (asset == null)
                {
                    Debug.LogError(name + " の一覧に空の欄がある（" + typeof(TAsset).Name + "）");
                    continue;
                }

                asset.Value.Key = asset.name;
                into.Add(asset.Value);
                catalog.Assets[asset.Value] = asset;
            }
        }

#if UNITY_EDITOR
        public static GameData InProject()
        {
            string[] found = UnityEditor.AssetDatabase.FindAssets("t:" + nameof(GameData));
            if (found.Length != 1)
            {
                throw new System.InvalidOperationException("GameData のアセットが " + found.Length + " 個ある（1 個のはず）");
            }

            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameData>(UnityEditor.AssetDatabase.GUIDToAssetPath(found[0]));
        }

        public IEnumerable<DefinitionAsset> DefinitionAssets => _materials.Cast<DefinitionAsset>().Concat(_facilities).Concat(Styles).Concat(_enemyTypes).Concat(_enemies).Concat(_ruins).Concat(_research).Concat(_tables).Concat(_upgrades).Where(asset => asset != null);

        private void OnValidate()
        {
            _catalog = null;
        }

        public void ForgetDefinitions()
        {
            _catalog = null;
        }
#endif

        private sealed class Catalog
        {
            public Catalog(GameDefinitions definitions)
            {
                Definitions = definitions;
            }

            public GameDefinitions Definitions { get; }
            public Dictionary<Definition, DefinitionAsset> Assets { get; } = new Dictionary<Definition, DefinitionAsset>();
            public Dictionary<string, RecipeWording> Recipes { get; } = new Dictionary<string, RecipeWording>();
        }
    }
}
