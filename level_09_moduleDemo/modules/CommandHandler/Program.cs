namespace dummyTemperatureTelemetry
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
    using System.Collections;

    /* Random

        When this module returns or fails, iotEdge container aka our module orchestrator notices this
        it then attempts to restart the container, thus a routine of
            send Message then exit
        is basically a very inefficient loop, what I should do is setup another cts token
    
    
     */

     /*
        I am going to split up these modules, Program will parse the config and send over the connection string / cts
     
        In this class I will have a few h
      */
    class Program
    {

        //these all being lowercased as they are local va
        static ITransportSettings[] settings = { new AmqpTransportSettings(TransportType.Amqp_Tcp_Only) };
        static ModuleClient IoTHubModuleClient;
        static string connectionString;
        static TransportType mqttTransport = TransportType.Mqtt;
        static DeviceClient deviceClient;
        static CancellationTokenSource cts = new CancellationTokenSource(); 

        static TaskCompletionSource<bool> UntilCancelled() {
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            cts.Token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), taskCompletionSource);

            return taskCompletionSource; //This is essentially a blocking till termination
        }


        public static int Main() => MainAsync().Result;

        public static async Task HandleCommandMessages(int listenFor=100000) {
            //listenFor x ms then handle message or try again -- skeptical on if this is the best way
            while(!cts.Token.IsCancellationRequested) {
                Message receivedMessage = await deviceClient
                .ReceiveAsync(TimeSpan.FromSeconds(listenFor))
                .ConfigureAwait(false); //my understanding is that this will listen in a non blocking way for x seconds?
                
                /* Normally I would decouple, but I am trying of the long simulator build and want to finish this demo */

                string messageString = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                Console.WriteLine("Beepboop doing important things with commandMessage : " + messageString);

                await deviceClient.CompleteAsync(receivedMessage).ConfigureAwait(false);
            }
        }



        public static async Task<int> MainAsync() {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("config/dev.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            connectionString = configuration.GetValue<string>("deviceConnectionString");
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, mqttTransport);
            IoTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await IoTHubModuleClient.OpenAsync();
            Console.WriteLine("Command handler started!");

            HandleCommandMessages();

            // listenForCommands(); //basically forked
            await UntilCancelled().Task;


            return 0;

        }
    }

}
