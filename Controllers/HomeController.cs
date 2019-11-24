using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataViewer.Models;
using Microsoft.Extensions.Options;


namespace DataViewer.Controllers
{
    public class HomeController : Controller
    {
        string connStr;
        public HomeController(IOptions<ConnectionStringOptions> op)
        {
            connStr = op.Value.DataViewerConnStr;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetParentTables()
        {
            List<Something> list = new List<Something>();
            list.Add(new Something("Person A"));

            return Json(new { recordsFiltered = 1, recordsTotal = 1, data = list.ToArray() });
            //return "{ \"data\": [{ \"name1\": \"Johns\" }, { \"name1\": \"Kevin\" }, { \"name1\": \"Kevin\" }, { \"name1\": \"Larry\" }] }";
        }

        public class Something
        {
            public Something(string name)
            {
                Name1 = name;
            }
            public string Name1 { get; set; }
        }








        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
