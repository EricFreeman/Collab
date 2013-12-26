using System;
using System.IO;
using Collab.Models;
using Extensions;
using Newtonsoft.Json;

namespace Collab.Services
{
    public static class ConfigService
    {
        public static ConfigModel Generate(string path)
        {
            var json = File.ReadAllText(Path.Combine(path, ".config"));
            return JsonConvert.DeserializeObject<ConfigModel>(json);
        }

        public static void GenerateNewCurrent()
        {
            var rand = new Random();
            var config = new ConfigModel {Width = rand.Next(4, 16), Height = rand.Next(4, 16)};
            string json = JsonConvert.SerializeObject(config);
            File.WriteAllText("~/UploadedImages/Current/.config".ToMapPath(), json);
        }
    }
}