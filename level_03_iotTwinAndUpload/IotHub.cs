using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Text;


namespace level_3_iotTwinAndUpload {
    public static class IotHub {
        public static string deviceId = Environment.GetEnvironmentVariable("DEVICE_ID");
        private static string connectionString = Environment.GetEnvironmentVariable("IOTHUB_CONNECTION_STRING");

        private static ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(connectionString);


        public static Message generateMessage(string value) {
            string message = JsonConvert.SerializeObject( new {
                data = value
            });
            return new Message(Encoding.ASCII.GetBytes(message));
        }
        
        public async static Task sendMessageToDevice(string dataString="beep boop" ){
            Message cmdMessage = generateMessage(dataString);
            cmdMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(deviceId, cmdMessage);
        }

        public static async Task receiveMessageFeedback(int listenFor=100) {
            FeedbackReceiver<FeedbackBatch> feedbackReceiver = serviceClient.GetFeedbackReceiver();
            FeedbackBatch feedback = await feedbackReceiver.ReceiveAsync(TimeSpan.FromSeconds(listenFor)).ConfigureAwait(false);
            if ( feedback != null ) {
                Console.WriteLine(
                    "IotHub has received a message {0}",
                    string.Join(", ", feedback.Records)
                );
            } else {
                Console.WriteLine("IotHub has not received any messages");
            }
        }

    }
}