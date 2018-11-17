using System;
using System.Collections.Generic;
using System.Text;

namespace WebScrapers
{
    public class User
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }

        public int CurrentEstimatedAmountOfScrapeablePages { get; set; }

        public ScrapeJob CurrentScrapeJob { get; set; }
        public List<ScrapeJob> ScrapeHistory { get; set; }


    }
}
