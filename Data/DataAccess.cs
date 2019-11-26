using DataViewer.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Data
{
    public interface IDataAccess
    {
        DataTable GetData(string sql);
    }

    public class DataAccess : IDataAccess
    {
        private protected string _connectionString;
        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        // need to implement at child level
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
                case AppConst.DBMS.MYSQL:
                    dataAccess = new SQLDataAccess(connectionString);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return dataAccess;
        }
    }

}
