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
    class DeviceExplorer : IDisposable
    {
        public DeviceExplorer(){
            var appData = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsFile = System.IO.Path.Combine(appData, "settings.json");
            Settings = AppSettings.Load(settingsFile);
        }



        public async Task<List<string>> GetDrives()
        {
            return await new TaskFactory<List<string>>().StartNew(() =>
            {
                var result = new List<string>();
                var script = GetScript("GetDrives.ps1");
                using (PowerShell shell = PowerShell.Create())
                {
                    shell.AddScript(script);
                    var results = shell.Invoke();
                    foreach(var driveItem in results)
                    {
                        result.Add(driveItem.BaseObject.ToString());
                    }
                }
                return result;
            });
        }

        public async Task<List<TransferInfo>> TransferFiles(string sourceDrive, string destination = null)
        {
            if(string.IsNullOrWhiteSpace(destination)){
                destination = Settings.DestinationPath;
            }

            if(sourceDrive != Settings.DeviceName){
                Settings.DeviceName = sourceDrive;
            }

            return await new TaskFactory<List<TransferInfo>>().StartNew(() => {
                var result = new List<TransferInfo>();
                var script = GetScript("TransferFiles.ps1");
                var output = new ObservableCollection<string[]>();
                output.CollectionChanged += (sender, evt) => {
                    foreach(var x in evt.NewItems){
                        var item = (string[])x;
                        var status = (TransferStatus)Enum.Parse(typeof(TransferStatus), item[1]);
                        var newItem = new TransferInfo(){
                            SourceFile = item[0],
                            Status = status,
                            DestinationFile = item[2]
                        };
                        result.Add(newItem);
                        FileScaned?.Invoke(newItem);
                    }
                };
                using(PowerShell shell = PowerShell.Create()){
                    shell.AddScript(script);
                    shell.Invoke(new object[] { sourceDrive, destination, Settings.IgnorePaths.ToArray() }, output);
                }

                return result;
            });
        }

        private static string GetScript(string scriptName)
        {
            scriptName = string.Format(@"PhoneSync.Scripts.{0}", scriptName);

            var embeddedResources = typeof(DeviceExplorer).Assembly.GetManifestResourceNames();
            var matchScript = embeddedResources.FirstOrDefault(x => x.Equals(scriptName, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrWhiteSpace(matchScript))
            {
                using (var stream = typeof(DeviceExplorer).Assembly.GetManifestResourceStream(matchScript))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            throw new Exception("Script name was invalid");
        }

        public event Action<TransferInfo> FileScaned;

        public AppSettings Settings {get; private set;}

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Settings.Save();
            }
        }
    }
}
