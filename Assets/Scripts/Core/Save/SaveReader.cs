using Newtonsoft.Json;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Save
{
    public static class SaveReader
    {
        public static bool TryRead(string text, GameDefinitions definitions, out World world, out LoadError error)
        {
            world = null;
            if (!TryParse(text, out SaveFile file))
            {
                error = new LoadError(LoadErrorKind.NotSaveFormat);
                return false;
            }

            var loaded = new World(definitions);
            try
            {
                loaded.Restore(file.World, new LoadContext(definitions.ByID<Definition>));
            }
            catch (LoadRefused refused)
            {
                error = refused.Error;
                return false;
            }

            world = loaded;
            error = default;
            return true;
        }

        public static bool TryParse(string text, out SaveFile file)
        {
            file = null;
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            try
            {
                file = JsonConvert.DeserializeObject<SaveFile>(text, SaveJSON.Settings);
            }
            catch (JsonException)
            {
                return false;
            }

            return file?.World != null;
        }
    }
}
