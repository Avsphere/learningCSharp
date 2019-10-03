using System;
using System.Threading.Tasks;

namespace level_6_tasksAndFuncs
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // await TaskExample.Demo();
            await CancellationTokens.Demo();
        }
    }
}
