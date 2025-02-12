using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using System.Windows.Threading;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for History_Config.xaml
    /// </summary>
    public partial class History_Config : Window
    {
        List<List_History> List_History = new List<List_History>();
        Excel excel = new Excel();
        System.DateTime dateTime = System.DateTime.Now;
        private DispatcherTimer timer;
        private static ushort Value_temp;
        public History_Config()
        {
            InitializeComponent();
            Loaded += History_Loaded;  // Thêm sự kiện Loaded
            Unloaded += History_Unloaded;
            var workingArea = SystemParameters.WorkArea;

            // Đặt kích thước và vị trí của cửa sổ
            this.Left = workingArea.Left;
            this.Top = workingArea.Top;
            this.Width = workingArea.Width + 5;
            this.Height = workingArea.Height + 5;
        }
        private void History_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();

            YourMethod();
            Common.Load_View_History(List_History_CG);
            // List_Err1 = History_UL.GetAllUsers();
        }

        private void History_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            // if (Data.Error > 0)
            // {
            //
            // }

        }
        Link_Path linkpath = new Link_Path();
        Common Common = new Common();
        private void YourMethod()
        {

            List_History_CG.AddHandler(DataGrid.SelectionChangedEvent, new SelectionChangedEventHandler(History_SelectionChanged));
        }

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRow = List_History_CG.SelectedItem as DataView_History;
            if (selectedRow != null)
            {
                // Lấy dữ liệu từ hàng được chọn

                var data_Code = selectedRow.Code;
                TB_List_Code.Text = data_Code.ToString();
            }
            Fill_Value_His();
        }
        private void Save_His()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            List_History List_History = new List_History();
            List_History.STT = 0;
            List_History.Code = TB_List_Code.Text;
            List_History.Content_ = TB_List_content_.Text;
            List_History.Solution = TB_List_Solution.Text;
            List_History.Time = dateTime.ToString();
            string list_His_Json = JsonConvert.SerializeObject(List_History);
            try
            {
                string json = File.ReadAllText(linkpath.History);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_History_temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.Code == TB_List_Code.Text)
                    {
                        item.STT = 0;
                        item.Code = TB_List_Code.Text;
                        item.Content_ = TB_List_content_.Text;
                        item.Solution = TB_List_Solution.Text;
                        item.Time = dateTime.ToString();
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(linkpath.History, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    json = json.Remove(json.Length - 1);
                    json = json + "," + list_His_Json + "]";
                    File.WriteAllText(linkpath.History, json);
                    MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                }
            }
            catch
            {
                string json_;
                json_ = "[" + list_His_Json + "]";
                File.WriteAllText(linkpath.History, json_);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            Common.Load_View_History(List_History_CG);
            //           }
            //            else
            //            {
            //               MessageBox.Show("Giá trị tốc độ không phù hợp, giá trị hợp lệ trong khoảng 3-20");
            //           }

        }

        private void Clear_His()
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã lỗi: " + TB_List_Code.Text, "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & TB_List_Code.Text.Length > 0)
            {

                string json = File.ReadAllText(linkpath.History);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_History_temp[]>(json, options);

                var newData = new List<List_History_temp>();

                foreach (var item in data)
                {
                    if (item.Code != TB_List_Code.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(linkpath.History, newJsonString);
                Common.Load_View_History(List_History_CG);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Lỗi: " + TB_List_Code.Text + " cần xóa");
            }

        }
        private void Fill_Value_His()
        {

            string json = File.ReadAllText(linkpath.History);
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["Code"] == TB_List_Code.Text)
                    {
                        TB_List_Code.Text = (string)obj["Code"];
                        TB_List_content_.Text = (string)obj["Content_"];
                        TB_List_Solution.Text = (string)obj["Solution"];
                    }


                }

            }
        }
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return !string.IsNullOrWhiteSpace(TB_List_Code.Text) &&
                   !string.IsNullOrWhiteSpace(TB_List_content_.Text) &&
                   !string.IsNullOrWhiteSpace(TB_List_Solution.Text);
        }

        private void bt_Import_Click(object sender, RoutedEventArgs e)
        {
            excel.Import_History_Filepath();
            Common.Load_View_History(List_History_CG);
        }

        private void bt_Export_Click(object sender, RoutedEventArgs e)
        {
            excel.Export_History_File("Template_History");
        }

        private void bt_Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear_His();
        }

        private void bt_Save_Click(object sender, RoutedEventArgs e)
        {
            if ((TB_List_Code.Text == ""))
            {
                // Nếu là số, xóa thông báo lỗi
                MessageBox.Show("Vui Lòng nhập mã lỗi");
            }
            else if (!AreTextBoxesFilled())
            {
                MessageBox.Show("Vui Lòng nhập đầy đủ thông tin còn thiếu");
            }
            else
            {
                Save_His();
            }
        }

        private void bt_Exit_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MouseDown_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void exitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
    }
}
