using System;
using System.Threading.Tasks;
using DeviceClient = Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices;

using LearningIot;

namespace level_1
{
    class Program
    {

        private static string deviceConnectionString = Environment.GetEnvironmentVariable("DEVICE_CONN_STRING");
        private static string hubConnectionString = Environment.GetEnvironmentVariable("IOTHUB_CONNECTION_STRING");
        private static string deviceId = Environment.GetEnvironmentVariable("DEVICE_ID");
        private static DeviceClient.TransportType mqttTransport = DeviceClient.TransportType.Mqtt;
        public static async Task Main(string[] args)
        {
            Console.WriteLine(deviceConnectionString);
            DeviceClient.DeviceClient deviceClient = DeviceClient
            .DeviceClient
            .CreateFromConnectionString(deviceConnectionString, mqttTransport);

            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(hubConnectionString);

            HubHelper hubHelper = new HubHelper(serviceClient, deviceId);
            DeviceHelper deviceHelper = new DeviceHelper(deviceClient);



            // deviceHelper.ReceiveCommands();
            hubHelper.ReceiveFeedback();
            
            
            await deviceHelper.SendEvent();
            // await hubHelper.SendMessageToDevice();

            Task.Delay(10000).Wait(); //if I block this thread then will the others continue?

            Console.WriteLine("All done");


        }
    }
}
