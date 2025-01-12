using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common
{
    public class JsonObject<T> where T : class
    {
        [JsonIgnore]
        private T _originalValue;

        public bool HasNoChanges()
        {
            if (_originalValue == null)
                return true;

            var currentOriginal = _originalValue;
            _originalValue = null;
            
            try
            {
                var currentToken = JToken.FromObject(this);
                var originalToken = JToken.FromObject(currentOriginal);
                return JToken.DeepEquals(currentToken, originalToken);
            }
            finally
            {
                _originalValue = currentOriginal;
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static T FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            _originalValue = (T)this.MemberwiseClone();
        }

        public static T Load(string path)
        {
            if (!File.Exists(path)) return default(T);
            var obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            if (obj is JsonObject<T> jsonObj)
            {
                jsonObj._originalValue = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            return obj;
        }
    }
}