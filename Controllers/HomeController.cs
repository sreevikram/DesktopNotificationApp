using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ElectronNetWithMVC.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using System.Threading;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using ElectronNET.API;
using System.Drawing;

namespace ElectronNetWithMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
  
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()
        {
           PayloadModel model = new PayloadModel();
            model.Title ="Realpage";
            model.SubTitle ="Notification Buddy";
            model.Body ="A GOOD LAUGH and LONG SLEEP are two BEST CURES for anything";
            model.Actions = new List<string>(){"Yes","No"};          
       
            return View(model);
        }

        public IActionResult Push(string payload)
        {
           
            PayloadModel model;
            try  {
             model =  JsonSerializer.Deserialize<PayloadModel>(payload);
            }
            catch(Exception  ex)
            {
                Console.WriteLine(ex.ToString());
                model = new PayloadModel();
                model.Title ="Realpage";
                model.SubTitle ="Notification Buddy";
                model.Body =payload;
                
            }          
         

            return View("index",model);
        }


        public  IActionResult Reminder()
        {
             Console.WriteLine($"Reminder action call at {DateTime.Now.ToLongTimeString()}");
            PayloadModel model = new PayloadModel();
            model.Title ="Realpage";
            model.SubTitle ="Notification Buddy -" + DateTime.Now.ToShortTimeString();
            model.Body ="A GOOD LAUGH and LONG SLEEP are two BEST CURES for anything";
            model.Actions = new List<string>(){"Yes","No"};          
       
            return View("index",model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
