using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;

namespace MyDB4.Models
{
    public static class Repository
    {
        public static string ObjectToJson<T>(this T obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

        private static T JsonToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static void CompressStringToFile(this string s, string filename)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            using (MemoryStream ms = new MemoryStream(byteArray))
            using (var fs = File.Create(filename))

            using (var gz = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Compress))
            {
                ms.WriteTo(gz);
            }
        }

        public static string DecompressFileToString(string filename)
        {
            using (MemoryStream ms = new MemoryStream())
            using (var fs = File.Open(filename, FileMode.Open))
            using (var gz = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
            {
                gz.CopyTo(ms);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static T LoadObjectFromStorage<T>(string storage_file) where T : class, new()
        {
            string json;
            if (!File.Exists(storage_file))
            {
                T obj = new T();
                json = obj.ObjectToJson();
                CompressStringToFile(json, storage_file);
            }
            json = DecompressFileToString(storage_file);
            return json.JsonToObject<T>();
        }

        public static void SaveObjectToStorage<T>(this T obj, string storage_file) where T : class, new()
        {
            string json = obj.ObjectToJson();
            json.CompressStringToFile(storage_file);
        }

        public static AppSettings LoadAppSettings(string settings_file = "settings.json")
        {
            if (!File.Exists(settings_file))
            {
                File.WriteAllText(settings_file, ObjectToJson(new AppSettings()));
            }
            return File.ReadAllText(settings_file).JsonToObject<AppSettings>();
        }

        public static void SaveAppSettings(this AppSettings settings, string settings_file = "settings.json")
        {
            File.WriteAllText(settings_file, settings.ObjectToJson());
        }

        public static List<T> GetObjectsFromStorage<T>(string storage_file)
        {
            string json;
            if (!File.Exists(storage_file))
            {
                var lst = new List<T>();
                json = ObjectToJson(lst);
                CompressStringToFile(json, storage_file);
            }

            json = DecompressFileToString(storage_file);
            return json.JsonToObject<List<T>>();
        }
    }

    public class MyDateTimeConvertor : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            else return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("dd.MM.yyyy"));
        }

        public override bool CanConvert(Type type)
        {
            return typeof(DateTime).IsAssignableFrom(type);
        }
    }
}
