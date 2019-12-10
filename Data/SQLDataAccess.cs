using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataViewer.Lib;
using Microsoft.SqlServer.Types;

namespace DataViewer.Data
{
    // SQL SERVER DATA ACCESS
    public class SQLDataAccess : DataAccess
    {
        public override string ColumnsAndTypesSql
        {
            get
            {
                return "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'";
            }
        }

        public override string PrimaryKeysSql

        {
            get
            {
                // , Col.DATA_TYPE 
                return "SELECT Col.COLUMN_NAME " +
                    "from INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab " +
                    "inner join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Usage " +
                    "on Usage.CONSTRAINT_NAME = Tab.CONSTRAINT_NAME " +
                    "inner join INFORMATION_SCHEMA.COLUMNS Col " +
                    "on Usage.TABLE_NAME = Col.TABLE_NAME AND Usage.COLUMN_NAME = Col.COLUMN_NAME " +
                    "WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' " +
                    "AND Col.TABLE_NAME = '{0}' ";
            }
        }


        public SQLDataAccess(string connectionString) : base(connectionString)
        {
        }

        public override DataTable GetData(string sql)
        {
            return SqlToDataTable(sql);
        }

        public override DataTable SqlToDataTable(string sql)
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
                cmd.Dispose();
                conn.Close();
                da.Dispose();
            }

            return dt;
        }

    }
}
