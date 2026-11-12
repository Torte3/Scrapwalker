using Newtonsoft.Json;

namespace Scrapwalker.Core.Save
{
    public static class SaveWriter
    {
        internal static string Write(SaveFile file)
        {
            return JsonConvert.SerializeObject(file, SaveJSON.Settings);
        }
    }
}
