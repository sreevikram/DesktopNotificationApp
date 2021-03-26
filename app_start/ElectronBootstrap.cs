using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;

namespace ElectronNetWithMVC
{    
    public class ElectronBootstrap
    {
        
        public static async void Start(IWebHostEnvironment _env)
        {          
            var browserOptions = new BrowserWindowOptions()
            {
                Width = 300,
                Height = 200,
                Show = false,
                Frame=false,
                Resizable=false,                
                SkipTaskbar=true,
                AlwaysOnTop=true,
                WebPreferences = new WebPreferences(){
                NodeIntegration=true

                } } ;             
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(browserOptions);
            
            await browserWindow.WebContents.Session.ClearCacheAsync();            
           
          // browserWindow.SetProgressBar(0.50);
             browserWindow.OnClosed += () => {             
               Electron.App.Quit();               
            };

            

            browserWindow.OnReadyToShow += async() =>{ 
                
                //browserWindow.Show();
                Rectangle newBounds = await GetTrayPosition(browserOptions.Height,browserOptions.Width);                
                browserWindow.SetBounds(newBounds,true);  
                browserWindow.SetTitle("Realpage Dhadkan");
                
            };
           

            
            Electron.IpcMain.On("showWindow", (e) =>{
                browserWindow.Show();
             });
            Electron.IpcMain.On("hideToSystemTray", (e) =>{
                browserWindow.Hide();
             });

            if (Electron.Tray.MenuItems.Count == 0)
            {
                var menu = new MenuItem[]
                {
                    new MenuItem
                    {
                        Label = "Show Window",
                        Click = () => browserWindow.Show()
                    },
                    new MenuItem
                    {
                        Label = "Exit",
                        Click = () => Electron.App.Exit()
                    }
                };

                Electron.Tray.Show(Path.Combine(_env.ContentRootPath, "wwwroot/app-icon/png/32.png"), menu);
                Electron.Tray.SetToolTip("Dhadkan");
            }

                Electron.Tray.OnClick +=   (TrayClickEventArgs args,Rectangle trayBounds ) => {
                    PositionWindowToTray();
                };

                Electron.IpcMain.On("async-msg", (args) =>
                {
                    args = String.IsNullOrEmpty(args.ToString())? "Null or Empty":args;                   
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    mainWindow.Hide();
                    Console.WriteLine($"args: {args}. ");
                });            
        }

        public static async void PositionWindowToTray()
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            var mWindowbounds = await mainWindow.GetBoundsAsync();
            var result = ( await mainWindow.IsVisibleAsync());

            if( result){
                mainWindow.Hide();         
            }
            else{                
                Rectangle newBounds = await GetTrayPosition(mWindowbounds.Height,mWindowbounds.Width);                
                mainWindow.SetBounds(newBounds,true);                        
                //mainWindow.SetAlwaysOnTop(true);
                mainWindow.Show();         
                //mainWindow.SetAlwaysOnTop(false);
            }
        }
        
        public static async Task<Rectangle> GetTrayPosition(int _height, int _width)
        {
             var mainWindow = Electron.WindowManager.BrowserWindows.First();
            var mWindowbounds = await mainWindow.GetBoundsAsync();
            var trayBounds =  await Electron.Tray.GetBoundsAsync();
            int yPostion = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)?trayBounds.Y -_height :trayBounds.Y;
            Rectangle newBounds = new Rectangle(){
                X= trayBounds.X -  (_width/2),
                Y=yPostion,
                Height=_height,
                Width=_width
            };
            
            return newBounds;
        }
    }
}