using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;


namespace level_3_iotTwinAndUpload {
    public static class IotTwin {
        private static string connectionString = Environment.GetEnvironmentVariable("DEVICE_CONN_STRING");
        private static TransportType mqttTransport = TransportType.Mqtt;

        private static DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString, mqttTransport);

        public static Message generateMessage(string value) {
            string message = JsonConvert.SerializeObject( new {
                data = value
            });
            return new Message(Encoding.UTF8.GetBytes(message));
        }
        public static async Task twinSample() {
            await deviceClient.SetDesiredPropertyUpdateCallbackAsync(onPropertyChange, null).ConfigureAwait(false);
            Twin twin = await deviceClient.GetTwinAsync();
            JObject jsonTwin = (JObject)JsonConvert.DeserializeObject(twin.ToJson());

            JObject twinProperties = (JObject)jsonTwin["properties"]; //returns another jobject

            // Console.WriteLine((string)twinProperties["desired"]["cat"]);

            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["non-cat"] = "not best friend";

            await deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
            await Task.Delay(30 * 1000);
        }

        //while this is set above, it means desired properties change from within iotHub
        public static async Task onPropertyChange( TwinCollection desiredProperties, object userContext ) {
            Console.WriteLine("onPropertyChange : Desired properties" + desiredProperties.ToJson());
            
            string dataField = "blablabla";
            Console.WriteLine("Sending data as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties[dataField] = DateTime.Now;
            await deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

            Console.WriteLine("Updated twin reported properties");
        }

    }
}