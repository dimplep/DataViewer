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
using DataViewer.Data;

namespace DataViewer.Controllers
{
    public class HomeController : Controller
    {
        IDataAccess _dataAccess;
        BusinessLayer _businessLayer;
        public HomeController(IOptions<DateSettingOptions> op)
        {
            string connStr = op.Value.ConnectionString;
            string dbms = op.Value.RDBMS;
            string relationsJsonFileName = op.Value.RelationsJsonFileName;
            _dataAccess = DataSwitcher.DataAccessByDBMS(connStr, dbms);
            _businessLayer = new BusinessLayer(_dataAccess, relationsJsonFileName);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult InitialScreenData()
        {
            try
            {
                List<string> allTables = _businessLayer.AllTables();

                return Json(new { allTables = allTables, operators = _businessLayer.FilterOperators(), columns = _businessLayer.GetColumns(allTables[0]) });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetColumns(string table)
        {
            try
            {
                return Json(new { columns = _businessLayer.GetColumns(table) });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        //[HttpGet]
        //public IActionResult AddFilter(AddFilterViewModel addFilterViewModel)
        //{
        //    return Json(_businessLayer.GetColumnFilterByType(addFilterViewModel));
        //}

        [HttpGet]
        public IActionResult MainEntityDataFetch(EntityDataFetchModel model)
        {
            try
            {
                List<JQDTFriendlyColumnInfo> columnsForFrontEnd = new List<JQDTFriendlyColumnInfo>();
                DataTable dt = _businessLayer.GetTableCriteriaData(model, ref columnsForFrontEnd);

                string orderByCol = model.orderBy;
                int sortColIndex = 0;
                if (!string.IsNullOrEmpty(model.orderBy))
                {
                    sortColIndex = dt.Columns[orderByCol].Ordinal;
                }

                return Json(new
                {
                    recordsFiltered = dt.Rows.Count,
                    recordsTotal = dt.Rows.Count,
                    data = dt.JQDTFriendlyTableData(),
                    columns = columnsForFrontEnd,
                    sortColIndex = sortColIndex,
                    ascDesc = model.ascDesc.ToLower()
                });
            }
            catch(Exception ex)
            {
                return Json(new {error = ex.Message});
            }
        }


        [HttpPost]
        public IActionResult MainEntityRowSelect([FromBody] MainTableRowSelectModel model)
        {
            try
            {
                return Json(new { parentEntities = _businessLayer.GetParentEntities(model), childEntities = _businessLayer.GetChildEntities(model) });
            }
            catch(Exception ex)
            {
                return Json(new {error = ex.Message});
            }
        }

        [HttpPost]
        public IActionResult ParentOrChildGetData([FromBody] RelatedDataSelectModel model)
        {
            try
            {
                List<JQDTFriendlyColumnInfo> columnsForFrontEnd = new List<JQDTFriendlyColumnInfo>();
                DataTable dt = _businessLayer.GetParentOrChildData(model, ref columnsForFrontEnd);

                return Json(new
                {
                    recordsFiltered = dt.Rows.Count,
                    recordsTotal = dt.Rows.Count,
                    data = dt.JQDTFriendlyTableData(),
                    columns = columnsForFrontEnd,
                    sortColIndex = 0,
                    ascDesc = model.ascDesc.ToLower()
                });
            }
            catch(Exception ex)
            {
                return Json(new {error = ex.Message});
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
