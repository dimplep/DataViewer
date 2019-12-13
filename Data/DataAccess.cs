using DataViewer.Lib;
using DataViewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DataViewer.Data
{
    public interface IDataAccess
    {
        DataTable GetData(string sql);
        List<JQDTFriendlyColumnInfo> GetColumns(string table);
        string ColNameValToCriteria(List<ColNameValueModel> colNameVals, List<JQDTFriendlyColumnInfo> cols);
        string BuildBasicSql(string cols, string from, string where, string orderBy, string ascDesc, int topN);
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
                return new List<string> { "int", "money", "numeric", "decimal" };
            }
        }

        public virtual List<string> BooleanCategoryKeywords
        {
            get
            {
                return new List<string> { "bit" };
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

        public virtual List<JQDTFriendlyColumnInfo> GetColumns(string schemaTable)
        {
            // ASSUMED passed parameter has SCHEMA.TABLE name
            string table = JustTableName(schemaTable);

            string sql = String.Format(ColumnsAndTypesSql, table);
            string sql2 = String.Format(PrimaryKeysSql, table);

            DataTable dt = SqlToDataTable(sql);
            List<string> pkCols = SqlToDataTable(sql2).ColumnToList<string>(0);  // get pk columns to list
            List<JQDTFriendlyColumnInfo> columns = new List<JQDTFriendlyColumnInfo>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dataCategory = ColumnTypeToCategory(dt.Rows[i].Field<string>(1));

                if (dataCategory != AppConst.Category.OTHER)
                {
                    string align = (dataCategory != AppConst.Category.NUMERIC ? AppConst.JQDT_COL_ALIGN.RIGHT : AppConst.JQDT_COL_ALIGN.LEFT);
                    columns.Add(new JQDTFriendlyColumnInfo(dt.Rows[i].Field<string>(0), dataCategory, pkCols.Contains(dt.Rows[i].Field<string>(0)), align));
                }
            }

            return columns;
        }

        public virtual string BuildBasicSql(string cols, string from, string where, string orderBy, string ascDesc, int topN)
        {
            string sql = "SELECT TOP " + (topN > 0 ? topN : AppConst.DEFAULT_TOP_N) + " " + cols
                            + " FROM " + from + (where != "" ? " WHERE " + where : "");
            
            if (!string.IsNullOrEmpty(orderBy))
            {
                string order = " ORDER BY " + orderBy + " " + (string.IsNullOrEmpty(ascDesc) ? "DESC" : ascDesc);
                sql += " " + order;
            }

            return sql;

        }


        public virtual string JustTableName(string schemaTable)
        {
            string table = schemaTable;
            if (table.IndexOf(".", StringComparison.Ordinal) >= 0)
            {
                table = table.Substring(table.IndexOf(".", StringComparison.Ordinal) + 1);
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
            else if (BooleanCategoryKeywords.Exists(listElement => dataType.Contains(listElement)))
            {
                dataCategory = AppConst.Category.BOOLEAN;
            }
            else
            {
                dataCategory = AppConst.Category.OTHER;
            }

            return dataCategory;
        }

        // returns criteria string (that can used in where)
        public virtual string ColNameValToCriteria(List<ColNameValueModel> colNameVals, List<JQDTFriendlyColumnInfo> cols)
        {
            string criteria = "";
            foreach(ColNameValueModel pair in colNameVals)
            {
                string newCriteria = "";
                if (cols.First(o => o.name == pair.colName).category == AppConst.Category.NUMERIC)
                {
                    newCriteria = pair.colName + " = " + pair.colValue;
                }
                else
                {
                    newCriteria = pair.colName + " = '" + pair.colValue + "'";
                }
                criteria += (criteria != "" ? " AND " : "") + newCriteria;
            }
            return criteria;
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
                case AppConst.DBMS.MYSQL:
                    dataAccess = new MYSQLDataAccess(connectionString);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return dataAccess;
        }
    }
}
