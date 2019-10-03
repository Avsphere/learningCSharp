using System.Threading.Tasks;

namespace EngineDataCollector
{
    interface ICanSense {
        void Start(int delayMs=1000);
        void Finish();
        string DataToJSONString();
    }

}