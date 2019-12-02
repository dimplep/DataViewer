using DataViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using static DataViewer.Lib.AppConst;
using System.IO;
using System.Text;
using DataViewer.Models;

namespace DataViewer.Lib
{
    public class BusinessLayer
    {
        private IDataAccess _dataAccess;
        private DataTable _relations;
        public BusinessLayer(IDataAccess dataAccess, string relationsJsonFileName)
        {
            _dataAccess = dataAccess;
            _relations = Relations.GetRelations(relationsJsonFileName);
        }

        public List<string> AllTables()
        {
            List<string> list = ((from row in _relations.AsEnumerable()
                           select row.Field<string>(RelationDataColumns.PARENT_TABLE))
                           .Union(from row in _relations.AsEnumerable()
                                  select row.Field<string>(RelationDataColumns.CHILD_TABLE))).Distinct().OrderBy(o => o).ToList();

            return list;
        }

        public List<string> FilterOperators()
        {
            return new List<string> { Operators.EQUAL, Operators.GREATER_THAN, Operators.GREATER_THAN_EQUAL, Operators.LESS_THAN, 
                                    Operators.LESS_THAN_EQUAL, Operators.NOT_EQUAL, Operators.IS_NULL, Operators.IS_NOT_NULL,
                                    Operators.IN, Operators.NOT_IN, Operators.LIKE};
        }

        public List<ColumnInfo> GetColumns(string table)
        {
            return _dataAccess.GetColumns(table);
        }

        // gives list of html/json compatible comma delimited columns
        public string CompatibleColumnsForSelect(string table)
        {
            string columns = "";
            foreach(ColumnInfo item in  GetColumns(table))
            {
                columns += (columns != "" ? ", " : "") + item.name;
            }
            return columns;
        }

        public DataTable GetTableCriteriaData(string table, string criteria, int topN)
        {
            List<ColumnInfo> cols = GetColumns(table);

            string sql = "SELECT TOP " + (topN > 0 ? topN : DEFAULT_TOP_N) + " " + CompatibleColumnsForSelect(table)  + " FROM " + table + (criteria != "" ? " WHERE " + criteria : "");
            
            if (sql.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').ToLower().Occurance(" from ") > 1)
            {
                throw new Exception("Invalid SQL");     // poor man's sql injection prevention
            }

            return _dataAccess.GetData(sql);
        }


    }

    // static implementation of relations reading to avoid disk i/o
    public class Relations
    {
        private static DataTable _relations = null;
        private static readonly object lockobj = new object();

        public static DataTable GetRelations(string relationsJsonFileName)
        {
            string relationsJson;
            var fileStream = new FileStream(relationsJsonFileName, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                relationsJson = streamReader.ReadToEnd();
            }

            _relations = relationsJson.JsonToDatatable();
            return _relations;
        }
    }

}
