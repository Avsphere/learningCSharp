using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Linq;

using Newtonsoft.Json;


namespace LearningIot {
    public class HubHelper {
        private ServiceClient hubClient;
        private string deviceId;
        public HubHelper(ServiceClient _serviceClient, string _deviceId) {
            hubClient = _serviceClient;
            deviceId = _deviceId;
            Console.WriteLine("Initialized hub helper");
        }

        public async Task SendMessageToDevice() {
            string messageString = "A very important message from the cloud to the device";
            Message commandMessage = new Message(Encoding.ASCII.GetBytes(messageString) );
            commandMessage.Ack = DeliveryAcknowledgement.Full;

            await hubClient.SendAsync(deviceId, commandMessage);
            Console.WriteLine("A message was sent from cloud to device");
        }

        public async Task ReceiveFeedback() {
            var feedbackReceiver = hubClient.GetFeedbackReceiver();

            var feedback = await feedbackReceiver.ReceiveAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
            
            if ( feedback != null ) {
                Console.WriteLine(
                    "Cloud has received a message {0}",
                    string.Join(", ", feedback.Records.Select( f => f.StatusCode))
                );
            }
        }
        

    }
}