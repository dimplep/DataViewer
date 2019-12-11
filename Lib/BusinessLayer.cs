using DataViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using static DataViewer.Lib.AppConst;
using System.IO;
using System.Text;
using DataViewer.Models;

namespace DataViewer.Lib
{
    public class BusinessLayer
    {
        private IDataAccess _dataAccess;
        private DataTable _relations;
        public BusinessLayer(IDataAccess dataAccess, string relationsJsonFileName)
        {
            _dataAccess = dataAccess;
            _relations = Relations.GetRelations(relationsJsonFileName);
        }

        public List<string> AllTables()
        {
            List<string> list = ((from row in _relations.AsEnumerable()
                           select row.Field<string>(RelationDataColumns.PARENT_TABLE))
                           .Union(from row in _relations.AsEnumerable()
                                  select row.Field<string>(RelationDataColumns.CHILD_TABLE))).Distinct().OrderBy(o => o).ToList();

            return list;
        }

        public List<string> FilterOperators()
        {
            return new List<string> { Operators.EQUAL, Operators.GREATER_THAN, Operators.GREATER_THAN_EQUAL, Operators.LESS_THAN, 
                                    Operators.LESS_THAN_EQUAL, Operators.NOT_EQUAL, Operators.IS_NULL, Operators.IS_NOT_NULL,
                                    Operators.IN, Operators.NOT_IN, Operators.LIKE};
        }

        public List<JQDTFriendlyColumnInfo> GetColumns(string table)
        {
            return _dataAccess.GetColumns(table);
        }

        // gives list of html/json compatible comma delimited columns
        public string CompatibleColumnsForSelect(string table)
        {
            string columns = "";
            foreach(JQDTFriendlyColumnInfo item in  GetColumns(table))
            {
                columns += (columns != "" ? ", " : "") + item.name;
            }
            return columns;
        }

