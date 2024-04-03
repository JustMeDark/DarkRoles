using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DarkRoles.Modules.Customs.Json
{
    public class JsonReader
    {
        public static string ReadJson(string path, string str)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream is null) return "Null";
            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            var parse = JObject.Parse(json);
            if (parse is not null && str is not null)
            {
                string result = parse[str].ToString();
                return result;
            }
            return "Please provide a valid string";
        }

        public static string ReadMultiLineJson(string path, string string1, string string2)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream is null) return "Null";
            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            var parse = JObject.Parse(json);
            if (parse is not null && string1 is not null)
            {
                string result = parse[string1][string2].ToString();
                return result;
            }
            return "Please provide a valid string";
        }

        public static string ReadExternalJson(string path, string string1)
        {
            var stream = $"{path}";
            if (stream is null) return "Null";
            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            var parse = JObject.Parse(json);
            if (parse is not null && string1 is not null)
            {
                string result = parse[string1].ToString();
                return result;
            }
            return "Please provide a valid string";
        }

        public static string ReadExternalMultiLineJson(string path, string string1, string string2)
        {
            var stream = $"{path}";
            if (stream is null) return "Null";
            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            var parse = JObject.Parse(json);
            if (parse is not null && string1 is not null)
            {
                string result = parse[string1][string2].ToString();
                return result;
            }
            return "Please provide a valid string";
        }
    }
}