using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Models
{
    public class MainTableDataFetchModel
    {
        public string table { set; get; }
        public string criteria { set; get; }
        public int topN { set; get; }
    }
}