        public DataTable GetTableCriteriaData(string entityName, string criteria, int topN, ref List<JQDTFriendlyColumnInfo> columnsForFrontEnd)
        {
            // columnsForFrontEnd is filled by refernce
            //columns = dt.JQDTFriendlyColumnList();
            //List<ColumnInfo> cols = GetColumns(table);

            string sql = _dataAccess.BuildBasicSql("*", entityName, criteria, topN);

            if (sql.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').ToLower().Occurance(" from ") > 1)
            {
                throw new Exception("Invalid SQL");     // poor man's sql injection prevention
            }

            return EntitySqlToDtForFrontEnd(entityName, sql, ref columnsForFrontEnd);
        }

        private DataTable EntitySqlToDtForFrontEnd(string entityName, string sql, ref List<JQDTFriendlyColumnInfo> columnsForFrontEnd)
        {
            DataTable dt = _dataAccess.GetData(sql);
            // set primary key columns in columnsForFrontEnd
            List<JQDTFriendlyColumnInfo> dbCols = _dataAccess.GetColumns(entityName);
            columnsForFrontEnd = dbCols;
            return dt;
        }

        //// returns jquery datatables friendly list of columns
        //private List<JQDTFriendlyColumnInfo> JQDTFriendlyColumnList(DataTable dt)
        //{
        //    List<JQDTFriendlyColumnInfo> list = new List<JQDTFriendlyColumnInfo>();
        //    foreach (DataColumn col in dt.Columns)
        //    {
        //        if (Type.GetTypeCode(col.DataType).IsNumericColumn())
        //        {
        //            list.Add(new JQDTFriendlyColumnInfo(col.ColumnName, ColumnTypeToCategory(col.DataType) AppConst.JQDT_COL_ALIGN.RIGHT));
        //        }
        //        else
        //        {
        //            list.Add(new JQDTFriendlyColumnInfo(col.ColumnName));
        //        }

        //    }
        //    return list;
        //}


        public DataTable GetParentOrChildData(RelatedDataSelectModel model, ref List<JQDTFriendlyColumnInfo> columnsForFrontEnd)
        {
            List<JQDTFriendlyColumnInfo> fromEntityCols = _dataAccess.GetColumns(model.fromEntity);
            List<JQDTFriendlyColumnInfo> toEntityCols = _dataAccess.GetColumns(model.toEntity);
            string criteria = _dataAccess.ColNameValToCriteria(model.keyVals, fromEntityCols);
            string sql = _dataAccess.BuildBasicSql("*", model.fromEntity, criteria, 1);
            DataTable dt = _dataAccess.GetData(sql);        // get from table row for passed pk values

            List<ColNameValueModel> colVals;

            if (model.toEntityType == RelationType.PARENT)
            {
                colVals = ChildToParentColVals(model, dt);
            }
            else
            {
                colVals = ParentToChildColVals(model, dt);
            }

            string toCiteria = _dataAccess.ColNameValToCriteria(colVals, toEntityCols);
            sql = _dataAccess.BuildBasicSql("*", model.toEntity, toCiteria, model.topN);
            DataTable toDt = EntitySqlToDtForFrontEnd(model.toEntity, sql, ref columnsForFrontEnd);

            return toDt;
        }

        private List<ColNameValueModel> ChildToParentColVals(RelatedDataSelectModel model, DataTable childTable)
        {
            List<ColNameValueModel> colVals = new List<ColNameValueModel>();
            foreach (var match in _relations.AsEnumerable()
                .Where(r => r.Field<string>(RelationDataColumns.CHILD_TABLE) == model.fromEntity
                        && r.Field<string>(RelationDataColumns.PARENT_TABLE) == model.toEntity))
            {
                string cNm = match.Field<string>(RelationDataColumns.PARENT_KEY);
                string cVal = childTable.Rows[0][match.Field<string>(RelationDataColumns.CHILD_KEY)].ToString();

                if (!colVals.Any(o => o.colName == cNm && o.colValue == cVal))
                {
                    colVals.Add(new ColNameValueModel{colName = cNm, colValue = cVal});
                }
            }
            
            return colVals;     
        }

        private List<ColNameValueModel> ParentToChildColVals(RelatedDataSelectModel model, DataTable parentTable)
        {
            List<ColNameValueModel> colVals = new List<ColNameValueModel>();
            foreach (var match in _relations.AsEnumerable()
                .Where(r => r.Field<string>(RelationDataColumns.PARENT_TABLE) == model.fromEntity
                        && r.Field<string>(RelationDataColumns.CHILD_TABLE) == model.toEntity))
            {
                // search by child key but set parent key because searching in parent
                colVals.Add(new ColNameValueModel
                {
                    colName = match.Field<string>(RelationDataColumns.CHILD_KEY),
                    colValue = parentTable.Rows[0][match.Field<string>(RelationDataColumns.PARENT_KEY)].ToString()
                });
            }
            return colVals;
        }

        // find entities in relations where given entity's primary key is being used as foreign key
        public List<string> GetChildEntities(MainTableRowSelectModel model)
        {
            List<string> allChildEntities = _relations.AsEnumerable()
                                            .Where(r => r.Field<string>(RelationDataColumns.PARENT_TABLE) == model.table)
                                            .Select(r => r.Field<string>(RelationDataColumns.CHILD_TABLE)).Distinct().OrderBy(o => o).ToList();

            List<string> childEntities = new List<string>();
            if (model.hideChilEntitiesWhenNoData)
            {
                // if need to hide child entities when no related data found, query each child entities to check if data exists
                foreach (string childEntity in allChildEntities)
                {
                    RelatedDataSelectModel fakeModel = new RelatedDataSelectModel { topN = 1, fromEntity = model.table, toEntity = childEntity,
                                                                                toEntityType = RelationType.CHILD, keyVals = model.colNameVals };
                    List<JQDTFriendlyColumnInfo> columnsForFrontEnd = new List<JQDTFriendlyColumnInfo>();
                    DataTable dt = GetParentOrChildData(fakeModel, ref columnsForFrontEnd);

                    if (dt.Rows.Count > 0)
                    {
                        childEntities.Add(childEntity);
                    }
                }
            }
            else
            {
                childEntities = allChildEntities;
            }

            return childEntities;
        }

        // find entities in relations where given entity contains foreign keys
        public List<string> GetParentEntities(MainTableRowSelectModel model)
        {
            // get current maintable row
            List<JQDTFriendlyColumnInfo> entityCols = _dataAccess.GetColumns(model.table);
            string criteria = _dataAccess.ColNameValToCriteria(model.colNameVals, entityCols);
            string sql = _dataAccess.BuildBasicSql("*", model.table, criteria, 1);
            DataTable dt = _dataAccess.GetData(sql);

            // for each foreignkeys make sure value is not null and then add to list
            List<string> parents = new List<string>();
            foreach(var relationRow in _relations.AsEnumerable()
                .Where(r => r.Field<string>(RelationDataColumns.CHILD_TABLE) == model.table))
            {
                string fkColName = relationRow.Field<string>(RelationDataColumns.CHILD_KEY);

                if (dt.Rows[0][fkColName] != DBNull.Value)
                {
                    parents.Add(relationRow.Field<string>(RelationDataColumns.PARENT_TABLE));
                }
            }

            return parents.Distinct().ToList();
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
