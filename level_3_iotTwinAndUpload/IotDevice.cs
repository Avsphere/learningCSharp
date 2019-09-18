using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;


namespace level_3_iotTwinAndUpload {
    public static class IotDevice {
        private static string connectionString = Environment.GetEnvironmentVariable("DEVICE_CONN_STRING");
        private static TransportType mqttTransport = TransportType.Mqtt;

        private static DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString, mqttTransport);

        public static Message generateMessage(string value) {
            string message = JsonConvert.SerializeObject( new {
                data = value
            });
            return new Message(Encoding.UTF8.GetBytes(message));
        }
        public async static Task sendMessage(string dataString="Beep boop says the device") {
            Message message = generateMessage(dataString);
            await deviceClient.SendEventAsync(message).ConfigureAwait(false);
            Console.WriteLine("device sent message to iothub");
        }

        public static async Task receiveMessageOnDevice(int listenFor=100) {
            Message receivedMessage = await deviceClient
            .ReceiveAsync(TimeSpan.FromSeconds(listenFor))
            .ConfigureAwait(false); //my understanding is that this will listen in a non blocking way for x seconds?
            
            Console.WriteLine("Device has received message from cloud" + Encoding.ASCII.GetString(receivedMessage.GetBytes()));
            await deviceClient.CompleteAsync(receivedMessage).ConfigureAwait(false);
        }

        public static async Task uploadDummyFileToCloud() {
            string filePath = DummyFile.createDummyFile();
            using( var fileStream = new FileStream(filePath, FileMode.Open) ) {
                const string filename = "aDummyFile.txt";
                await deviceClient.UploadToBlobAsync(filename, fileStream).ConfigureAwait(false);
            }
            DummyFile.removeDummyFile(); //moving outside the using block so no resource conflicts
        }


    }
}