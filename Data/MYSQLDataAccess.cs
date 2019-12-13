using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using DataViewer.Lib;

//using Microsoft.SqlServer.Types;

namespace DataViewer.Data
{
    // SQL SERVER DATA ACCESS
    public class MYSQLDataAccess : SQLDataAccess
    {
        public override string PrimaryKeysSql

        {
            get
            {

                return "SELECT k.column_name " +
                        "FROM information_schema.table_constraints t " +
                        "JOIN information_schema.key_column_usage k " +
                        "USING(constraint_name, table_schema, table_name) " +
                        "WHERE t.constraint_type = 'PRIMARY KEY' " +
                          "AND t.table_name = '{0}'";
            }
        }


        public MYSQLDataAccess(string connectionString) : base(connectionString)
        {
        }

        public override DataTable GetData(string sql)
        {
            return SqlToDataTable(sql);
        }

        public override string BuildBasicSql(string cols, string from, string where, string orderBy, string ascDesc, int topN)
        {
            string sql = "SELECT " + cols + " FROM " + from
                + (where != "" ? " WHERE " + where : "");

            if (!string.IsNullOrEmpty(orderBy))
            {
                string order = " ORDER BY " + orderBy + " " + (string.IsNullOrEmpty(ascDesc) ? "DESC" : ascDesc);
                sql += " " + order;
            }

            sql += " LIMIT " + (topN > 0 ? topN : AppConst.DEFAULT_TOP_N);

            return sql;

        }

        public override DataTable SqlToDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();

                // create data adapter
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
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
