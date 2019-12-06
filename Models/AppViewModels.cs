using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Models
{
    public class MainTableDataFetchModel
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
        public int topN { set; get; }
    }

    public class MainTableRowSelectModel
    {
        public string table { get; set; }
        public List<ColNameValueModel> colNameVals { get; set; }

    }

    public class RelatedDataSelectModel
    {
        public string fromEntity { get; set; }
        public string toEntity { get; set; }
        public string toEntityType { get; set; }
        public List<ColNameValueModel> keyVals { get; set; }
        public int topN { get; set; }
    }

    public class ColNameValueModel
    {
        public string colName { set; get; }
        public string colValue { set; get; }

    }

}
