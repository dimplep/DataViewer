using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Lib
{
    public class AppConst
    {
        public const int DEFAULT_TOP_N = 100;
        public const string PARENT_TABLE_FK_SEPARATOR = " -> ";
        public static class JQDT_COL_ALIGN
        {
            public const string LEFT = "text-left";
            public const string RIGHT = "text-right";
        }

        public static class DBMS
        {
            public const string SQLSERVER = "SQL";
            public const string MYSQL = "MYSQL";
        }

        public static class RelationDataColumns
        {
            public const string CHILD_TABLE = "ChildTable";
            public const string CHILD_KEY = "ChildKey";
            public const string PARENT_TABLE = "ParentTable";
            public const string PARENT_KEY = "PARENTKEY";
        }

        public static class Operators
        {
            public const string EQUAL = "=";
            public const string GREATER_THAN = ">";
            public const string GREATER_THAN_EQUAL = ">=";
            public const string LESS_THAN = "<";
            public const string LESS_THAN_EQUAL = "<=";
            public const string NOT_EQUAL = "<>";
            public const string IS_NULL = "Is Null";
            public const string IS_NOT_NULL = "Is Not Null";
            public const string IN = "In";
            public const string NOT_IN = "Not In";
            public const string LIKE = "Like";
        }

        public static class Category
        {
            public const string TEXT = "text";      // text, str, char
            public const string NUMERIC = "numeric";  // int, decimal, float, bit
            public const string DATE = "date";      // date, datetime, time
            public const string BOOLEAN = "bool";      // date, datetime, time
            public const string OTHER = "other";      // Any other column types than text, number, date should be excluded
        }

        public static class RelationType
        {
            public const string PARENT = "Parent";
            public const string CHILD = "Child";

        }
    }
}
