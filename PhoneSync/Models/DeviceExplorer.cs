using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.IO;

namespace PhoneSync.Models
{
    class DeviceExplorer : IDisposable
    {
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
            var result = new List<TransferInfo>();

            return result;
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

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }
    }
}
