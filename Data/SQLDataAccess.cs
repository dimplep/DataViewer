using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataViewer.Lib;
using static DataViewer.Lib.AppConst;

namespace DataViewer.Data
{
    // SQL SERVER DATA ACCESS
    public class SQLDataAccess : DataAccess
    {
        private string ColumnListSql = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'";
        private string ColumnDataTypeSql = "SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = '{1}'";

        public SQLDataAccess(string connectionString) : base(connectionString)
        {
        }

        public override DataTable GetData(string sql)
        {
            return SqlToDataTable(sql);
        }

        private DataTable SqlToDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dt);
                conn.Close();
                da.Dispose();
            }

            return dt;
        }

        public override List<string> GetColumns(string schemaTable)
        {
            // ASSUMED passed parameter has SCHEMA.TABLE name
            string table = RemoveSchema(schemaTable);

            string sql = String.Format(ColumnListSql, table);

            DataTable dt = SqlToDataTable(sql);
            List<string> columns = dt.ColumnToList<string>(0);
            return columns;
        }

        private string RemoveSchema(string schemaTable)
        {
            string table = schemaTable;
            if (table.IndexOf(".") >= 0)
            {
                table = table.Substring(table.IndexOf(".") + 1);
            }
            return table;
        }

        public override string GetColumnFilterByType(string schemaTable, string column, string filterOperator, string value)
        {
            
            string retVal;
            if (filterOperator == Operators.IS_NULL || filterOperator == Operators.IS_NOT_NULL)
            {
                retVal = column + " " + filterOperator;
            }
            else if (filterOperator == Operators.IN || filterOperator == Operators.NOT_IN)
            {
                retVal = column + " " + filterOperator + " " + value;       // user must enter values corretcly wrapped
            }
            else
            {
                string table = RemoveSchema(schemaTable);
                string sql = String.Format(ColumnDataTypeSql, table, column);
                DataTable dt = SqlToDataTable(sql);
                string dataType = dt.Rows[0][0].ToString();

                retVal = column + " " + filterOperator + TypeCompatibleStaticValue(value, dataType);
            }

            return retVal;
        }

        private string TypeCompatibleStaticValue(string value, string dataType)
        {
            // returns sql compatible static value by data type (e.g. John will be returned as 'John', 2.2 will be returned as 2.2)
            string retVal = value;
            if (WrapStaticValueInQuotes(dataType))
            {
                retVal = "'" + value + "'";
            }
            return retVal;
        }

        private bool WrapStaticValueInQuotes(string dataType)
        {
            return (dataType.IndexOf("date") >= 0 || dataType.IndexOf("time") >= 0 || dataType.IndexOf("char") >= 0 || dataType.IndexOf("text") >= 0);

        }



    }
}
