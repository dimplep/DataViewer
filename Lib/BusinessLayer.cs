using DataViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace DataViewer.Lib
{
    public class BusinessLayer
    {
        private DataTable ConvertJsonToDatatable(string jsonString)
        {
            var jsonLinq = JObject.Parse(jsonString);

            // Find the first array using Linq
            var linqArray = jsonLinq.Descendants().Where(x => x is JArray).First();
            var jsonArray = new JArray();
            foreach (JObject row in linqArray.Children<JObject>())
            {
                var createRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        createRow.Add(column.Name, column.Value);
                    }
                }
                jsonArray.Add(createRow);
            }

            return JsonConvert.DeserializeObject<DataTable>(jsonArray.ToString());
        }

    }
}
