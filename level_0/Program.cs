using System;
using DataDummy;
using System.Threading.Tasks;
using System.Linq;

namespace level_0
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Crawler crawler = new Crawler();
            Analyze analyze = new Analyze();
            string[] responses = await crawler.crawlMany();
            string aggregatedTitles = analyze.aggregateTitles(responses);

        }

    }
}
