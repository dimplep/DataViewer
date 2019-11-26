using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Lib
{

    public class DataTableLib
    {
        public bool IsNumericColumn(TypeCode typeCode)
        {
            return (typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64
                || typeCode == TypeCode.Byte || typeCode == TypeCode.Decimal || typeCode == TypeCode.Double
                || typeCode == TypeCode.Single);
        }
    }

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
            DataTableLib dtLib = new DataTableLib();
            List<JQDTFriendlyColumnInfo> list = new List<JQDTFriendlyColumnInfo>();
            foreach (DataColumn col in dt.Columns)
            {
                if (dtLib.IsNumericColumn(Type.GetTypeCode(col.DataType)))
                {
                    list.Add(new JQDTFriendlyColumnInfo(col.ColumnName, AppConst.JQDT_COL_ALIGN.RIGHT));
                }
                else
                {
                    list.Add(new JQDTFriendlyColumnInfo(col.ColumnName));
                }
                
            }
            return list;
        }
    }

    public class JQDTFriendlyColumnInfo
    {
        public string data { get; set; }
        public string name { get; set; }
        public string className { get; set; }
        public JQDTFriendlyColumnInfo(string colName, string colAlignment = AppConst.JQDT_COL_ALIGN.LEFT)
        {
            data = colName;
            name = colName;
            className = colAlignment;
        }
    }
}
