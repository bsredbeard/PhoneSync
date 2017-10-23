using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.IO;
using System.Configuration;

namespace PhoneSync.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            IgnorePaths = new List<string> { @"\DCIM\.thumbnails\", @"\Android\data\" };
        }

        [Newtonsoft.Json.JsonIgnore()]
        public string SettingsPath { get; set; }
        public string DestinationPath { get; set; }
        public string DeviceName { get; set; }
        public List<string> IgnorePaths { get; set; }

        public bool AutoSync { get; set; }

        public void Save()
        {
            Save(new FileInfo(SettingsPath));
        }

        private void Save(FileInfo destinationFile)
        {
            if (!destinationFile.Directory.Exists)
            {
                destinationFile.Directory.Create();
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            using (var writer = new StreamWriter(destinationFile.FullName, false))
            {
                writer.Write(data);
                writer.Flush();
                writer.Close();
            }
        }

        private static Dictionary<string, AppSettings> _cache = new Dictionary<string, AppSettings>();

        public static AppSettings Load(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(appData, typeof(AppSettings).Assembly.GetName().Name, "settings.json");
            }

            if (_cache.ContainsKey(path))
            {
                return _cache[path];
            }

            var file = new FileInfo(path);
            if (file.Exists)
            {
                using (var stream = file.OpenRead())
                using (var reader = new StreamReader(stream))
                {
                    var contents = reader.ReadToEnd();
                    var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(contents);
                    settings.SettingsPath = path;
                    _cache[path] = settings;
                    return settings;
                }
            }
            else
            {
                var settings = new AppSettings { SettingsPath = path };
                settings.Save(file);
                _cache[path] = settings;
                return settings;
            }
        }
    }
}