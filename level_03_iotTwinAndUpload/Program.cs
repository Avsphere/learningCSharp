using System;
using System.Threading.Tasks;

namespace level_3_iotTwinAndUpload
{
    class Program
    {

        static async Task Main(string[] args)
        {
            IotDevice.receiveMessageOnDevice();
            // IotHub.receiveMessageFeedback();

            await IotHub.sendMessageToDevice();
            await IotHub.sendMessageToDevice();
        
            // await IotDevice.uploadDummyFileToCloud();

            // Console.WriteLine(message);
            await Task.Delay(100000);
        }
    }
}
