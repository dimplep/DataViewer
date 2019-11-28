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
                case AppConst.DBMS.SQLSERVER:
                    dataAccess = new SQLDataAccess(connectionString);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return dataAccess;
        }
    }

    // static implementation of relations reading to avoid disk i/o
    public class Relations
    {
        private static DataTable _relations = null;
        private static readonly object lockobj = new object();

        public static DataTable GetRelations(string relationsJsonFileName)
        {
            string relationsJson;
            var fileStream = new FileStream(relationsJsonFileName, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                relationsJson = streamReader.ReadToEnd();
            }

            _relations = relationsJson.JsonToDatatable();
            return _relations;
        }
    }
}
