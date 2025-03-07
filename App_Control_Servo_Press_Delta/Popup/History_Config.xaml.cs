using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Ports;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Media;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using Newtonsoft.Json.Linq;
using System.Net.Security;
using System.Text.Json;
using System.ComponentModel;
using System.Windows.Input;
using ViewModel;
namespace App_Control_Servo_Press_Delta.Popup
{
    /// <summary>
    /// Interaction logic for HisE_config.xaml
    /// </summary>
    public partial class History_Config : Window
    {
        Common Common = new Common();
        Link_Path path = new Link_Path();

        Excel excel = new Excel();
        private DispatcherTimer timer;
        Update_Screen ud = new Update_Screen();
        private static bool is_Forcus = false;
        private static bool is_Forcus2 = false;
        PLC plc = new PLC();
        private static string old_language = "";
        private string _language;

        public string language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged(nameof(Header_No));
            }
        }

        public string Header_No => language == "English" ? "No" : "STT";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public History_Config()
        {
            InitializeComponent();
            Loaded += Setting_Loaded;
            Unloaded += Setting_Unloaded;
            DataContext = new MainWindow_VM();
        }
        private void Setting_Loaded(object sender, RoutedEventArgs e)
        {

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click += Button_Click;
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }
            foreach (var dataGrid in Common.FindVisualChildren<DataGrid>(this))
            {
                dataGrid.SelectionChanged += Model_SelectionChanged;
            }

            Common.Load_View_History(List_History_Error_Config);

        }
        private void Setting_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop(); // Dừng timer
                timer.Tick -= Timer_Tick; // Hủy đăng ký sự kiện
            }
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click -= Button_Click;
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
            }
            foreach (var dataGrid in Common.FindVisualChildren<DataGrid>(this))
            {
                dataGrid.SelectionChanged -= Model_SelectionChanged;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Update_Screen();
                });
                if (old_language != Global.Language)
                {
                    Common.Load_View_History(List_History_Error_Config);
                    _language = Global.Language;
                    old_language = Global.Language;
                }

            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
            }
        }
        private void Update_Screen()
        {

        }
        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            string datagridname = datagrid.Name;


            // Lấy dữ liệu từ hàng được chọn

            if (datagridname == "List_History_Error_Config")
            {

                var selectedRow = List_History_Error_Config.SelectedItem as DataView_History;
                if (selectedRow != null)
                {

                    var data = selectedRow.Code;
                    tb_code.Text = data.ToString();
                }
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            // NumPad numberPad = new NumPad(textBox);
            //numberPad.ShowDialog(); // Hiển thị cửa sổ như hộp thoại
            if (!double.TryParse(textBox.Text, out _) & (textBox.Text != "") & (textboxName != "TB_PLC_Port") & (textboxName != "TB_PLC_IP") & (textboxName != "TB_Server_IP") & (textboxName != "TB_Server_Port"))
            {
                // Nếu là số, xóa thông báo lỗi
                // MessageBox.Show("Vui Lòng nhập lại dữ liệu kiểu số");
                // textBox.Text = "";
            }

            if (textboxName == "tb_code")
            {
                Fill_Value_His();
            }

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string buttonName = ((Button)sender).Name;
            string PopupName = buttonName.Substring(4);

        }


        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return true;

        }



        private void Save_His()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            System.DateTime dateTime = System.DateTime.Now;
            List_History List_History_EN = new List_History();
            List_History List_History_VN = new List_History();
            List_History_EN.STT = 0;
            List_History_EN.Code = tb_code.Text;
            List_History_EN.Description = tb_DescriptionEN.Text;
            List_History_EN.Solution = tb_SolutionEn.Text;
            List_History_EN.Time = dateTime.ToString();
            List_History_VN.STT = 0;
            List_History_VN.Code = tb_code.Text;
            List_History_VN.Description = tb_DescriptionEN.Text;
            List_History_VN.Solution = tb_SolutionEn.Text;
            List_History_VN.Time = dateTime.ToString();
            string list_His_Json_EN = JsonConvert.SerializeObject(List_History_EN);
            string list_His_Json_VN = JsonConvert.SerializeObject(List_History_VN);
            try
            {
                string json_EN = File.ReadAllText(path.History_EN);
                string json_VN = File.ReadAllText(path.History_VN);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data_EN = System.Text.Json.JsonSerializer.Deserialize<List_History_temp[]>(json_EN, options);
                var data_VN = System.Text.Json.JsonSerializer.Deserialize<List_History_temp[]>(json_VN, options);
                float flag = 0;
                foreach (var item in data_EN)
                {

                    if (item.Code == tb_code.Text)
                    {
                        item.STT = 0;
                        item.Code = tb_code.Text;
                        item.Description = tb_DescriptionEN.Text;
                        item.Solution = tb_SolutionEn.Text;
                        item.Time = dateTime.ToString();
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data_EN, jsonOptions);
                        File.WriteAllText(path.History_EN, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    json_EN = json_EN.Remove(json_EN.Length - 1);
                    json_EN = json_EN + "," + list_His_Json_EN + "]";
                    File.WriteAllText(path.History_EN, json_EN);
                    MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                }
                foreach (var item in data_VN)
                {

                    if (item.Code == tb_code.Text)
                    {
                        item.STT = 0;
                        item.Code = tb_code.Text;
                        item.Description = tb_DescriptionVN.Text;
                        item.Solution = tb_SolutionVN.Text;
                        item.Time = dateTime.ToString();
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data_VN, jsonOptions);
                        File.WriteAllText(path.History_VN, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    json_VN = json_VN.Remove(json_VN.Length - 1);
                    json_VN = json_VN + "," + list_His_Json_VN + "]";
                    File.WriteAllText(path.History_EN, json_VN);
                    MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                }
            }
            catch (Exception e)
            {
                Common.Log_err(e.ToString());
        
            string json_;
                json_ = "[" + list_His_Json_EN + "]";
                File.WriteAllText(path.History_EN, json_);
                json_ = "[" + list_His_Json_VN + "]";
                File.WriteAllText(path.History_VN, json_);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            Common.Load_View_History(List_History_Error_Config);
            //           }
            //            else
            //            {
            //               MessageBox.Show("Giá trị tốc độ không phù hợp, giá trị hợp lệ trong khoảng 3-20");
            //           }

        }

        private void Clear_His()
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã lỗi: " + tb_code.Text, "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);
            try
            {
                if (result == MessageBoxResult.Yes & tb_code.Text.Length > 0)
                {

                    string json_EN = File.ReadAllText(path.History_EN);
                    string json_VN = File.ReadAllText(path.History_VN);
                    var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                    var data_EN = System.Text.Json.JsonSerializer.Deserialize<List_History_temp[]>(json_EN, options);
                    var data_VN = System.Text.Json.JsonSerializer.Deserialize<List_History_temp[]>(json_VN, options);

                    var newData = new List<List_History_temp>();

                    foreach (var item in data_EN)
                    {
                        if (item.Code != tb_code.Text)
                        {
                            newData.Add(item);
                        }
                    }
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                    // Write back to file
                    File.WriteAllText(path.History_EN, newJsonString);
                    foreach (var item in data_VN)
                    {
                        if (item.Code != tb_code.Text)
                        {
                            newData.Add(item);
                        }
                    }
                    newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                    // Write back to file
                    File.WriteAllText(path.History_VN, newJsonString);

                    Common.Load_View_History(List_History_Error_Config);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy mã Lỗi: " + tb_code.Text + " cần xóa");
                }
            }
            catch (Exception e)
            {
                Common.Log_err(e.ToString());
            }

        }
        private void Fill_Value_His()
        {
            try
            {
                string json_EN = File.ReadAllText(path.History_EN);
                string json_VN = File.ReadAllText(path.History_VN);
                if (json_EN.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(json_EN);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["Code"] == tb_code.Text)
                        {
                            tb_code.Text = (string)obj["Code"];
                            tb_DescriptionEN.Text = (string)obj["Description"];
                            tb_SolutionEn.Text = (string)obj["Solution"];
                        }


                    }

                }
                if (json_VN.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(json_VN);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["Code"] == tb_code.Text)
                        {
                            tb_code.Text = (string)obj["Code"];
                            tb_DescriptionVN.Text = (string)obj["Description"];
                            tb_SolutionVN.Text = (string)obj["Solution"];
                        }


                    }

                }
            }
            catch (Exception e)
            {
                Common.Log_err(e.ToString());
            }
        }
        public static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }
        private static bool Is_String(string input, string Compari_1)
        {
            return input.Contains(Compari_1);
        }


        private void btn_import_HisE_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
            {
                excel.Import_History_Filepath();
                Common.Load_View_History(List_History_Error_Config);
            }
            else MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical hoặc STI-Service để cấu hình");
        }

        private void btn_export_HisE_Click(object sender, RoutedEventArgs e)
        {
            excel.Export_History_File("Template_History", "Chọn thư mục lưu file", "History");
        }

        private void btn_Del_HisE_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
            {
                Clear_His();
            }
            else MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical / STI-Service để cài đặt!");

        }
        private void btn_Save_HisE_Click(object sender, RoutedEventArgs e)
        {


            if (tb_code.Text != "" & tb_DescriptionEN.Text != "" & tb_DescriptionVN.Text != "" & tb_SolutionEn.Text != "" & tb_SolutionVN.Text != "")
            {
                if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service" || Is_String(MainWindow.UserName, "Admin"))
                {
                    MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn Lưu mã Lỗi: " + tb_code.Text + " ?", "Xác nhận Lưu", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Save_His();
                    }
                    else
                    {
                        MessageBox.Show("Lưu model không thành công!");
                    }
                }
                else
                {

                    MessageBox.Show("Vui Lòng đăng nhập quyền cao nhất!");
                }

            }
            else
            {
                MessageBox.Show("Vui Lòng nhập đầy đủ thông số");
            }
        }


    }
}
