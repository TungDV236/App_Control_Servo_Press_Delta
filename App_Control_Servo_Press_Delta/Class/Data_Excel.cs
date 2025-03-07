
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
using System.Windows.Controls;

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
        public void Backup_File(string File_Root_name, string folder)
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            string ID = formattedDate.Replace("/", "_") + formattedtime.Replace(":", "_");
            string relativeSourcePath = "";
            if (folder == "Model")
            {
                relativeSourcePath = @"..\Debug\Model\" + File_Root_name; // Đường dẫn đến file gốc
            }
            if (folder == "Path")
            {
                relativeSourcePath = @"..\Debug\Path\" + File_Root_name; // Đường dẫn đến file gốc
            }
            string sourceFilePath = System.IO.Path.GetFullPath(relativeSourcePath); // Đường dẫn đến file gốc
            string filePath = System.IO.Path.Combine("Backup", ID + "_" + File_Root_name);
            string destinationFilePath = filePath; // Đường dẫn đến file sao chép
            try
            {
                File.Copy(sourceFilePath, destinationFilePath, true); // true để ghi đè nếu file đã tồn tại
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show($"Lỗi sao chép file: {ex.Message}");
            }
        }
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
            try
            {
                string Jsontemp;
                string Json_new = "";
                string JigU = "";
                string JigM;
                string JigD;
                string BearingsU;
                string BearingsD;
                float thicknessBearingsU;
                float thicknessBearingsD;
                float thicknessJigU;
                float thicknessJigD;
                bool checkJigU = false;
                bool checkJigM = false;
                bool checkJigD = false;
                bool checkBearingsU = false;
                bool checkBearingsD = false;
                int checkMode1 = 0;
                int checkMode2 = 0;

                //  List_Model List_Model = new List_Model();
                List_Model List_Model = new List_Model();

                // Kiểm tra xem file có tồn tại không
                if (!File.Exists(filePath))
                {
                    System.Windows.MessageBox.Show("File không tồn tại.");
                    return;
                }

                Backup_File("Model.Json", "Model");
                // Sử dụng EPPlus để đọc file Excel
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Thiết lập ngữ cảnh giấy phép
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    // Lấy worksheet đầu tiên
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet.Cells[1, 1].Text == "Model")
                    {
                        for (int row = 3; row <= worksheet.Dimension.Rows; row++)
                        {
                            List_Model.Model = worksheet.Cells[row, 1].Text;
                            List_Model.ID_Shaft = worksheet.Cells[row, 2].Text;
                            List_Model.ID_Rotor = worksheet.Cells[row, 3].Text;
                            List_Model.ID_Bearings_Up = worksheet.Cells[row, 4].Text;
                            List_Model.ID_Bearings_Down = worksheet.Cells[row, 5].Text;
                            checkBearingsU = Fill_Bearings_JigUD(linkpath.Bearings_Up, worksheet.Cells[row, 4].Text, out BearingsU, out thicknessBearingsU);
                            checkBearingsD = Fill_Bearings_JigUD(linkpath.Bearings_Down, worksheet.Cells[row, 5].Text, out BearingsD, out thicknessBearingsD);
                            checkJigU = Fill_Bearings_JigUD(linkpath.Jig_Up, worksheet.Cells[row, 6].Text, out JigU, out thicknessJigU);
                            checkJigM = Fill_JigM(linkpath.Jig_Mid, worksheet.Cells[row, 7].Text, out JigM);
                            checkJigD = Fill_Bearings_JigUD(linkpath.Jig_Down, worksheet.Cells[row, 8].Text, out JigD, out thicknessJigD);
                            List_Model.Jig_Up = JigU;
                            List_Model.Jig_Mid = JigM;
                            List_Model.Jig_Down = JigD;
                            List_Model.Height_Stand = float.Parse(worksheet.Cells[row, 9].Text);
                            List_Model.After_press_bearings_distance = float.Parse(worksheet.Cells[row, 10].Text);
                            List_Model.Pre_press_Bearings_distance = float.Parse(worksheet.Cells[row, 11].Text);
                            List_Model.Ofset_position1 = float.Parse(worksheet.Cells[row, 12].Text);
                            List_Model.Ofset_position2 = float.Parse(worksheet.Cells[row, 13].Text);
                            List_Model.Origin_Position = float.Parse(worksheet.Cells[row, 14].Text);
                            List_Model.Origin_Velocity = float.Parse(worksheet.Cells[row, 15].Text);
                            List_Model.Standby_Position = float.Parse(worksheet.Cells[row, 16].Text);
                            List_Model.Standby_Velocity = float.Parse(worksheet.Cells[row, 17].Text);
                            List_Model.Standby_Time = float.Parse(worksheet.Cells[row, 18].Text);
                            List_Model.Data_Func1 = new List<DataFunC>
                        {
                            new DataFunC
                            {
                                Mode = 0,
                                Press_Condition = "",
                                Press_Force = 0,
                                Press_Vel = 0,
                                Press_Time = 0,
                                End_Max_Force_Limit = 0,
                                End_Min_Force_Limit = 0,
                                End_Max_Pos_Limit = 0,
                                End_Min_Pos_Limit = 0
                            }
                        };
                            List_Model.Data_Func2 = new List<DataFunC>
                        {
                            new DataFunC
                            {
                                Mode = 0,
                                Press_Condition = "",
                                Press_Force = 0,
                                Press_Vel = 0,
                                Press_Time = 0,
                                End_Max_Force_Limit = 0,
                                End_Min_Force_Limit = 0,
                                End_Max_Pos_Limit = 0,
                                End_Min_Pos_Limit = 0
                             }
                        };
                            List_Model.Data_Func1[0].Mode = float.Parse(worksheet.Cells[row, 19].Text);
                            List_Model.Data_Func1[0].Press_Condition = CheckMode(worksheet.Cells[row, 19].Text, out checkMode1);
                            List_Model.Data_Func1[0].Press_Force = float.Parse(worksheet.Cells[row, 20].Text);
                            List_Model.Data_Func1[0].Press_Vel = float.Parse(worksheet.Cells[row, 21].Text);
                            List_Model.Data_Func1[0].Press_Time = float.Parse(worksheet.Cells[row, 22].Text);
                            List_Model.Data_Func1[0].End_Max_Force_Limit = float.Parse(worksheet.Cells[row, 23].Text);
                            List_Model.Data_Func1[0].End_Min_Force_Limit = float.Parse(worksheet.Cells[row, 24].Text);
                            List_Model.Data_Func1[0].End_Max_Pos_Limit = float.Parse(worksheet.Cells[row, 25].Text);
                            List_Model.Data_Func1[0].End_Min_Pos_Limit = float.Parse(worksheet.Cells[row, 26].Text);
                            List_Model.Data_Func2[0].Mode = float.Parse(worksheet.Cells[row, 27].Text);
                            List_Model.Data_Func2[0].Press_Condition = CheckMode(worksheet.Cells[row, 27].Text, out checkMode2);
                            List_Model.Data_Func2[0].Press_Force = float.Parse(worksheet.Cells[row, 28].Text);
                            List_Model.Data_Func2[0].Press_Vel = float.Parse(worksheet.Cells[row, 29].Text);
                            List_Model.Data_Func2[0].Press_Time = float.Parse(worksheet.Cells[row, 30].Text);
                            List_Model.Data_Func2[0].End_Max_Force_Limit = float.Parse(worksheet.Cells[row, 31].Text);
                            List_Model.Data_Func2[0].End_Min_Force_Limit = float.Parse(worksheet.Cells[row, 32].Text);
                            List_Model.Data_Func2[0].End_Max_Pos_Limit = float.Parse(worksheet.Cells[row, 33].Text);
                            List_Model.Data_Func2[0].End_Min_Pos_Limit = float.Parse(worksheet.Cells[row, 34].Text);
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
                        if (!checkJigU)
                        {

                            System.Windows.MessageBox.Show("Mã Jig_Up không có trong model jig, vui lòng kiểm tra lại");
                        }
                        else if (!checkJigM)
                        {

                            System.Windows.MessageBox.Show("Mã Jig_Mid không có trong model jig, vui lòng kiểm tra lại");
                        }
                        else if (!checkJigD)
                        {

                            System.Windows.MessageBox.Show("Mã Jig_Down không có trong model jig, vui lòng kiểm tra lại");
                        }
                        else if (!checkBearingsU)
                        {

                            System.Windows.MessageBox.Show("Mã Bearings_Up không có trong model Bearings_Up, vui lòng kiểm tra lại");
                        }
                        else if (!checkBearingsD)
                        {

                            System.Windows.MessageBox.Show("Mã Bearings_Down không có trong model Bearings_Down, vui lòng kiểm tra lại");
                        }
                        else if (checkMode1 == 6)
                        {

                            System.Windows.MessageBox.Show("Mode1 cài đặt không chính xác, vui lòng kiểm tra lại");
                        }
                        else if (checkMode2 == 6)
                        {

                            System.Windows.MessageBox.Show("Mode2 cài đặt không chính xác, vui lòng kiểm tra lại");
                        }
                        else
                        {

                            File.WriteAllText(linkpath.Model, Json_new);
                            System.Windows.MessageBox.Show("Đã nhập dữ liệu thành công");
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("File nhập không đúng mẫu");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                Common common = new Common();
                common.Log_err(ex.ToString());

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

            Backup_File(Model_name+".Json", "Model");
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
                if (((Model_name == "Jig_Up" | Model_name == "Jig_Down") & worksheet.Cells[1, 2].Text == "Thickness"))
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
                else if (((Model_name == "Beer_Down" | Model_name == "Beer_Up" | Model_name == "Jig_Mid") & worksheet.Cells[1, 1].Text == "ID"))
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
        public static string open(string title)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = title;
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
        public static (string, string) Coppy_File(string File_Root_name, string title, string NameFileExxport)
        {
            string relativeSourcePath = @"..\Debug\Template\" + File_Root_name + ".xlsx";
            string sourceFilePath = System.IO.Path.GetFullPath(relativeSourcePath); // Đường dẫn đến file gốc
            string folder_path = open(title);
            string filePath = System.IO.Path.Combine(folder_path, NameFileExxport + "_Export.xlsx");
            string destinationFilePath = filePath; // Đường dẫn đến file sao chép
            int count = 1;
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
            else
            {
                try
                {
                    while (File.Exists(filePath))
                    {

                        filePath = System.IO.Path.Combine(folder_path, NameFileExxport + $"_Export_{count}.xlsx");
                        count++;
                    }
                    File.Copy(sourceFilePath, filePath, true);

                }
                catch (IOException ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi sao chép file: {ex.Message}");
                }
            }

            return (folder_path, filePath);
        }
        public void Export_BJ_File(string File_Root_name, string Linkpath_Json, string Model_name, string title, string NameFileEXport)
        {
            var result = Coppy_File(File_Root_name, title, NameFileEXport);
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
        public void Export_Model_File(string File_Root_name, string Linkpath_Json, bool Enable_Message, string title, string NameFileEXport)
        {
            var result = Coppy_File(File_Root_name, title, NameFileEXport);
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
                            int nextRow = 3;
                            List<List_Model> jsonArray = JsonConvert.DeserializeObject<List<List_Model>>(Fill_json);
                            //JArray jsonArray = JArray.Parse(json);
                            foreach (var obj in jsonArray)
                            {

                            worksheet = package.Workbook.Worksheets.First();

                                worksheet.Cells[nextRow, 1].Value = obj.Model;
                                worksheet.Cells[nextRow, 2].Value = obj.ID_Shaft;
                                worksheet.Cells[nextRow, 3].Value = obj.ID_Rotor;
                                worksheet.Cells[nextRow, 4].Value = obj.ID_Bearings_Up;
                                worksheet.Cells[nextRow, 5].Value = obj.ID_Bearings_Down;
                                worksheet.Cells[nextRow, 6].Value = obj.Jig_Up;
                                worksheet.Cells[nextRow, 7].Value = obj.Jig_Mid;
                                worksheet.Cells[nextRow, 8].Value = obj.Jig_Down;
                                worksheet.Cells[nextRow, 9].Value = obj.Height_Stand;
                                worksheet.Cells[nextRow, 10].Value = obj.Pre_press_Bearings_distance;
                                worksheet.Cells[nextRow, 11].Value = obj.After_press_bearings_distance;
                                worksheet.Cells[nextRow, 12].Value = obj.Ofset_position1;
                                worksheet.Cells[nextRow, 13].Value = obj.Ofset_position2;
                                worksheet.Cells[nextRow, 14].Value = obj.Origin_Position;
                                worksheet.Cells[nextRow, 15].Value = obj.Origin_Velocity;
                                worksheet.Cells[nextRow, 16].Value = obj.Standby_Position;
                                worksheet.Cells[nextRow, 17].Value = obj.Standby_Velocity;
                                worksheet.Cells[nextRow, 18].Value = obj.Standby_Time;
                                worksheet.Cells[nextRow, 19].Value = obj.Data_Func1[0].Mode;
                                worksheet.Cells[nextRow, 20].Value = obj.Data_Func1[0].Press_Force;
                                worksheet.Cells[nextRow, 21].Value = obj.Data_Func1[0].Press_Vel;
                                worksheet.Cells[nextRow, 22].Value = obj.Data_Func1[0].Press_Time;
                                worksheet.Cells[nextRow, 23].Value = obj.Data_Func1[0].End_Max_Force_Limit;
                                worksheet.Cells[nextRow, 24].Value = obj.Data_Func1[0].End_Min_Force_Limit;
                                worksheet.Cells[nextRow, 25].Value = obj.Data_Func1[0].End_Max_Pos_Limit;
                                worksheet.Cells[nextRow, 26].Value = obj.Data_Func1[0].End_Min_Pos_Limit;
                                worksheet.Cells[nextRow, 27].Value = obj.Data_Func2[0].Mode;
                                worksheet.Cells[nextRow, 28].Value = obj.Data_Func2[0].Press_Force;
                                worksheet.Cells[nextRow, 29].Value = obj.Data_Func2[0].Press_Vel;
                                worksheet.Cells[nextRow, 30].Value = obj.Data_Func2[0].Press_Time;
                                worksheet.Cells[nextRow, 31].Value = obj.Data_Func2[0].End_Max_Force_Limit;
                                worksheet.Cells[nextRow, 32].Value = obj.Data_Func2[0].End_Min_Force_Limit;
                                worksheet.Cells[nextRow, 33].Value = obj.Data_Func2[0].End_Max_Pos_Limit;
                                worksheet.Cells[nextRow, 34].Value = obj.Data_Func2[0].End_Min_Pos_Limit;
                                nextRow = nextRow + 1;
                            }
                            package.Save();

                            if (Enable_Message)
                            {
                                System.Windows.MessageBox.Show("Dữ liệu đã được xuất ra Excel thành công! Dữ liệu được lưu tại:" + filePath);
                            }
                        }

                    }
                }
                else
                {
                    // Nếu file không tồn tại, tạo worksheet mới và thêm tiêu đề
                    using (var package = new ExcelPackage())
                    {
                        worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        worksheet.Cells[2, 1].Value = "Model";
                        worksheet.Cells[2, 2].Value = "ID_Shaft";
                        worksheet.Cells[2, 3].Value = "ID_Rotor";
                        worksheet.Cells[2, 4].Value = "ID_Bearings_Up";
                        worksheet.Cells[2, 5].Value = "ID_Bearings_Down";
                        worksheet.Cells[2, 6].Value = "Jig_Up";
                        worksheet.Cells[2, 7].Value = "Jig_Mid";
                        worksheet.Cells[2, 8].Value = "Jig_Down";
                        worksheet.Cells[2, 9].Value = "Height_Stand";
                        worksheet.Cells[2, 10].Value = "Pre-Press_Bearings_Distance";
                        worksheet.Cells[2, 11].Value = "After-Press_Bearings_Distance";
                        worksheet.Cells[2, 12].Value = "Ofset_Press";
                        worksheet.Cells[2, 13].Value = "Ofset2";
                        worksheet.Cells[2, 14].Value = "Origin_Position";
                        worksheet.Cells[2, 15].Value = "Origin_Velocity";
                        worksheet.Cells[2, 16].Value = "Standby_Position";
                        worksheet.Cells[2, 17].Value = "Standby_Velocity";
                        worksheet.Cells[2, 18].Value = "Standby_Time";
                        worksheet.Cells[2, 19].Value = "Mode";
                        worksheet.Cells[2, 20].Value = "Press_Force";
                        worksheet.Cells[2, 21].Value = "Press_Velocity";
                        worksheet.Cells[2, 22].Value = "Press_Time";
                        worksheet.Cells[2, 23].Value = "Max_Force";
                        worksheet.Cells[2, 24].Value = "Min_Force";
                        worksheet.Cells[2, 25].Value = "Max_Position";
                        worksheet.Cells[2, 26].Value = "Min_Position";
                        worksheet.Cells[2, 27].Value = "Mode";
                        worksheet.Cells[2, 28].Value = "Press_Force";
                        worksheet.Cells[2, 29].Value = "Press_Velocity";
                        worksheet.Cells[2, 30].Value = "Press_Time";
                        worksheet.Cells[2, 31].Value = "Max_Force";
                        worksheet.Cells[2, 32].Value = "Min_Force";
                        worksheet.Cells[2, 33].Value = "Max_Position";
                        worksheet.Cells[2, 34].Value = "Min_Position";

                        int nextRow = 3;
                        List<List_Model> jsonArray = JsonConvert.DeserializeObject<List<List_Model>>(Fill_json);
                        //JArray jsonArray = JArray.Parse(json);
                        foreach (var obj in jsonArray)
                        {

                            worksheet.Cells[nextRow, 1].Value = obj.Model;
                            worksheet.Cells[nextRow, 2].Value = obj.ID_Shaft;
                            worksheet.Cells[nextRow, 3].Value = obj.ID_Rotor;
                            worksheet.Cells[nextRow, 4].Value = obj.ID_Bearings_Up;
                            worksheet.Cells[nextRow, 5].Value = obj.ID_Bearings_Down;
                            worksheet.Cells[nextRow, 6].Value = obj.Jig_Up;
                            worksheet.Cells[nextRow, 7].Value = obj.Jig_Mid;
                            worksheet.Cells[nextRow, 8].Value = obj.Jig_Down;
                            worksheet.Cells[nextRow, 9].Value = obj.Height_Stand;
                            worksheet.Cells[nextRow, 10].Value = obj.Pre_press_Bearings_distance;
                            worksheet.Cells[nextRow, 11].Value = obj.After_press_bearings_distance;
                            worksheet.Cells[nextRow, 12].Value = obj.Ofset_position1;
                            worksheet.Cells[nextRow, 13].Value = obj.Ofset_position2;
                            worksheet.Cells[nextRow, 14].Value = obj.Origin_Position;
                            worksheet.Cells[nextRow, 15].Value = obj.Origin_Velocity;
                            worksheet.Cells[nextRow, 16].Value = obj.Standby_Position;
                            worksheet.Cells[nextRow, 17].Value = obj.Standby_Velocity;
                            worksheet.Cells[nextRow, 18].Value = obj.Standby_Time;
                            worksheet.Cells[nextRow, 19].Value = obj.Data_Func1[0].Mode;
                            worksheet.Cells[nextRow, 20].Value = obj.Data_Func1[0].Press_Force;
                            worksheet.Cells[nextRow, 21].Value = obj.Data_Func1[0].Press_Vel;
                            worksheet.Cells[nextRow, 22].Value = obj.Data_Func1[0].Press_Time;
                            worksheet.Cells[nextRow, 23].Value = obj.Data_Func1[0].End_Max_Force_Limit;
                            worksheet.Cells[nextRow, 24].Value = obj.Data_Func1[0].End_Min_Force_Limit;
                            worksheet.Cells[nextRow, 25].Value = obj.Data_Func1[0].End_Max_Pos_Limit;
                            worksheet.Cells[nextRow, 26].Value = obj.Data_Func1[0].End_Min_Pos_Limit;
                            worksheet.Cells[nextRow, 27].Value = obj.Data_Func2[0].Mode;
                            worksheet.Cells[nextRow, 28].Value = obj.Data_Func2[0].Press_Force;
                            worksheet.Cells[nextRow, 29].Value = obj.Data_Func2[0].Press_Vel;
                            worksheet.Cells[nextRow, 30].Value = obj.Data_Func2[0].Press_Time;
                            worksheet.Cells[nextRow, 31].Value = obj.Data_Func2[0].End_Max_Force_Limit;
                            worksheet.Cells[nextRow, 32].Value = obj.Data_Func2[0].End_Min_Force_Limit;
                            worksheet.Cells[nextRow, 33].Value = obj.Data_Func2[0].End_Max_Pos_Limit;
                            worksheet.Cells[nextRow, 34].Value = obj.Data_Func2[0].End_Min_Pos_Limit;
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
        public void Export_History_File(string File_Root_name, string title, string NameFileEXport)
        {
            var result = Coppy_File(File_Root_name, title, NameFileEXport);
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
        public void Export_IO_File(string File_Root_name, string title, string NameFileEXport)
        {
            var result = Coppy_File(File_Root_name, title, NameFileEXport);
            var folderPath = result.Item1;
            string sourcePath = result.Item2;

            // Kiểm tra xem đường dẫn thư mục có hợp lệ không
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("Vui lòng nhập một đường dẫn thư mục hợp lệ.");
                return;
            }
            var filePath = System.IO.Path.Combine(folderPath, File_Root_name + "_ExportedData.xlsx");
            string Fill_json_EN = File.ReadAllText(linkpath.GPIO_EN);
            string Fill_json_VN = File.ReadAllText(linkpath.GPIO_VN);
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
                                worksheet.Cells[nextRow, 1].Value = (string)obj["IO_Name"];
                                worksheet.Cells[nextRow, 2].Value = (string)obj["IO_Define"];
                                nextRow = nextRow + 1;
                            }
                            nextRow = 6;
                            JArray json_fillArray_VN = JArray.Parse(Fill_json_VN);
                            foreach (JObject obj in json_fillArray_VN)
                            {
                                worksheet.Cells[nextRow, 3].Value = (string)obj["IO_Define"];
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
                        worksheet.Cells[5, 1].Value = "IO_Name";
                        worksheet.Cells[5, 2].Value = "IO_Define_EN";
                        worksheet.Cells[5, 3].Value = "IO_Define_VN";
                        int nextRow = 6;
                        JArray json_fillArray_EN = JArray.Parse(Fill_json_EN);
                        foreach (JObject obj in json_fillArray_EN)
                        {
                            worksheet.Cells[nextRow, 1].Value = (string)obj["IO_Name"];
                            worksheet.Cells[nextRow, 2].Value = (string)obj["IO_Define"];
                            nextRow = nextRow + 1;
                        }
                        nextRow = 6;
                        JArray json_fillArray_VN = JArray.Parse(Fill_json_VN);
                        foreach (JObject obj in json_fillArray_VN)
                        {
                            worksheet.Cells[nextRow, 3].Value = (string)obj["IO_Define"];
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
        public void Import_IO_Filepath()
        {
            string filePath;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Chọn file Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    Import_IO(filePath);
                }
            }
        }
        public void Import_IO(string filePath)
        {
            // System.DateTime dateTime = System.DateTime.Now;
            // string formattedDate = dateTime.ToString("dd/MM/yy");
            // string formattedtime = dateTime.ToString("HH:mm:ss");
            // string ID = formattedDate.Replace("/", "") + formattedtime.Replace(":", "");
            string Jsontemp_EN;
            string Jsontemp_VN;
            string Json_new_EN = "";
            string Json_new_VN = "";
            Items_IO List_IO_EN = new Items_IO();
            Items_IO List_IO_VN = new Items_IO();
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
                if (worksheet.Cells[5, 1].Text == "IO_Name")
                {
                    for (int row = 6; row <= worksheet.Dimension.Rows; row++)
                    {
                        List_IO_EN.IO_Name = worksheet.Cells[row, 1].Text;
                        List_IO_EN.IO_Define = worksheet.Cells[row, 2].Text;
                        List_IO_VN.IO_Name = worksheet.Cells[row, 1].Text;
                        List_IO_VN.IO_Define = worksheet.Cells[row, 3].Text;
                        Jsontemp_EN = JsonConvert.SerializeObject(List_IO_EN);
                        Jsontemp_VN = JsonConvert.SerializeObject(List_IO_VN);
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
                    File.WriteAllText(linkpath.GPIO_EN, Json_new_EN);
                    Json_new_VN = "[" + Json_new_VN + "]";
                    File.WriteAllText(linkpath.GPIO_VN, Json_new_VN);
                    System.Windows.MessageBox.Show("Đã nhập dữ liệu thành công!");
                }
                else
                {
                    System.Windows.MessageBox.Show("File nhập không đúng mẫu");
                }
                // Gán DataTable cho DataGridView
            }
        }
        public void Export_Chart_File(string File_Root_name, List<Position> dataPoint, string title)
        {
            string json = File.ReadAllText(linkpath.Chart);
            var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            string folderPath = open(title);

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
                //    worksheet.Cells[2, 1].Value = Data_Report_temp.OrderCode;
                //    worksheet.Cells[2, 2].Value = Data_Report_temp.Model;
                //    worksheet.Cells[2, 3].Value = Data_Report_temp.RotorID;
                //    worksheet.Cells[2, 4].Value = Data_Report_temp.TrucID;
                //    worksheet.Cells[2, 5].Value = Data_Report_temp.Beer_Up;
                //    worksheet.Cells[2, 6].Value = Data_Report_temp.Beer_Down;
                //    worksheet.Cells[2, 7].Value = Data_Report_temp.Jig_Up;
                //    worksheet.Cells[2, 8].Value = Data_Report_temp.Jig_Mid;
                //    worksheet.Cells[2, 9].Value = Data_Report_temp.Jig_Down;
                //    worksheet.Cells[2, 10].Value = Data_Report_temp.HStand;
                //    worksheet.Cells[2, 11].Value = Data_Report_temp.Force;
                //    worksheet.Cells[2, 12].Value = Data_Report_temp.Force_Max;

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
        public void Export_Report_All_File(string File_Root_name, string title, string datajson)
        {
            string json = datajson;
            string folderPath = open(title);

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
                    foreach (var data in Global.List_report)
                    {
                        worksheet.Cells[nextRow, 1].Value = "Time";
                        worksheet.Cells[nextRow, 2].Value = "Order_Code";
                        worksheet.Cells[nextRow, 3].Value = "Model";
                        worksheet.Cells[nextRow, 4].Value = "ID_Shaft";
                        worksheet.Cells[nextRow, 5].Value = "ID_Rotor";
                        worksheet.Cells[nextRow, 6].Value = "Force_max";
                        worksheet.Cells[nextRow, 7].Value = "Status";
                        worksheet.Cells[nextRow, 8].Value = "Position";
                        worksheet.Cells[nextRow, 9].Value = "Force";
                        nextRow++;
                        worksheet.Cells[nextRow, 1].Value = data.Time;
                        worksheet.Cells[nextRow, 2].Value = data.OrderCode;
                        worksheet.Cells[nextRow, 3].Value = data.Model;
                        worksheet.Cells[nextRow, 4].Value = data.ID_Shaft;
                        worksheet.Cells[nextRow, 5].Value = data.ID_Rotor;
                        worksheet.Cells[nextRow, 6].Value = data.Force_Max;
                        worksheet.Cells[nextRow, 7].Value = data.Status;
                        for (int i = 0; i < data.Chart.Count; i++)
                        {
                            worksheet.Cells[nextRow, 8].Value = data.Chart[i].X;
                            worksheet.Cells[nextRow, 9].Value = data.Chart[i].Y;
                            nextRow++;
                        }

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
        public static string CheckMode(string Mode, out int checkmode)
        {
            checkmode = 6;
            if (Mode != null)
            {
                switch (Mode)
                {
                    case "0":
                        checkmode = 0;
                        return "---";
                    case "1":
                        checkmode = 1;
                        return "Position";
                    case "2":
                        checkmode = 2;
                        return "Force";
                    case "3":
                        checkmode = 3;
                        return "Distance";
                    case "4":
                        checkmode = 4;
                        return "Force Position";
                    case "5":
                        checkmode = 5;
                        return "Force Distance";

                }
            }

            return "---";
        }
        private static bool Fill_Bearings_JigUD(string path, string id,out string ID_Jig,out float Thickness)
        {
            ID_Jig = "";
            Thickness = 0;
            try
            {
                string jsons = File.ReadAllText(path); ;
                if (jsons.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(jsons);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["ID"] == id)
                        {
                            ID_Jig = (string)obj["ID"];
                            Thickness = (float)obj["Thickness"];
                            return true;
                        }
                    }
                }
            }
            catch { }
           
            return false;
        }
        private static bool Fill_JigM(string path,  string id , out string ID_Jig)
        {

            ID_Jig = "";
            try
            {
                string jsons = File.ReadAllText(path); ;
                if (jsons.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(jsons);
                    foreach (JObject obj in jsonArray)
                    {

                        if ((string)obj["ID"] == id)
                        {
                            ID_Jig = (string)obj["ID"];
                            return true;
                        }

                    }
                }
            }
            catch {}
            return false;
        }
 

    }
}
