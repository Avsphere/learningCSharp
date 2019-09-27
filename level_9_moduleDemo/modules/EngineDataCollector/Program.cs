namespace EngineDataCollector
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System.Collections.Generic;


    static class Program
    {
        static ModuleClient IoTHubModuleClient;
        static ITransportSettings[] settings = { new AmqpTransportSettings(TransportType.Amqp_Tcp_Only) };
        static CancellationTokenSource cts = new CancellationTokenSource(); //This feels okay in the scope of this module, rather than passing from a higher scoped orchestrator
        static ICanSense[] sensors = { new InteractionSensor(), new TemperatureSensor() }; //because these are different classes ICanSense is most specific type selection

        public static void StartSensors()
        {
            foreach( ICanSense sensor in sensors ) {
                sensor.Start(100);
            }

        }
        public static void StopSensors()
        {
            foreach( ICanSense sensor in sensors ) {
                sensor.Finish();
            }
        }


        static TaskCompletionSource<bool> UntilCancelled() {
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            cts.Token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), taskCompletionSource);

            return taskCompletionSource; //This is essentially a blocking till termination
        }

        static Message MessageFromString(string value) {
            string message = JsonConvert.SerializeObject( new {
                message = value
            });
            return new Message(Encoding.UTF8.GetBytes(message));
        }

        public async static Task SendMessage(string dataString, string outputname="output1") {
            Message message = MessageFromString(dataString);
            message.Properties.Add("sensorData", "true");
            await IoTHubModuleClient.SendEventAsync(outputname, message).ConfigureAwait(false);
        }

        public static async Task StartCollectionInterval(int collectionDelayMs=1000) 
        {   
            while (!cts.Token.IsCancellationRequested)
            {
                foreach( ICanSense sensor in sensors )
                {
                    await SendMessage( sensor.DataToJSONString() ); //I suppose that if there were many sensors I would send in parallel
                }
                Console.WriteLine("EngineDataCollector is passing sensor data data"); //maybe use some type of reflection to output module name?
                await Task.Delay(collectionDelayMs);
            }
        }


        static async Task<MessageResponse> HandleModuleMessage(Message message, object userContext)
        {
            try
            {
                string messageString = Encoding.UTF8.GetString(message.GetBytes());
                Console.WriteLine("ENGINER COLLECTOR handler inter module message " + messageString );
                return MessageResponse.Completed;
            } 
            catch (Exception e)
            {   
                Console.WriteLine(e.ToString());
                return MessageResponse.Abandoned;
            } 
        }
        public static int Main() => MainAsync().Result;

        public static async Task<int> MainAsync() {
            /* INITIALIZATION */
            IoTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            EventSimulator.Start(); //I wonder if there is a better way to start simulator... decoupled from my non simulated code
            StartSensors();
            await IoTHubModuleClient.OpenAsync();
            await IoTHubModuleClient.SetInputMessageHandlerAsync("input2", HandleModuleMessage, IoTHubModuleClient);

            /* MAIN PROCESS */
            Task collectionInterval = StartCollectionInterval(collectionDelayMs : 3000);

            await UntilCancelled().Task;
            StopSensors();
            return 0;

        }
    }

}
