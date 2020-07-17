using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Shared.ExcelGenerator
{
    public static class ExcelGenerator
    {
        public static byte[] ToExcel<T>(List<T> Data) where T : class
        {
            // check data provided

            if (!(Data != null && Data.Count > 0))
                throw new InvalidOperationException("data cannot be null or empty");

            // check if type has attribute data export attribute
            var hasExport = typeof(T).GetCustomAttributes(typeof(DataExport), true).Any();
            bool hasIndex = false;
            string sheetName = null;

            if (hasExport)
            {
                var attr = (Attribute.GetCustomAttribute(typeof(T), typeof(DataExport)) as DataExport);
                hasIndex = attr.AddIndex;
                sheetName = attr.SheetName;
            }

            //DataTable table = new DataTable();

            // add columns
            // add index column if available
            var tableheaders = new List<string>();
            if (hasIndex)
            {
                tableheaders.Add("Index");
               // table.Columns.Add("Index", typeof(Int32));
            }

            // add property columns
            var addedProperties = new List<string>();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (var prop in typeof(T).GetProperties())
            {
                var ignoreAttr = Attribute.GetCustomAttribute(prop, typeof(ExportCellIgnore));
                var nameAttr = Attribute.GetCustomAttribute(prop, typeof(ExportCell)) as ExportCell;

                if (ignoreAttr == null)
                {
                    var headerName = nameAttr == null ? prop.Name : nameAttr.HeaderName;
                   // table.Columns.Add(prop.Name, typeof(string));
                    addedProperties.Add(prop.Name);
                    tableheaders.Add(headerName);
                }


            }

            // add report data
            int j = 1;
            var dictionaryData = new List<Dictionary<string, string>>();
            foreach (T item in Data)
            {
                var data = new Dictionary<string, string>();
                foreach (var prop in addedProperties)
                {
                    data.Add(prop, properties[prop].GetValue(item)?.ToString() ?? "");
                }

                dictionaryData.Add(data);

                j++;
            }

            var workbook = new HSSFWorkbook();
            var sheet = sheetName == null ? workbook.CreateSheet() : workbook.CreateSheet(sheetName);

           
            // Set header name this header use for set name in excel first row  
            var headers = tableheaders.ToArray();

            var headerRow = sheet.CreateRow(0);

            //Below loop is create header  
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
            }

            //Below loop is fill content  
            for (int i = 0; i < dictionaryData.Count; i++)
            {
                var rowIndex = i + 1;
                var row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(rowIndex.ToString());
                for (int z = 0; z < addedProperties.Count; z++)
                {
                    var cell = row.CreateCell(z + 1);
                    var o = dictionaryData[i];
                    cell.SetCellValue(o[addedProperties[z]]);
                }
            }

            byte[] returnFile;
            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                returnFile = exportData.ToArray();
            }

            return returnFile;
        }

    }
}
