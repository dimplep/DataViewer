using DataViewer.Models;
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
        //
        // returns count of Occurances
        public static int Occurance(this string str, string value)
        {
            int occurances = 0;
            int index = 0;
            if (!String.IsNullOrEmpty(value))
            {
                while (index >= 0)
                {
                    index = str.IndexOf(value, index);
                    if (index >= 0)
                    {
                        occurances++;
                        index += value.Length;
                    }
                }
            }
            return occurances;
        }
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
                            if (row[column] != DBNull.Value)
                            {
                                dict[column.ColumnName] = row.Field<DateTime>(column).ToString("G");
                            }
                            else
                            {
                                dict[column.ColumnName] = "";
                            }
                            break;
                        case TypeCode.Boolean:
                            dict[column.ColumnName] = (row.Field<bool>(column) ? 1 : 0);
                            break;
                        default:
                            dict[column.ColumnName] = row[column];
                            break;
                    }

                }
            }
            return dynamicDt;
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
}

