using System.IO;
using Newtonsoft.Json;

namespace Common
{
    public class JsonObject<T>
    {
        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }

        public static T Load(string path)
        {
            if (!File.Exists(path)) return default(T);
            
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}