using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataViewer.Data
{
    // SQL SERVER DATA ACCESS
    public class SQLDataAccess : DataAccess
    {
        public SQLDataAccess(string connectionString) : base(connectionString)
        {
        }

        public override DataTable GetData(string sql)
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
    }
}
