using DataViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using static DataViewer.Lib.AppConst;

namespace DataViewer.Lib
{
    public class BusinessLayer
    {
        private IDataAccess _dataAccess;
        private DataTable _relations;

        public BusinessLayer(IDataAccess dataAccess, DataTable relations)
        {
            _dataAccess = dataAccess;
            _relations = relations;
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
}
