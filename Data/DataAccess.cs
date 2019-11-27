using DataViewer.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
        private protected DataTable _relations;
        public DataAccess(string connectionString, string relationsJsonFileName)
        {
            _connectionString = connectionString;

            string relationsJson;
            var fileStream = new FileStream(relationsJsonFileName, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                relationsJson = streamReader.ReadToEnd();
            }

            _relations = relationsJson.JsonToDatatable();
        }

        // need to implement at child level
        public virtual DataTable GetData(string sql)
        {
            throw new NotImplementedException();
        }

    }

    public class DataSwitcher
    {
        public static IDataAccess DataAccessByDBMS(string connectionString, string dbms, string relationsJsonFileName)
        {
            IDataAccess dataAccess;
            switch (dbms)
            {
                case AppConst.DBMS.SQLSERVER:
                    dataAccess = new SQLDataAccess(connectionString, relationsJsonFileName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return dataAccess;
        }
    }
}
