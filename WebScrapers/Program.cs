using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Threading;

namespace WebScrapers
{
    public class Program
    {
        static void Main(string[] args)
        {
            var scrapeJob = new ScrapeJob();
            var user = new User();
            var scrapeService = new ScrapeService();

            scrapeService.StartAScrapeJob("1111197");

            Console.ReadLine(); // catch debug before exiting
        }

    }
}