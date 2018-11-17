using System;
using System.Collections.Generic;
using System.Text;

namespace WebScrapers
{
    public class ScrapeJob
    {
        public List<ScrapedProduct> AllScrapedProducts { get; set; }
        public List<ScrapedProduct> ScrapedProductsInKoopBlok { get; set; }
        public List<ScrapedProduct> ScrapedProductsNotInKoopBlok { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeSpan Duration
        {
            get
            {
                return Duration;
            }
            set
            {
                Duration = EndTime - StartTime;
            }
        }

        public List<string> PagesToScrape { get; set; }

        public int AmountOfPagesToScrape
        {
            get
            {
                return PagesToScrape.Count;
            }
            set
            {
                AmountOfPagesToScrape = PagesToScrape.Count;
            }
        }

        public ScrapeJob()
        {
            PagesToScrape = new List<string>();
            AllScrapedProducts = new List<ScrapedProduct>();
            ScrapedProductsInKoopBlok = new List<ScrapedProduct>();
            ScrapedProductsNotInKoopBlok = new List<ScrapedProduct>();
        }
    }
}
