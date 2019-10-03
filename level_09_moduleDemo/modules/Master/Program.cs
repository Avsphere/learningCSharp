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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Master
{
    static class Program
    {
        static string ENGINE_COLLECTOR_OUTPUT = "output2";   
        static int counter;
        static Random random = new Random();

        /*
            This follows the pattern I am seeing quite often of abstracting a settings object to some class that formally defines the configuration construct
            In this case TransportType is a floating enum e.g. Mqtt_Tcp_Only => 6, other options are Amqp =0, HTTP1 = 1, ...
            We then bundle these settings up in the ...Devices.Client namespace ITransportSettings interface (which MqttTransportSettings implements)

            ITransportSettings interface requires a : DefaultReceiveTimeout and a GetTransportType
         */

        private static ITransportSettings[] settings = { new MqttTransportSettings(TransportType.Mqtt_Tcp_Only) };
        static ModuleClient IoTHubModuleClient;

        
        static TaskCompletionSource<bool> UntilCancelled() {
            //This is the main entry for maintaining / terminating the process where we wait until unload or direct cancel

            CancellationTokenSource cts = new CancellationTokenSource(); //we create a token that allows us to cancel from the scope of whatever context, given that it has the token 
            
            //AssemblyLoadContext.Default.Unloading is our event handler delegate that we are adding this addtional handler to
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            
            //ie if we ctrl+C then we also use the cancellation token source to cancel
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

            /*
                This is actually quite a nice way to essentially wait upon a cancellation event. remember that TaskCompletion source is like a
                promise resolve or reject, in this case we are saying that upon a cancel, resolve(true)
             */

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            cts.Token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), taskCompletionSource);

            return taskCompletionSource; //This is essentially a blocking till termination
        }


        static async Task Main(string[] args)
        {
            IoTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await IoTHubModuleClient.OpenAsync();
            await IoTHubModuleClient.SetInputMessageHandlerAsync("input1", HandleIncomingMessage, IoTHubModuleClient);
            await UntilCancelled().Task; //essentially waiting for the resolve on a promise that contains an inner process that eventually rejects
        }

        public async static Task SendModuleMessage(string messageString, string outputname="output2") 
        {
            Message message = new Message( Encoding.UTF8.GetBytes(messageString) );
            message.Properties.Add("interModuleMessage", "true");
            await IoTHubModuleClient.SendEventAsync("output2", message).ConfigureAwait(false);
            Console.WriteLine("sent intermodule message");
        }

        static async Task<MessageResponse> HandleCommandMessage(string messageString)
        {
            try
            {

                await SendModuleMessage("testMessage", ENGINE_COLLECTOR_OUTPUT);

                return MessageResponse.Completed;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return MessageResponse.Abandoned;
            }
        }

        static async Task<MessageResponse> HandleSensorMessage(string messageString)
        {
            try
            {
                await SendModuleMessage("testMessage", "output2");

                JObject jsonMessage = JObject.Parse(messageString);
                JObject messageData = JObject.Parse( jsonMessage["message"].ToString() );
                JArray eventDatas = (JArray)messageData["data"];
                List<JObject> importantEventData = eventDatas
                    .Where( ev => ev.ToString().Length > 0 ) //make sure the jtoken was not a newline
                    .Select( ev => JObject.Parse(ev.ToString()) ) //JToken -> Jobject 
                    .Where(SensorEventFilter.IsImportantEvent) //Filter out "non-important" events
                    .ToList();

                if ( importantEventData.Count > 0 ) 
                {
                    string asJsonString = JsonConvert.SerializeObject(importantEventData);
                    Message deviceToCloudMsg = new Message( Encoding.UTF8.GetBytes(asJsonString) );
                    deviceToCloudMsg.Properties.Add("eventRank", "IMPORTANT");
                    await IoTHubModuleClient.SendEventAsync("output1", deviceToCloudMsg );
                    Console.WriteLine("Sent {0} important events to hub!", importantEventData.Count);
                }
                return MessageResponse.Completed;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return MessageResponse.Abandoned;
            }
        }


        static async Task<MessageResponse> HandleIncomingMessage(Message message, object userContext)
        {
            try
            {
                int counterValue = Interlocked.Increment(ref counter);
                string messageString = Encoding.UTF8.GetString(message.GetBytes());
                // Console.WriteLine( message.Properties.ContainsKey("sensorData") + message.InputName );

                if ( !string.IsNullOrEmpty(messageString) && message.Properties.ContainsKey("sensorData") )
                    return await HandleSensorMessage(messageString);
                if ( !string.IsNullOrEmpty(messageString) && message.Properties.ContainsKey("commandMessage") )
                    return await HandleCommandMessage(messageString);

                throw new Exception("Message type not recognized");
            } 
            catch (Exception e)
            {   
                Console.WriteLine(e.ToString());
                return MessageResponse.Abandoned;
            } 
        }

        //     byte[] messageBytes = message.GetBytes();
        //     string messageString = Encoding.UTF8.GetString(messageBytes);
        //     Console.WriteLine($"Received message: {counterValue}, Body: [{messageString}]");

        //     if ( !string.IsNullOrEmpty(messageString))
        //     {
        //         var pipeMessage = new Message(messageBytes);
        //         foreach (var prop in message.Properties)
        //         {
        //             Console.WriteLine("prop key " + prop.Key + "prop value" + prop.Value );
        //             pipeMessage.Properties.Add(prop.Key, prop.Value);
        //         }
        //         await IoTHubModuleClient.SendEventAsync("output1", pipeMessage);
        //         Console.WriteLine("Received message sent");
        //     }
        //     return MessageResponse.Completed;
        // }
    }
}
