using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataViewer.Models;
using Microsoft.Extensions.Options;
using System.Data;
using System.Dynamic;
using DataViewer.Lib;

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
            DataTable dt = new DataTable();
            dt.Columns.Add("Person Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            dt.Columns.Add("Date Hire", typeof(DateTime));

            dt.Rows.Add(new object[3] { "Mike", 30, new DateTime(2018, 12, 1) });
            dt.Rows.Add(new object[3] { "Lori", 70, new DateTime(2019, 06, 15) });
            dt.Rows.Add(new object[3] { "Kevin", 20, new DateTime(2017, 03, 15) });

            return Json(new { recordsFiltered = dt.Rows.Count, recordsTotal = dt.Rows.Count, data = dt.JQDTFriendlyTableData(), columns = dt.JQDTFriendlyColumnList() });
            //return Json(new { recordsFiltered = 1, recordsTotal = 1, data = list.ToArray() });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
