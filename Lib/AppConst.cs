using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Lib
{
    public class AppConst
    {
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
        }
    }
}
