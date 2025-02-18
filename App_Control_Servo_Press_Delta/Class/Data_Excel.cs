
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOPath = System.IO.Path;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using OfficeOpenXml;
using System.Windows;
using System;
using System.IO.Packaging;
using OxyPlot.Series;
using System.Windows.Shapes;
using OxyPlot;
using System.Text.Json;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta.Class
{
    public class Excel
    {
        public class FilePath
        {
            public string Path { get; set; }
        }
        Auto auto_screen = new Auto();
        Link_Path linkpath = new Link_Path();
        public void Import_Model_Filepath()
        {
            string filePath;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    Import_Model(filePath);
                }
            }
        }
        public void Import_Model(string filePath)
        {
            string Jsontemp;
            string Json_new = "";
            List_Model List_Model = new List_Model();
            // Kiểm tra xem file có tồn tại không
            if (!File.Exists(filePath))
            {
                System.Windows.MessageBox.Show("File không tồn tại.");
                return;
            }
            // Sử dụng EPPlus để đọc file Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Lấy worksheet đầu tiên
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet.Cells[1, 10].Text == "Force")
                {
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        List_Model.Model = worksheet.Cells[row, 1].Text;
                        List_Model.ID_Shaft = worksheet.Cells[row, 2].Text;
                        List_Model.ID_Rotor = worksheet.Cells[row, 3].Text;
                        List_Model.ID_Bearings_Up = worksheet.Cells[row, 4].Text;
                        List_Model.ID_Bearings_Down = worksheet.Cells[row, 5].Text;
                        List_Model.Jig_Up = worksheet.Cells[row, 6].Text;
                        List_Model.Jig_Mid = worksheet.Cells[row, 7].Text;
                        List_Model.Jig_Down = worksheet.Cells[row, 8].Text;
                        List_Model.Height_Stand = float.Parse(worksheet.Cells[row, 9].Text);

                        Jsontemp = JsonConvert.SerializeObject(List_Model);
                        if (Json_new.Length < 2)
                        {
                            Json_new = Json_new + Jsontemp;
                        }
                        else
                        {
                            Json_new = Json_new + "," + Jsontemp;
                        }


                    }
                    Json_new = "[" + Json_new + "]";
                    File.WriteAllText(linkpath.Model, Json_new);
                    System.Windows.MessageBox.Show("Đã nhập dữ liệu thành công");
                }
                else
                {
                    System.Windows.MessageBox.Show("File nhập không đúng mẫu");
                }
            }
        }
        public void Import_BJ_Filepath(string Model_name, string linkpath_json)
        {
            string filePath;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    Import_Beer_Jig(filePath, Model_name, linkpath_json);
                }
            }
        }
        public void Import_Beer_Jig(string filePath, string Model_name, string linkpath_json)
        {
            string Jsontemp;
            string Json_new = "";
            Beer_Jig List_Beer_Jig = new Beer_Jig();
            // Kiểm tra xem file có tồn tại không
            if (!File.Exists(filePath))
            {
                System.Windows.MessageBox.Show("File không tồn tại.");
                return;
            }
            // Sử dụng EPPlus để đọc file Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Lấy worksheet đầu tiên
                var worksheet = package.Workbook.Worksheets[0];
                if (((Model_name == "Model_Jig_Up" | Model_name == "Model_Jig_Down") & worksheet.Cells[1, 2].Text == "Thickness"))
                {
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        List_Beer_Jig.ID = worksheet.Cells[row, 1].Text;
                        List_Beer_Jig.Thickness = float.Parse(worksheet.Cells[row, 2].Text);


                        Jsontemp = JsonConvert.SerializeObject(List_Beer_Jig);
                        if (Json_new.Length < 2)
                        {
                            Json_new = Json_new + Jsontemp;
                        }
                        else
                        {
                            Json_new = Json_new + "," + Jsontemp;
                        }
                    }
                    Json_new = "[" + Json_new + "]";
                    File.WriteAllText(linkpath_json, Json_new);
                    System.Windows.MessageBox.Show("Đã nhập dữ liệu thành công");
                }
                else if (((Model_name == "Model_Beer_Down" | Model_name == "Model_Beer_Up" | Model_name == "Model_Jig_Mid") & worksheet.Cells[1, 1].Text == "ID"))
                {
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        List_Beer_Jig.ID = worksheet.Cells[row, 1].Text;
                        Jsontemp = JsonConvert.SerializeObject(List_Beer_Jig);
                        if (Json_new.Length < 2)
                        {
                            Json_new = Json_new + Jsontemp;
                        }
                        else
                        {
                            Json_new = Json_new + "," + Jsontemp;
                        }
                    }
                    Json_new = "[" + Json_new + "]";
                    File.WriteAllText(linkpath_json, Json_new);
                    System.Windows.MessageBox.Show("Đã Lưu Thành Công");
                }
                else
                {
                    System.Windows.MessageBox.Show("File nhập không đúng mẫu");
                }
            }
        }
        public static string open()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Chọn thư mục.";
                folderDialog.ShowNewFolderButton = true; // Cho phép tạo thư mục mới

                // Hiển thị hộp thoại và kiểm tra xem người dùng đã chọn thư mục hay chưa
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Lấy đường dẫn thư mục đã chọn
                    string selectedPath = folderDialog.SelectedPath;
                    //  System.Windows.MessageBox.Show($"Bạn đã chọn thư mục: {selectedPath}");
                }
                return folderDialog.SelectedPath;
            }
            return "";
        }
        public static (string, string) Coppy_File(string File_Root_name)
        {
            string relativeSourcePath = @"..\Debug\Template\" + File_Root_name + ".xlsx";
            string sourceFilePath = System.IO.Path.GetFullPath(relativeSourcePath); // Đường dẫn đến file gốc
            string folder_path = open();
            string filePath = System.IO.Path.Combine(folder_path, File_Root_name + "_Export.xlsx");
            string destinationFilePath = filePath; // Đường dẫn đến file sao chép
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
            if (!File.Exists(destinationFilePath))
            {
                try
                {
                    File.Copy(sourceFilePath, destinationFilePath, true); // true để ghi đè nếu file đã tồn tại
                                                                          // System.Windows.MessageBox.Show("Sao chép file Excel thành công!");
                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi sao chép file: {ex.Message}");
                }
            }

            return (folder_path, filePath);
        }
        public void Export_BJ_File(string File_Root_name, string Linkpath_Json, string Model_name)
        {
            var result = Coppy_File(File_Root_name);
            var folderPath = result.Item1;
            string sourcePath = result.Item2;

            // Kiểm tra xem đường dẫn thư mục có hợp lệ không
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("Vui lòng nhập một đường dẫn thư mục hợp lệ.");
                return;
            }
            var filePath = System.IO.Path.Combine(folderPath, File_Root_name + "_ExportedData.xlsx");
            Beer_Jig List_Beer_Jig = new Beer_Jig();
            string Fill_json = File.ReadAllText(Linkpath_Json);
            int cnt = 0;
            //try
            //{
            try
            {
                // Tạo một FileInfo cho file Excel
                var fileInfo = new FileInfo(sourcePath);//------------------------
                bool fileExists = fileInfo.Exists;
                ExcelWorksheet worksheet;

                if (fileExists)
                {
                    if (Fill_json.Length > 0)
                    {
                        using (var package = new ExcelPackage(fileInfo))
                        {
                            int nextRow = 2;
                            JArray json_fillArray = JArray.Parse(Fill_json);

                            worksheet = package.Workbook.Worksheets.First();
                            foreach (JObject obj in json_fillArray)
                            {
                                worksheet.Cells[nextRow, 1].Value = (string)obj["ID"];
                                if ((Model_name == "Model_Jig_Up" | Model_name == "Model_Jig_Down"))
                                {
                                    worksheet.Cells[nextRow, 2].Value = (string)obj["Thickness"];
                                }
                                nextRow = nextRow + 1;
                            }
                            package.Save();
                            System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công! Dữ liệu được lưu tại:" + filePath);
                        }

                    }
                }
                else
                {
                    // Nếu file không tồn tại, tạo worksheet mới và thêm tiêu đề
                    using (var package = new ExcelPackage())
                    {
                        worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Cells[1, 1].Value = "ID";
                        if ((Model_name == "Model_Jig_Up" | Model_name == "Model_Jig_Down"))
                        {
                            worksheet.Cells[1, 2].Value = "Thickness";
                        }
                        int nextRow = 2;
                        JArray json_fillArray = JArray.Parse(Fill_json);
                        foreach (JObject obj in json_fillArray)
                        {
                            worksheet.Cells[nextRow, 1].Value = (string)obj["ID"];
                            if ((Model_name == "Model_Jig_Up" | Model_name == "Model_Jig_Down"))
                            {
                                worksheet.Cells[nextRow, 2].Value = (string)obj["Thickness"];
                            }
                            nextRow = nextRow + 1;
                        }
                        package.SaveAs(filePath);
                        System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Đã xảy ra lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }
        public void Export_Model_File(string File_Root_name, string Linkpath_Json)
        {
            var result = Coppy_File(File_Root_name);
            var folderPath = result.Item1;
            string sourcePath = result.Item2;

            // Kiểm tra xem đường dẫn thư mục có hợp lệ không
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("Vui lòng nhập một đường dẫn thư mục hợp lệ.");
                return;
            }
            var filePath = System.IO.Path.Combine(folderPath, File_Root_name + "_ExportedData.xlsx");
            List_Model List_Model = new List_Model();
            string Fill_json = File.ReadAllText(Linkpath_Json);
            int cnt = 0;
            //try
            //{
            try
            {
                // Tạo một FileInfo cho file Excel
                var fileInfo = new FileInfo(sourcePath);//------------------------
                bool fileExists = fileInfo.Exists;
                ExcelWorksheet worksheet;

                if (fileExists)
                {
                    if (Fill_json.Length > 0)
                    {
                        using (var package = new ExcelPackage(fileInfo))
                        {
                            int nextRow = 2;
                            JArray json_fillArray = JArray.Parse(Fill_json);

                            worksheet = package.Workbook.Worksheets.First();
                            foreach (JObject obj in json_fillArray)
                            {
                                worksheet.Cells[nextRow, 1].Value = (string)obj["Model"];
                                worksheet.Cells[nextRow, 2].Value = (string)obj["TrucID"];
                                worksheet.Cells[nextRow, 3].Value = (string)obj["RotorID"];
                                worksheet.Cells[nextRow, 4].Value = (string)obj["Beer_UP"];
                                worksheet.Cells[nextRow, 5].Value = (string)obj["Beer_Down"];
                                worksheet.Cells[nextRow, 6].Value = (string)obj["Jig_Up"];
                                worksheet.Cells[nextRow, 7].Value = (string)obj["Jig_Mid"];
                                worksheet.Cells[nextRow, 8].Value = (string)obj["Jig_Down"];
                                worksheet.Cells[nextRow, 9].Value = (string)obj["HStand"];
                                worksheet.Cells[nextRow, 10].Value = (string)obj["force"];
                                nextRow = nextRow + 1;
                            }
                            package.Save();
                            System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công! Dữ liệu được lưu tại:" + filePath);
                        }

                    }
                }
                else
                {
                    // Nếu file không tồn tại, tạo worksheet mới và thêm tiêu đề
                    using (var package = new ExcelPackage())
                    {
                        worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Cells[1, 1].Value = "Model";
                        worksheet.Cells[1, 2].Value = "Truc ID";
                        worksheet.Cells[1, 3].Value = "RotoID";
                        worksheet.Cells[1, 4].Value = "Beer_up";
                        worksheet.Cells[1, 5].Value = "Beer_down";
                        worksheet.Cells[1, 6].Value = "Jig_Up";
                        worksheet.Cells[1, 7].Value = "Jig_Mid";
                        worksheet.Cells[1, 8].Value = "Jig_Down";
                        worksheet.Cells[1, 9].Value = "Hstand";
                        worksheet.Cells[1, 10].Value = "Force";
                        int nextRow = 2;
                        JArray json_fillArray = JArray.Parse(Fill_json);
                        foreach (JObject obj in json_fillArray)
                        {
                            worksheet.Cells[nextRow, 1].Value = (string)obj["Model"];
                            worksheet.Cells[nextRow, 2].Value = (string)obj["TrucID"];
                            worksheet.Cells[nextRow, 3].Value = (string)obj["RotorID"];
                            worksheet.Cells[nextRow, 4].Value = (string)obj["Beer_UP"];
                            worksheet.Cells[nextRow, 5].Value = (string)obj["Beer_Down"];
                            worksheet.Cells[nextRow, 6].Value = (string)obj["Jig_Up"];
                            worksheet.Cells[nextRow, 7].Value = (string)obj["Jig_Mid"];
                            worksheet.Cells[nextRow, 8].Value = (string)obj["Jig_Down"];
                            worksheet.Cells[nextRow, 9].Value = (string)obj["HStand"];
                            worksheet.Cells[nextRow, 10].Value = (string)obj["force"];
                            nextRow = nextRow + 1;
                        }
                        package.SaveAs(filePath);
                        System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Đã xảy ra lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }
        public void Export_History_File(string File_Root_name)
        {
            var result = Coppy_File(File_Root_name);
            var folderPath = result.Item1;
            string sourcePath = result.Item2;

            // Kiểm tra xem đường dẫn thư mục có hợp lệ không
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("Vui lòng nhập một đường dẫn thư mục hợp lệ.");
                return;
            }
            var filePath = System.IO.Path.Combine(folderPath, File_Root_name + "_ExportedData.xlsx");
            string Fill_json_EN = File.ReadAllText(linkpath.History_EN);
            string Fill_json_VN = File.ReadAllText(linkpath.History_VN);
            int cnt = 0;
            try
            {
                // Tạo một FileInfo cho file Excel
                var fileInfo = new FileInfo(sourcePath);//------------------------
                bool fileExists = fileInfo.Exists;
                ExcelWorksheet worksheet;

                if (fileExists)
                {
                    if (Fill_json_EN.Length > 0 & Fill_json_VN.Length > 0)
                    {
                        using (var package = new ExcelPackage(fileInfo))
                        {
                            worksheet = package.Workbook.Worksheets.First();
                            int nextRow = 6;
                            JArray json_fillArray_EN = JArray.Parse(Fill_json_EN);
                            foreach (JObject obj in json_fillArray_EN)
                            {
                                worksheet.Cells[nextRow, 1].Value = (string)obj["Code"];
                                worksheet.Cells[nextRow, 2].Value = (string)obj["Description"];
                                worksheet.Cells[nextRow, 3].Value = (string)obj["Solution"];
                                nextRow = nextRow + 1;
                            }
                            nextRow = 6;
                            JArray json_fillArray_VN = JArray.Parse(Fill_json_VN);
                            foreach (JObject obj in json_fillArray_VN)
                            {
                                worksheet.Cells[nextRow, 4].Value = (string)obj["Description"];
                                worksheet.Cells[nextRow, 5].Value = (string)obj["Solution"];
                                nextRow = nextRow + 1;
                            }

                            package.Save();
                            System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công!");
                        }

                    }
                }
                else
                {
                    // Nếu file không tồn tại, tạo worksheet mới và thêm tiêu đề
                    using (var package = new ExcelPackage())
                    {
                        worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Cells[5, 1].Value = "Code";
                        worksheet.Cells[5, 2].Value = "Desciption_EN";
                        worksheet.Cells[5, 3].Value = "Solution_EN";
                        worksheet.Cells[5, 2].Value = "Desciption_VN";
                        worksheet.Cells[5, 3].Value = "Solution_VN";
                        int nextRow = 6;
                        JArray json_fillArray_EN = JArray.Parse(Fill_json_EN);
                        foreach (JObject obj in json_fillArray_EN)
                        {
                            worksheet.Cells[nextRow, 1].Value = (string)obj["Code"];
                            worksheet.Cells[nextRow, 2].Value = (string)obj["Description"];
                            worksheet.Cells[nextRow, 3].Value = (string)obj["Solution"];
                            nextRow = nextRow + 1;
                        }
                        nextRow = 6;
                        JArray json_fillArray_VN = JArray.Parse(Fill_json_VN);
                        foreach (JObject obj in json_fillArray_VN)
                        {
                            worksheet.Cells[nextRow, 4].Value = (string)obj["Description"];
                            worksheet.Cells[nextRow, 5].Value = (string)obj["Solution"];
                            nextRow = nextRow + 1;
                        }
                        package.SaveAs(filePath);
                        System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Đã xảy ra lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }
        public void Import_History_Filepath()
        {
            string filePath;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    Import_History(filePath);
                }
            }
        }
        public void Import_History(string filePath)
        {
            // System.DateTime dateTime = System.DateTime.Now;
            // string formattedDate = dateTime.ToString("dd/MM/yy");
            // string formattedtime = dateTime.ToString("HH:mm:ss");
            // string ID = formattedDate.Replace("/", "") + formattedtime.Replace(":", "");
            string Jsontemp_EN;
            string Jsontemp_VN;
            string Json_new_EN = "";
            string Json_new_VN = "";
            List_History List_History_EN = new List_History();
            List_History List_History_VN = new List_History();
            // Kiểm tra xem file có tồn tại không
            if (!File.Exists(filePath))
            {
                System.Windows.MessageBox.Show("File không tồn tại.");
                return;
            }
            // Sử dụng EPPlus để đọc file Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Lấy worksheet đầu tiên
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet.Cells[5, 3].Text == "Solution_EN")
                {
                    for (int row = 6; row <= worksheet.Dimension.Rows; row++)
                    {
                        List_History_EN.Code = worksheet.Cells[row, 1].Text;
                        List_History_EN.Description = worksheet.Cells[row, 2].Text;
                        List_History_EN.Solution = worksheet.Cells[row, 3].Text;
                        List_History_VN.Code = worksheet.Cells[row, 1].Text;
                        List_History_VN.Description = worksheet.Cells[row, 4].Text;
                        List_History_VN.Solution = worksheet.Cells[row, 5].Text;
                        Jsontemp_EN = JsonConvert.SerializeObject(List_History_EN);
                        Jsontemp_VN = JsonConvert.SerializeObject(List_History_VN);
                        if (Json_new_EN.Length < 2)
                        {
                            Json_new_EN = Json_new_EN + Jsontemp_EN;
                        }
                        else
                        {
                            Json_new_EN = Json_new_EN + "," + Jsontemp_EN;
                        }
                        if (Json_new_VN.Length < 2)
                        {
                            Json_new_VN = Json_new_VN + Jsontemp_VN;
                        }
                        else
                        {
                            Json_new_VN = Json_new_VN + "," + Jsontemp_VN;
                        }

                    }
                    Json_new_EN = "[" + Json_new_EN + "]";
                    File.WriteAllText(linkpath.History_EN, Json_new_EN);
                    Json_new_VN = "[" + Json_new_VN + "]";
                    File.WriteAllText(linkpath.History_VN, Json_new_VN);
                    System.Windows.MessageBox.Show("Đã nhập dữ liệu thành công!");
                }
                else
                {
                    System.Windows.MessageBox.Show("File nhập không đúng mẫu");
                }
                // Gán DataTable cho DataGridView
            }
        }
        public void Export_Chart_File(string File_Root_name, List<Position> dataPoint)
        {
            string json = File.ReadAllText(linkpath.Chart);
            var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            string folderPath = open();

            // Kiểm tra xem đường dẫn thư mục có hợp lệ không
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yyyy").Replace('/', '_');
            var filePath = System.IO.Path.Combine(folderPath, File_Root_name + ".xlsx");
            var FinalPath = GetUniqueFileName(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
            try
            {
                // Tạo một FileInfo cho file Excel
                var fileInfo = new FileInfo(filePath);//------------------------
                bool fileExists = fileInfo.Exists;
                ExcelWorksheet worksheet;
                // Nếu file không tồn tại, tạo worksheet mới và thêm tiêu đề
                using (var package = new ExcelPackage())
                {
                    worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    int nextRow = 1;
                    worksheet.Cells[1, 1].Value = "Order_Code:";
                    worksheet.Cells[1, 2].Value = "Model:";
                    worksheet.Cells[1, 3].Value = "RotorID:";
                    worksheet.Cells[1, 4].Value = "TrucID:";
                    worksheet.Cells[1, 5].Value = "Beer_Up:";
                    worksheet.Cells[1, 6].Value = "Beer_Down:";
                    worksheet.Cells[1, 7].Value = "Jig_Up:";
                    worksheet.Cells[1, 8].Value = "Jig_Mid:";
                    worksheet.Cells[1, 9].Value = "Jig_Down:";
                    worksheet.Cells[1, 10].Value = "Hstand:";
                    worksheet.Cells[1, 11].Value = "Force:";
                    worksheet.Cells[1, 12].Value = "Force_max:";
                    worksheet.Cells[1, 13].Value = "Lực Ép:";
                    worksheet.Cells[1, 14].Value = "Vị Trí:";
                    worksheet.Cells[2, 1].Value = Data_Report_temp.OrderCode;
                    worksheet.Cells[2, 2].Value = Data_Report_temp.Model;
                    worksheet.Cells[2, 3].Value = Data_Report_temp.RotorID;
                    worksheet.Cells[2, 4].Value = Data_Report_temp.TrucID;
                    worksheet.Cells[2, 5].Value = Data_Report_temp.Beer_Up;
                    worksheet.Cells[2, 6].Value = Data_Report_temp.Beer_Down;
                    worksheet.Cells[2, 7].Value = Data_Report_temp.Jig_Up;
                    worksheet.Cells[2, 8].Value = Data_Report_temp.Jig_Mid;
                    worksheet.Cells[2, 9].Value = Data_Report_temp.Jig_Down;
                    worksheet.Cells[2, 10].Value = Data_Report_temp.HStand;
                    worksheet.Cells[2, 11].Value = Data_Report_temp.Force;
                    worksheet.Cells[2, 12].Value = Data_Report_temp.Force_Max;

                    nextRow = 2;
                    for (int i = 0; i < dataPoint.Count; i++)
                    {
                        worksheet.Cells[nextRow, 13].Value = dataPoint[i].Force;
                        worksheet.Cells[nextRow, 14].Value = dataPoint[i].PST;
                        nextRow++;
                    }
                    package.SaveAs(FinalPath);
                    System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Đã xảy ra lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }
        public void Export_Report_All_File(string File_Root_name)
        {
            string json = File.ReadAllText(linkpath.Chart);
            var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            string folderPath = open();

            // Kiểm tra xem đường dẫn thư mục có hợp lệ không
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yyyy").Replace('/', '_');
            var filePath = System.IO.Path.Combine(folderPath, File_Root_name + ".xlsx");
            var FinalPath = GetUniqueFileName(filePath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
            try
            {
                // Tạo một FileInfo cho file Excel
                var fileInfo = new FileInfo(filePath);//------------------------
                bool fileExists = fileInfo.Exists;
                ExcelWorksheet worksheet;
                // Nếu file không tồn tại, tạo worksheet mới và thêm tiêu đề
                using (var package = new ExcelPackage())
                {
                    worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    int nextRow = 0;
                    foreach (var data in Global.List_report_all)
                    {
                        nextRow++;
                        worksheet.Cells[nextRow, 1].Value = "Time:";
                        worksheet.Cells[nextRow, 2].Value = "Order_Code:";
                        worksheet.Cells[nextRow, 3].Value = "Model:";
                        worksheet.Cells[nextRow, 4].Value = "RotorID:";
                        worksheet.Cells[nextRow, 5].Value = "TrucID:";
                        worksheet.Cells[nextRow, 6].Value = "Beer_Up:";
                        worksheet.Cells[nextRow, 7].Value = "Beer_Down:";
                        worksheet.Cells[nextRow, 8].Value = "Jig_Up:";
                        worksheet.Cells[nextRow, 9].Value = "Jig_Mid:";
                        worksheet.Cells[nextRow, 10].Value = "Jig_Down:";
                        worksheet.Cells[nextRow, 11].Value = "Hstand:";
                        worksheet.Cells[nextRow, 12].Value = "Force:";
                        worksheet.Cells[nextRow, 13].Value = "Force_max:";
                        worksheet.Cells[nextRow, 14].Value = "Lực Ép:";
                        worksheet.Cells[nextRow, 15].Value = "Vị Trí:";
                        nextRow++;
                        worksheet.Cells[nextRow, 1].Value = data.Time;
                        worksheet.Cells[nextRow, 2].Value = data.OrderCode;
                        worksheet.Cells[nextRow, 3].Value = data.Model;
                        worksheet.Cells[nextRow, 4].Value = data.RotorID;
                        worksheet.Cells[nextRow, 5].Value = data.TrucID;
                        worksheet.Cells[nextRow, 6].Value = data.Beer_Up;
                        worksheet.Cells[nextRow, 7].Value = data.Beer_Down;
                        worksheet.Cells[nextRow, 8].Value = data.Jig_Up;
                        worksheet.Cells[nextRow, 9].Value = data.Jig_Mid;
                        worksheet.Cells[nextRow, 10].Value = data.Jig_Down;
                        worksheet.Cells[nextRow, 11].Value = data.HStand;
                        worksheet.Cells[nextRow, 12].Value = data.Force;
                        worksheet.Cells[nextRow, 13].Value = data.Force_Max;
                        strim_Position(data.Position);
                        for (int i = 0; i < Global.List_Position_all.Count; i++)
                        {
                            worksheet.Cells[nextRow, 14].Value = Global.List_Position_all[i].Force;
                            worksheet.Cells[nextRow, 15].Value = Global.List_Position_all[i].PST;
                            nextRow++;
                        }
                    }
                    package.SaveAs(FinalPath);
                    System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Đã xảy ra lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }
        private void strim_Position(string data)
        {
            string[] pairs = data.Split(',');

            // Tạo List để lưu trữ các cặp mã và giá trị
            Global.List_Position_all.Clear();
            foreach (var pair in pairs)
            {
                // Phân tách từng cặp số bằng dấu gạch dưới
                var parts = pair.Split('_');
                if (parts.Length == 2)
                {

                    float Momen = 0;
                    float PTS = 0;
                    if (float.TryParse(parts[0].Replace('.', ','), out float result))
                    {
                        // Làm tròn đến 2 chữ số thập phân
                        Momen = (float)Math.Round(result, 2);
                    }
                    if (float.TryParse(parts[1].Replace('.', ','), out float result1))
                    {
                        // Làm tròn đến 2 chữ số thập phân
                        PTS = (float)Math.Round(result1, 2);
                    }
                    Global.List_Position_all.Add(new Position(PTS, Momen));
                }
            }
        }
        static string GetUniqueFileName(string filePath)
        {
            string directory = System.IO.Path.GetDirectoryName(filePath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string extension = System.IO.Path.GetExtension(filePath);
            string uniqueFilePath = filePath;

            int count = 1;
            while (File.Exists(uniqueFilePath))
            {
                uniqueFilePath = System.IO.Path.Combine(directory, $"{fileName} ({count}){extension}");
                count++;
            }

            return uniqueFilePath;
        }
    }
}
