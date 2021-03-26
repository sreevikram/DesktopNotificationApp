using Coravel.Invocable;
using ElectronNET.API;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronNetWithMVC.Invocables
{
    public class ReminderJob : IInvocable
    {
        public ReminderJob()
        {
        }

        public async Task Invoke()
        {
            Console.WriteLine($"Reminder Job Invoked at {DateTime.Now.ToLongTimeString()}");
            await Task.Run(()=> {
                    if(Electron.WindowManager.BrowserWindows.Count > 0)
                            {                          
                                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                                var dt = DateTime.Now.ToLongTimeString();
                                mainWindow.LoadURL($"http://localhost:{BridgeSettings.WebPort}/Home/Reminder"); 
                            }
            });
          
        }
    }
}
