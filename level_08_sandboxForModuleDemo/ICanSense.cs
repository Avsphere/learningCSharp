using System.Threading.Tasks;

namespace level_8_sandboxForModuleDemo
{
    interface ICanSense {
        void Start(int delayMs=100);
        void Finish();
        string DataToJSONString();
    }

}