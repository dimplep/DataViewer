using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Lib
{
    public static class DataTableExtensions
    {
        // Does jquery datatables friendly data conversion and creates a list
        public static List<dynamic> JQDTFriendlyTableData(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dict = (IDictionary<string, object>)dyn;

                    // convert data based on type
                    switch (Type.GetTypeCode(column.DataType))
                    {
                        case TypeCode.DateTime:
                            dict[column.ColumnName] = row.Field<DateTime>(column).ToString("G");
                            break;
                        default:
                            dict[column.ColumnName] = row[column];
                            break;
                    }

                }
            }
            return dynamicDt;
        }

        // returns jquery datatables friendly list of columns
        public static List<JQDTFriendlyColumnInfo> JQDTFriendlyColumnList(this DataTable dt)
        {
            List<JQDTFriendlyColumnInfo> list = new List<JQDTFriendlyColumnInfo>();
            foreach (DataColumn col in dt.Columns)
            {
                list.Add(new JQDTFriendlyColumnInfo(col.ColumnName));
            }
            return list;
        }
    }

    public class JQDTFriendlyColumnInfo
    {
        public string data { get; set; }
        public string name { get; set; }
        public JQDTFriendlyColumnInfo(string colName)
        {
            data = colName;
            name = colName;
        }
    }
}
