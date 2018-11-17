using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace WebScrapers
{
    public class ScrapeService
    {
        //this has to be refactored, service cannot hold state!
        public string VendorNameByMethodCall { get; set; }
        string defaultVendorPLP = "https://www.bol.com/nl/w/algemeen/vendor/";
        string baseUrlLeftSide = "https://www.bol.com/nl/w/algemeen/-/";
        string baseUrlMiddleSide = "/index.html?page=";
        string baseUrlRightSide = "&view=list";
        HtmlDocument htmlDocument = new HtmlDocument();
        ScrapeJob scrapeJob = new ScrapeJob();
        HttpClient httpClient = new HttpClient();
        int amountOfProductsPerPLP = 24;
        int vendorId;

        public void ReturnVendorName(int vendorId, HttpClient httpClient)
        {
            this.vendorId = vendorId;
            var baseVendorUrlForPaginationIndexation = defaultVendorPLP + vendorId;
            var html =
                httpClient
                .GetStringAsync(baseVendorUrlForPaginationIndexation)
                .Result;

            htmlDocument.LoadHtml(html);
            VendorNameByMethodCall = ReadVendorName(htmlDocument);
        }

        public string QuickScanForEstimation(int vendorId)
        {
            this.vendorId = vendorId;
            var baseVendorUrlForPaginationIndexation = defaultVendorPLP + vendorId;
            var html =
                httpClient
                .GetStringAsync(baseVendorUrlForPaginationIndexation)
                .Result;

            htmlDocument.LoadHtml(html);
            int maxPageNumber = CalculateMaxPageNumber(htmlDocument);
            string vendorName = ReadVendorName(htmlDocument);
            int totalPageCount = maxPageNumber + (maxPageNumber * amountOfProductsPerPLP);
            var estimatedDuration = TimeSpan.FromSeconds(totalPageCount * 2);

            string result = $"Dag {vendorName}, "
                           + $"vendorId is {vendorId} "
                           + $"aantal PLPs is {maxPageNumber} "
                           + $"geschatte kost is {totalPageCount} page credits "
                           + $"dit zal ongeveer {estimatedDuration.Hours} uur, "
                           + $"{estimatedDuration.Minutes} minuten en "
                           + $"{estimatedDuration.Seconds} seconden duren.";

            return result;
        }
        public List<string> UrlsToScrape()
        {
            return null;
        }

        public ScrapeJob ScrapeAllUrls()
        {
            return null;
        }
        public async void StartAScrapeJob(string vendorId)
        {
            var baseVendorUrlForPaginationIndexation = defaultVendorPLP + vendorId;
            var html = await httpClient.GetStringAsync(baseVendorUrlForPaginationIndexation);
            htmlDocument.LoadHtml(html);

            //1 QuickRead max page number and vendorName
            int maxPageNumber = CalculateMaxPageNumber(htmlDocument);
            string vendorName = ReadVendorName(htmlDocument);
            int totalPageCount = maxPageNumber + (maxPageNumber * amountOfProductsPerPLP);
            int estimatedDuration = totalPageCount * 2;

            //3 iterate over all PLPs to add product urls to list
            //for (int i = 1; i <= maxPageNumber; i++) // real code
            for (int i = 1; i <= 2; i++) // faster test code
            {
                Thread.Sleep(2000); //respect regulations
                var PLPurl = baseUrlLeftSide + vendorId + baseUrlMiddleSide + i + baseUrlRightSide;

                htmlDocument = new HtmlDocument();
                html = await httpClient.GetStringAsync(PLPurl);
                htmlDocument.LoadHtml(html);

                //add all the PLP listed productHrefs to the scrapejobs list
                AddProductsToTheScrapingList(htmlDocument, scrapeJob);
            }
            Console.WriteLine("total amount of pages to scrape : " + scrapeJob.AmountOfPagesToScrape);

            //4 start visiting each page and write the details of the scrapedProducts to the ScrapJobs list 'ScrapedProducts'
            foreach (var url in scrapeJob.PagesToScrape)
            {
                Thread.Sleep(2000); //respect regulations

                htmlDocument = new HtmlDocument();
                html = await httpClient.GetStringAsync(url);
                htmlDocument.LoadHtml(html);

                string Ean = "search ean in DOM structure";
                string productName = GetProductName(htmlDocument);
                string ProductPrice = GetProductPrice(htmlDocument);
                string ProductKoopblok = GetProductKoopblok(htmlDocument);

                Console.WriteLine("productname: " + productName);
                Console.WriteLine("koopblok: " + ProductKoopblok);
                Console.WriteLine("prijs: " + ProductPrice);

                var scrapedProduct =
                    new ScrapedProduct(
                         productName,
                         ProductPrice,
                         ProductKoopblok
                         );

                scrapeJob.AllScrapedProducts.Add(scrapedProduct);

                if (scrapedProduct.ProductKoopblok == vendorName)
                {
                    scrapeJob.ScrapedProductsInKoopBlok.Add(scrapedProduct);
                    Console.WriteLine("product added to inkoopbloklist");
                }
                else
                {
                    scrapeJob.ScrapedProductsNotInKoopBlok.Add(scrapedProduct);
                    Console.WriteLine("product added to notinkoopbloklist");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }

        private static string GetProductKoopblok(HtmlDocument htmlDocument)
        {
            var ProductKoopblok = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                    .Equals("buy-block__seller-name"))
                        .FirstOrDefault()
                        .InnerText
                        .Replace("\n", "")
                        .Trim()
                        .Replace("Verkoop door partner van bol.com", "")
                        .Trim();
            while (ProductKoopblok.Contains("  "))
            {
                ProductKoopblok = ProductKoopblok.Replace("  ", " ");
            }
            return ProductKoopblok;
        }

        private static string GetProductName(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("h1")
                .Where(node => node.GetAttributeValue("class", "")
               .Equals("pdp-header__title bol_header"))
               .FirstOrDefault()
               .InnerText;
        }

        private static string GetProductPrice(HtmlDocument htmlDocument)
        {
            var ProductPrice = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("price-block__price"))
                .FirstOrDefault()
                .InnerText
                .Trim()
                .Replace("\n", "");
            ProductPrice = ProductPrice.Replace(",-", ""); // not working?
            while (ProductPrice.Contains(" "))
            {
                ProductPrice = ProductPrice.Replace(" ", "");
            }
            return ProductPrice;
        }

        // SUBMETHOD
        private static void AddProductsToTheScrapingList(HtmlDocument htmlDocument, ScrapeJob scrapeJob)
        {
            var ProductsHtml = htmlDocument.DocumentNode.Descendants("a")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("product-title")).ToList();

            //here starts the scan for each productHref on a PLP
            foreach (var ProductListItem in ProductsHtml)
            {
                var ProductHref = ProductListItem
                    .Attributes
                    .Where(attribute => attribute.Name == "href")
                    .FirstOrDefault()
                    .Value
                    .Replace("?suggestionType&#x3D;browse", "")
                    .Insert(0, "https://www.bol.com");
                Console.WriteLine(ProductHref);
                scrapeJob.PagesToScrape.Add(ProductHref);

            }
            Console.WriteLine("done looping over current page, total urls : " + scrapeJob.AmountOfPagesToScrape);
        }

        // SUBMETHOD
        private static string ReadVendorName(HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.Descendants("h1")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("h2 clear_autoheight h-bottom--xs top_0 js_content_title_control"))
                    .FirstOrDefault()
                    .InnerText
                    .Replace("Alle artikelen van ", "");

        }
        // SUBMETHOD
        private static int CalculateMaxPageNumber(HtmlDocument htmlDocument)
        {
            var TotalProductListPages = htmlDocument.DocumentNode.Descendants("ul")
            .Where(node => node.GetAttributeValue("class", "")
            .Equals("pagination"))
            .FirstOrDefault()
            .InnerText
            .Replace("\n", "");

            while (TotalProductListPages.Contains("  "))
            {
                TotalProductListPages = TotalProductListPages.Replace("  ", " ");
            }

            TotalProductListPages = TotalProductListPages.Replace(" 1 2 3 4 5 &hellip; ", "");

            int maxPageNumber = Convert.ToInt32(TotalProductListPages);
            return maxPageNumber;
        }


    }
}
