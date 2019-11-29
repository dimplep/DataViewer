using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataViewer.Models
{
    public class AddFilterViewModel
    {
        public string table { get; set; }
        public string column { get; set; }
        public string filterOperator { get; set; }
        public string newFilter { get; set; }

        private string _currentFilters;
        public string currentFilters 
        { 
            get
            {
                return _currentFilters;
            }

            set
            {
                if (value == null)
                {
                    _currentFilters = "";
                }
                else
                {
                    _currentFilters = value;
                }

            }
        }
    }
}
