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

    public class TestModel
    {
        public List<PrimaryKeyModel> list;
    }
    public class PrimaryKeyModel
    {
        public string name { set; get; }
        public string value { set; get; }

    }

}
