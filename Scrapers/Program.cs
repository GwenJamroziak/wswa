using System;

namespace Scrapers
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHtmlAsync();
            Console.ReadLine();
        }

        private static async void GetHtmlAsync()
        {
            var baseUrl = "https://www.bol.com/nl/w/algemeen/vendor/"
            var vendorId = 1159608;
            var url = baseUrl & vendorId;

        }
    }
}
