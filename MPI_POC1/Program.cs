using MPI_POC1.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPI_POC1
{
    class Program
    {

        static void Main(string[] args)
        {
            OrderProcess _Process = new OrderProcess();

            string strAccessToken = _Process.GetAUTHToken("LIVE");
            if (!(String.IsNullOrEmpty(strAccessToken)))
            {
                bool HasMore = false;
                do
                {
                    object details = _Process.GetOrderDetails("LIVE", strAccessToken, out HasMore);
                } while (HasMore != false);
            }
            Console.Read();
        }
        
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public static void WriteToCsvFile1(DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);



            foreach (DataRow dr in dataTable.Rows)
            {

                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            //System.IO.File.WriteAllText("C:\\DilipWorkSpace\\MISC\\PROCVIEW\\PROCVIEW\\CSVFiledump.csv", fileContent.ToString());

            StreamWriter sw = new StreamWriter("C:\\DilipWorkSpace\\MISC\\PROCVIEW\\PROCVIEW\\CSVFiledump.csv", true);
            sw.Write(fileContent.ToString());
            sw.Close();
            
        }

        public static void WriteToCsvFile2(DataTable dataTable, string filePath) {
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns) {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);



            foreach (DataRow dr in dataTable.Rows) {

                foreach (var column in dr.ItemArray) {
                    fileContent.Append("\"" + column.ToString() + "\",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            System.IO.File.WriteAllText(filePath, fileContent.ToString());

        }
    }
}
