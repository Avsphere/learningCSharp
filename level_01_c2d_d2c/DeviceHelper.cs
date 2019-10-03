using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;


namespace LearningIot {
    public class DeviceHelper {
        private DeviceClient deviceClient;
        private const int messageCount = 10;
        private float temperature = 0;
        private float humidity = 0;
        public DeviceHelper(DeviceClient _deviceClient) {
            deviceClient = _deviceClient;
            Console.WriteLine("Initialized device helper");
        }

        private string generateMessage() {
            string message = JsonConvert.SerializeObject( new { 
                temperature = temperature++,
                humidity = humidity++
             });
            return message;
        }
        public async Task SendEvent() {
            string message = generateMessage();
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(message));
            
            await deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);
            Console.WriteLine("Device succesfully sent message: {0} ", message);
        }

        public async Task ReceiveCommands() {
            Message receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false); //my understanding is that this will listen in a non blocking way for 30 seconds?

            if ( receivedMessage != null ) {
                string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine("Received message {0}", messageData);

                await deviceClient.CompleteAsync(receivedMessage).ConfigureAwait(false);
            } else {
                Console.WriteLine("Receive message timed out");
            }

        }


    }
}