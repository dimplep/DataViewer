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
            public const string CHILDTABLE = "ChildTable";
            public const string CHILDKEY = "ChildKey";
            public const string PARENTTABLE = "ParentTable";
            public const string PARENTKEY = "PARENTKEY";
        }
    }
}
