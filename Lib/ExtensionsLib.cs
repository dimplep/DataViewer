using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Lib
{
    public static class ExtensionsLib
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
                if (Type.GetTypeCode(col.DataType).IsNumericColumn())
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

        public static List<T> ColumnToList<T>(this DataTable dt, int colIndex)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = row.Field<T>(colIndex);
                data.Add(item);
            }
            return data;
        }

        // returns data table from json
        public static DataTable JsonToDatatable(this String jsonString)
        {
            var jsonArray = JArray.Parse(jsonString);

            return JsonConvert.DeserializeObject<DataTable>(jsonArray.ToString());
        }

        public static bool IsNumericColumn(this TypeCode typeCode)
        {
            return (typeCode == TypeCode.Int16 || typeCode == TypeCode.Int32 || typeCode == TypeCode.Int64
                || typeCode == TypeCode.Byte || typeCode == TypeCode.Decimal || typeCode == TypeCode.Double
                || typeCode == TypeCode.Single);
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

