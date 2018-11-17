using System;
using System.Collections.Generic;
using System.Text;

namespace WebScrapers
{
    public class ScrapedProduct
    {
        public string Ean { get; set; }
        public string ProductTitle { get; set; }
        public string ProductPrice { get; set; }
        public string ProductKoopblok { get; set; }

        public ScrapedProduct(string productTitle, string productPrice, string productKoopblok)
        {
            ProductTitle = productTitle;
            ProductPrice = productPrice;
            ProductKoopblok = productKoopblok;
        }
    }
}
