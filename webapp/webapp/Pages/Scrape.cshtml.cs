using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebScrapers;

namespace webapp.Pages
{
    public class ScrapeModel : PageModel
    {
        public void OnGet()
        { }

        public string CallVendorNameFromService(int vendorId)
        {
            {
                //var ws = new ScrapeService();
                //ws.ReturnVendorName(vendorId);
                //var result = ws.VendorNameByMethodCall;
                //return result;

                return "123";
            }
        }

        public string QuickScanFromService(int vendorId)
        {
            var ws = new ScrapeService();
            return ws.QuickScanForEstimation(vendorId);
            
        }
    }
}