using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace dewebscraper
{
    public class Scraper
    {
        public int vendorId { get; set; }

        public static  List<String> BScrape(int vendorId)
        {
            var url = $"http://{vendorId}/";

            var httpClient = new HttpClient();
            var html =  httpClient.GetStringAsync(url);
            //var htmlDocument = new HtmlDocument(); 
            //install nuget package htmlAgility

            return null;
        }
    }
}
