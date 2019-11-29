using DataViewer.Lib;
using System;
using System.Collections.Generic;
using System.Data;


namespace DataViewer.Data
{
    public interface IDataAccess
    {
        DataTable GetData(string sql);
        List<string> GetColumns(string table);

        string GetColumnFilterByType(string table, string column, string filterOperator, string value);
    }

    public class DataAccess : IDataAccess
    {
        private protected string _connectionString;

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual List<string> GetColumns(string table)
        {
            throw new NotImplementedException();
        }

        public virtual string GetColumnFilterByType(string table, string column, string filterOperator, string value)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable GetData(string sql)
        {
            throw new NotImplementedException();
        }

    }

    public class DataSwitcher
    {
        public static IDataAccess DataAccessByDBMS(string connectionString, string dbms)
        {
            IDataAccess dataAccess;
            switch (dbms)
            {
                case AppConst.DBMS.SQLSERVER:
                    dataAccess = new SQLDataAccess(connectionString);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return dataAccess;
        }
    }
}
