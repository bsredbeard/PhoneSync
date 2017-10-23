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
            Settings = AppSettings.Load();
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
                //var script = GetScript("Test.ps1");
                var output = new ObservableCollection<Tuple<string,string,string>>();
                output.CollectionChanged += (sender, evt) => {
                    foreach(var x in evt.NewItems){
                        var item = (Tuple<string, string, string>)x;
                        var status = (TransferStatus)Enum.Parse(typeof(TransferStatus), item.Item2);
                        var newItem = new TransferInfo(){
                            SourceFile = item.Item1,
                            Status = status,
                            DestinationFile = item.Item3
                        };
                        result.Add(newItem);
                        FileScaned?.Invoke(newItem);
                    }
                };
                using(PowerShell shell = PowerShell.Create()){
                    shell.AddScript(script);
                    shell.AddParameter("selectedDrive", sourceDrive);
                    shell.AddParameter("destination", destination);
                    shell.AddParameter("ignoreList", Settings.IgnorePaths.ToArray());
                    shell.Invoke(null, output);
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
