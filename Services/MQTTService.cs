using System;
using System.Linq;
using System.Text;
using ElectronNET.API;
using ElectronNetWithMVC.Settings;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace  ElectronNetWithMVC.Services
{
    public class MQTTService
    {
        
        public static async void Start()
        {

            var username = Environment.UserName;
            // Setup and start a managed MQTT client.
            var options = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(username)
                .WithTcpServer(AppSettingsProvider.BrokerHostSettings.Host,AppSettingsProvider.BrokerHostSettings.Port)
                .Build())
            .Build();

        var mqttClient = new MqttFactory().CreateManagedMqttClient();
        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(AppSettingsProvider.BrokerHostSettings.Topic).Build());
        await mqttClient.StartAsync(options);

        // StartAsync returns immediately, as it starts a new thread using Task.Run, 
        // and so the calling thread needs to wait.
      Console.WriteLine($"### CONNECTED WITH SERVER+++++with clinet {username}+++++++++++++++++++++++++++++++++++ ###");
       mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
                    try
                    {
                        string topic = e.ApplicationMessage.Topic;

                        if (string.IsNullOrWhiteSpace(topic) == false)
                        {
                         string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);                        
                         //Notify(payload);
                        if(Electron.WindowManager.BrowserWindows.Count > 0)
                        {
                            var mainWindow = Electron.WindowManager.BrowserWindows.First();
                            mainWindow.LoadURL($"http://localhost:{BridgeSettings.WebPort}/Home/Push?t={DateTime.Now.ToLongTimeString()}&payload="+payload);                          
                             
                            
                        }
                        Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, ex);
                    }
                });
        }

    }
}