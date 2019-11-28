using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataViewer.Lib;
namespace DataViewer.Data
{
    // SQL SERVER DATA ACCESS
    public class SQLDataAccess : DataAccess
    {
        private string ColumnListSql = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'";

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
            string table = schemaTable;
            if (table.IndexOf(".") >= 0)
            {
                table = table.Substring(table.IndexOf(".") + 1);
            }

            string sql = String.Format(ColumnListSql, table);

            DataTable dt = SqlToDataTable(sql);
            List<string> columns = dt.ColumnToList<string>(0);
            return columns;
        }



    }
}
