using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DataDummy
{
    public class Crawler {
        public async Task<string> crawl(int n=0) {
            string url = $"https://jsonplaceholder.typicode.com/todos/{n.ToString()}"; //here i use @ to forgive all \ delims
            HttpWebRequest request  = (HttpWebRequest)WebRequest.Create(url); //here i use (HttpWebRequest) to explictly cast this WebRequest
            HttpWebResponse res = (HttpWebResponse) await request.GetResponseAsync();
            System.IO.Stream stream = res.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            
            return await reader.ReadToEndAsync();
        }

        public async Task<string[]> crawlMany() {
            int[] pageIds = new[] { 1, 2, 3 };
            return await Task.WhenAll( pageIds.Select( i => crawl(i) ) );
        }
    }

    public class Analyze {
        public string aggregateTitles(string[] jsonStrings) {
            var aggregatedTitles = jsonStrings
            .Select( d => JObject.Parse(d) )
            .Select( o => o.GetValue("title").ToString() )
            .Aggregate( (acc, el) => string.Concat(acc, el) );
            return aggregatedTitles;
        }
    }
}