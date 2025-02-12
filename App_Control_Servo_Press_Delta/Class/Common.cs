using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using static App_Control_Servo_Press_Delta.LoginWindow;
using System.Text.Json;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using System.Diagnostics;

namespace App_Control_Servo_Press_Delta
{

    public class Link_Path
    {
        public string Setting = System.IO.Path.Combine("Path", "scnn.ini");
        public string Model = System.IO.Path.Combine("Model", "Model.json");
        public string Beer_Up = System.IO.Path.Combine("Model", "Beer_Up.json");
        public string Beer_Down = System.IO.Path.Combine("Model", "Beer_Down.json");
        public string Jig_Up = System.IO.Path.Combine("Model", "Jig_Up.json");
        public string Jig_Mid = System.IO.Path.Combine("Model", "Jig_Mid.json");
        public string Jig_Down = System.IO.Path.Combine("Model", "Jig_Down.json");
        public string Error = System.IO.Path.Combine("Path", "Error.json");
        public string History = System.IO.Path.Combine("Path", "History.json");
        public string Alarm = System.IO.Path.Combine("Path", "Alarm.json");
        public string List_Error = System.IO.Path.Combine("Path", "List_Error.ini");
        public string List_Alarm = System.IO.Path.Combine("Path", "List_Alarm.ini");
        public string User_List = System.IO.Path.Combine("Path", "UserCredentials.json");
        public string GPIO = System.IO.Path.Combine("Path", "GPIO.json");
        public string Time_work = System.IO.Path.Combine("Path", "Time_Work.json");
        public string Chart = System.IO.Path.Combine("Path", "Chart.json");
    }
    public class ColorChecker
    {
        public static bool IsColorEqual(Brush brush, Color targetColor)
        {
            if (brush != null)
            {
                if (brush is SolidColorBrush solidColorBrush)
                {
                    Color brushColor = solidColorBrush.Color;
                    return brushColor.R == targetColor.R && brushColor.G == targetColor.G && brushColor.B == targetColor.B;
                }
                // Handle other types of brushes if needed
            }

            return false;
        }
    }
    public class Common
    {

        Link_Path linkpath = new Link_Path();
        public void Load_View_Model(DataGrid dataGrid)
        {
            List<DataView_Model> items = new List<DataView_Model>();
            int index = 1;
            try
            {
                string List_Show = File.ReadAllText(linkpath.Model);
                if (List_Show.Length > 0)
                {
                    JArray List_Show_array = JArray.Parse(List_Show);
                    foreach (JObject obj in List_Show_array)
                    {
                        items.Add(new DataView_Model { STT = index, RotorID = (string)obj["RotorID"], Model = (string)obj["Model"], TrucID = (string)obj["TrucID"] });
                        index++;
                    }
                    dataGrid.ItemsSource = items;
                }
            }
            catch
            {

            }
        }
        public void Load_View_History(DataGrid dataGrid)
        {
            List<DataView_History> items = new List<DataView_History>();
            int index = 1;
            try
            {
                string List_Show = File.ReadAllText(linkpath.History);
                if (List_Show.Length > 0)
                {
                    JArray List_Show_array = JArray.Parse(List_Show);
                    foreach (JObject obj in List_Show_array)
                    {
                        items.Add(new DataView_History { STT = index, Code = (string)obj["Code"], Content_ = (string)obj["Content_"], Solution = (string)obj["Solution"] });
                        index++;
                    }
                    dataGrid.ItemsSource = items;
                }
            }
            catch
            {

            }
        }
        public void Load_View(DataGrid dataGrid, string path)
        {
            List<DataView> items = new List<DataView>();
            int index = 1;
            try
            {
                string List_Show = File.ReadAllText(path);
                if (List_Show.Length > 0)
                {
                    JArray List_Show_array = JArray.Parse(List_Show);
                    foreach (JObject obj in List_Show_array)
                    {
                        items.Add(new DataView { STT = index, ID = (string)obj["ID"] });
                        index++;
                    }
                    dataGrid.ItemsSource = items;
                }
            }
            catch
            {

            }
        }
        public void Load_View_Report(DataGrid dataGrid, string time)
        {
            // List<DataView_Report> items = new List<DataView_Report>();
            Global.List_report = new List<DataView_Report>();
            Global.List_report_all = new List<Data_Report_all>();
            Global.List_report_temp = new List<DataView_Report>();
            int index = 1;
            try
            {
                string filepath = time + "_Report.json";
                string List_Show = File.ReadAllText(System.IO.Path.Combine("Log", filepath));
                if (List_Show.Length > 0)
                {
                    JArray List_Show_array = JArray.Parse(List_Show);
                    foreach (JObject obj in List_Show_array)
                    {
                        Global.List_report.Add(new DataView_Report { STT = index, Model = (string)obj["Model"], TrucID = (string)obj["TrucID"], RotorID = (string)obj["RotorID"], Force_Max = (string)obj["Force_Max"], Force = (string)obj["Force"], Time = (string)obj["Time"] });
                        Global.List_report_all.Add(new Data_Report_all { STT = index, OrderCode = (string)obj["OrderCode"], Model = (string)obj["Model"], TrucID = (string)obj["TrucID"], RotorID = (string)obj["RotorID"], Beer_Up = (string)obj["Beer_Up"], Beer_Down = (string)obj["Beer_Down"], Jig_Up = (string)obj["Jig_Up"], Jig_Mid = (string)obj["Jig_Mid"], Jig_Down = (string)obj["Jig_Down"], HStand = (string)obj["HStand"], Force_Max = (string)obj["Force_Max"], Force = (string)obj["Force"], Position = (string)obj["Position"], Time = (string)obj["Time"] });
                        index++;
                    }
                    //  dataGrid.ItemsSource = null;
                    //  dataGrid.ItemsSource = Global.List_report;
                    //   MessageBox.Show(items.ToString());
                }
            }
            catch
            {
                dataGrid.ItemsSource = null;
                //MessageBox.Show("Lỗi mở file");
            }
        }

