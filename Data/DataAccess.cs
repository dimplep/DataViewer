using DataViewer.Lib;
using System;
using System.Collections.Generic;
using System.Data;


namespace DataViewer.Data
{
    public interface IDataAccess
    {
        DataTable GetData(string sql);
        List<ColumnInfo> GetColumns(string table);
    }

    public class DataAccess : IDataAccess
    {
        private protected string _connectionString;

        public virtual List<string> TextCategoryKeywords
        {
            get
            {
                return new List<string> { "text", "char", "uniqueidentifier" };
            }
        }

        public virtual List<string> NumericCategoryKeywords
        {
            get
            {
                return new List<string> { "bit", "int", "money", "numeric", "decimal" };
            }
        }

        public virtual List<string> DateCategoryKeywords
        {
            get
            {
                return new List<string> { "date", "time" };
            }
        }

        public virtual string ColumnsAndTypesSql
        {
            get
            {
                return "";
            }
        }

        public virtual string PrimaryKeysSql
        {
            get
            {
                return "";
            }
        }

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DataTable SqlToDataTable(string sql)
        {
            throw new NotImplementedException();
        }

        // backend dependent

        public virtual List<ColumnInfo> GetColumns(string schemaTable)
        {
            // ASSUMED passed parameter has SCHEMA.TABLE name
            string table = JustTableName(schemaTable);

            string sql = String.Format(ColumnsAndTypesSql, table);
            string sql2 = String.Format(PrimaryKeysSql, table);

            DataTable dt = SqlToDataTable(sql);
            List<string> pkCols = SqlToDataTable(sql2).ColumnToList<string>(0);  // get pk columns to list
            List<ColumnInfo> columns = new List<ColumnInfo>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dataCategory = ColumnTypeToCategory(dt.Rows[i].Field<string>(1));

                if (dataCategory != AppConst.Category.OTHER)
                {
                    columns.Add(new ColumnInfo(dt.Rows[i].Field<string>(0), dataCategory, pkCols.Contains(dt.Rows[i].Field<string>(0))));
                }
            }

            return columns;
        }


        public virtual string JustTableName(string schemaTable)
        {
            string table = schemaTable;
            if (table.IndexOf(".") >= 0)
            {
                table = table.Substring(table.IndexOf(".") + 1);
            }
            return table;
        }

        public virtual DataTable GetData(string sql)
        {
            throw new NotImplementedException();
        }

        public virtual string ColumnTypeToCategory(string dataType)
        {
            string dataCategory;

            if (TextCategoryKeywords.Exists(listElement => dataType.Contains(listElement)))
            {
                dataCategory = AppConst.Category.TEXT;
            }
            else if (NumericCategoryKeywords.Exists(listElement => dataType.Contains(listElement)))
            {
                dataCategory = AppConst.Category.NUMERIC;
            }
            else if (DateCategoryKeywords.Exists(listElement => dataType.Contains(listElement)))
            {
                dataCategory = AppConst.Category.DATE;
            }
            else
            {
                dataCategory = AppConst.Category.OTHER;
            }

            return dataCategory;
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

    public class ColumnInfo
    {
        public string name { get; set; }
        public bool isPrimaryKey { get; set; }
        public string category { get; set; }       // basic category/group of data (e.g. text, date, number, logical)
        public ColumnInfo(string name, string category, bool isPrimaryKey)
        {
            this.name = name;
            this.category = category;
            this.isPrimaryKey = isPrimaryKey;
        }
    }
}
