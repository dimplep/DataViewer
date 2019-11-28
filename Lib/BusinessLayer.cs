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
                           select row.Field<string>(RelationDataColumns.PARENTTABLE))
                           .Union(from row in _relations.AsEnumerable()
                                  select row.Field<string>(RelationDataColumns.CHILDTABLE))).Distinct().OrderBy(o => o).ToList();

            return list;
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
