using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataViewer.Lib;

namespace DataViewer.Models
{
    public class EntityDataFetchModel
    {
        public string table { set; get; }
        private string _criteria;
        public string criteria 
        { 
            set
            {
                _criteria = (value == null ? "" : value.Trim());
            }
            get
            {
                return _criteria;
            }
        }

        public string orderBy { set; get; }
        public string ascDesc { set; get; }

        public int topN { set; get; }
    }

    public class MainTableRowSelectModel
    {
        public string table { get; set; }
        public List<ColNameValueModel> colNameVals { get; set; }
        public bool hideChilEntitiesWhenNoData { get; set; }
    }

    public class RelatedDataSelectModel
    {
        public string fromEntity { get; set; }
        public string fromEntityForeignKey { get; set; }        // used internally in backend only to determine results in case there are multiple fks for same parent
        public string toEntity { get; set; }
        public string toEntityType { get; set; }
        public List<ColNameValueModel> keyVals { get; set; }
        public string ascDesc { set; get; }
        public int topN { get; set; }
    }

    public class ColNameValueModel
    {
        public string colName { set; get; }
        public string colValue { set; get; }

    }

    public class JQDTFriendlyColumnInfo
    {
        public string name { get; set; }
        public string data { get; set; }
        public bool isPrimaryKey { get; set; }
        public string category { get; set; }
        public string className { get; set; }
        public JQDTFriendlyColumnInfo(string name, string category, bool isPrimaryKey = false, string colAlignment = AppConst.JQDT_COL_ALIGN.LEFT)
        {
            this.name = data = name;
            this.category = category;
            className = colAlignment;
            this.isPrimaryKey = isPrimaryKey;
        }
    }


}