        public void SetEmptyTextBoxToZero(TextBox TextBox)
        {
            foreach (var textBox in FindVisualChildren<TextBox>(TextBox))
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "0";
                }
            }
        }
        public IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }
                else
                {
                    var result = FindVisualChildren<T>(child);
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
        public static IEnumerable<T> FindVisualChildren2<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    yield return typedChild;
                }

                foreach (var grandChild in FindVisualChildren2<T>(child))
                {
                    yield return grandChild;
                }
            }
        }
        public static string Search_IO(string IO_Name)
        {
            string json = File.ReadAllText(System.IO.Path.Combine("Path", "GPIO.json"));
            // string IO_Define;
            if (json.Length > 0)
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<Items_IO_temp[]>(json, options);
                foreach (var item in data)
                {
                    if (item.IO_Name == IO_Name.Substring(2))
                    {

                        return item.IO_Define;
                        break;
                    }
                }
            }
            return "---";
        }
        public void Edit_IO(string IO_Name, string IO_Define)
        {
            string json = File.ReadAllText(linkpath.GPIO);
            Items_IO Items_IO_ = new Items_IO();
            //try
            //{
            if (json.Length > 0)
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<Items_IO_temp[]>(json, options);
                foreach (var item in data)
                {
                    if (item.IO_Name == IO_Name.Substring(2))
                    {
                        //  item.IO_Name = IO_Name;
                        item.IO_Define = IO_Define;
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(linkpath.GPIO, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        break;
                    }
                }
            }

        }
        public void Log_data(string name_screen, string textboxname, string value_old, string value_new)
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            if (value_old != value_new)
            {
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine("Log", formattedDate.Replace("/", "_") + "_Log_data.txt"), true)) // true để thêm
                {
                    writer.WriteLine(formattedtime + ", Name_screen: " + name_screen + ", Textboxname: " + textboxname + ", value_old: " + value_old + ", value_new: " + value_new);
                }
            }
        }
        public void Log_err(string name_screen, string Function, string status)
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine("Log", formattedDate.Replace("/", "_") + "_Tag_Err.txt"), true)) // true để thêm
            {
                writer.WriteLine(formattedtime + ", Name_screen: " + name_screen + ", Function: " + Function + ", status: " + status);
            }

        }
        public void Log_Connect(string status)
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine("Log", formattedDate.Replace("/", "_") + "_Log_Connect.txt"), true)) // true để thêm
            {
                writer.WriteLine(formattedtime + ", PLC: " + status);
            }

        }
        public void Open_KeyBoard()
        {
            try
            {
                Process[] keyboardProcesses = Process.GetProcessesByName("osk");

                // Nếu Bàn phím trên màn hình không chạy và Command Prompt không chạy
                if (keyboardProcesses.Length == 0)
                {
                    Process process = Process.Start(new ProcessStartInfo(((Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\osk.exe"))));
                }
            }
            catch { }

        }
    }

}
