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
  public class AppSettings{
    public AppSettings(){
      IgnorePaths = new List<string>{ @"\DCIM\.thumbnails\", @"\Android\data\"};
    }

    [Newtonsoft.Json.JsonIgnore()]
    public string SettingsPath{ get; set;}
    public string DestinationPath {get;set;}
    public string DeviceName{get;set;}
    public List<string> IgnorePaths{ get; set;}

    public bool AutoSync{ get; set; }

    public void Save(){
      var data = Newtonsoft.Json.JsonConvert.SerializeObject(this);
      using(var writer = new StreamWriter(SettingsPath, false))
      {
        writer.Write(data);
        writer.Flush();
        writer.Close();
      }
    }

    private static Dictionary<string, AppSettings> _cache = new Dictionary<string, AppSettings>();

    public static AppSettings Load(string path){
      if(_cache.ContainsKey(path)){
        return _cache[path];
      }

      var file = new FileInfo(path);
      if(file.Exists){
        using(var stream = file.OpenRead())
        using(var reader = new StreamReader(stream))
        {
          var contents = reader.ReadToEnd();
          var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(contents);
          settings.SettingsPath = path;
          _cache[path] = settings;
          return settings;
        }
      } else {
        var settings = new AppSettings { SettingsPath = path };
        settings.Save();
        _cache[path] = settings;
        return settings;
      }
    }
  }
}