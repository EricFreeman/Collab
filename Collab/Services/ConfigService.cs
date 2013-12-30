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
            var fullPath = Path.Combine(path, ".config");
            string json = string.Empty;

            try
            {
                json = File.ReadAllText(fullPath);
            }
            catch (FileNotFoundException)
            {
                var config = new ConfigModel {Width = 10, Height = 10};
                json = JsonConvert.SerializeObject(config);

                using (var s = new StreamWriter(fullPath))
                {
                    s.Write(json);
                }
            }


            return JsonConvert.DeserializeObject<ConfigModel>(json);
        }

        public static void GenerateNewCurrent()
        {
            var rand = new Random();
            var config = new ConfigModel {Width = rand.Next(4, 16), Height = rand.Next(4, 16)};
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText("~/UploadedImages/Current/.config".ToMapPath(), json);
        }
    }
}