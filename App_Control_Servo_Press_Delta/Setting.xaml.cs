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

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        Common Common = new Common();
        Link_Path path = new Link_Path();

        Excel excel = new Excel();
        private DispatcherTimer timer;
        Update_Screen ud = new Update_Screen();
        private static bool is_Forcus = false;
        private static bool is_Forcus2 = false;
        PLC plc = new PLC();
        private static string old_language ="";
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

        public Setting()
        {
            InitializeComponent();
            Loaded += Setting_Loaded;
            Unloaded += Setting_Unloaded;
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
            Load_View(List_Upper_Jig,path.Jig_Up);
            Load_View(List_Middle_Jig, path.Jig_Mid);
            Load_View(List_Lower_Jig, path.Jig_Down);
            Common.Load_View_History(List_History_Error_Config);
        }
        private void Setting_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Tick -= Timer_Tick;
            timer.Stop();
            timer = null;
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
                    _language= Global.Language;
                    old_language = Global.Language;
                }    
                
            }
            catch
            {
            }
        }
        private void Update_Screen()
        {
            if (!is_Forcus)
            {

            }
            if (!is_Forcus2)
            {
                string json = File.ReadAllText(path.Setting);
                var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                string[] parts = SplitString(data_Setting["Server"], ':');
                TB_Server_IP.Text = parts[0];
                TB_Server_Port.Text = parts[1];
                string[] parts2 = SplitString(data_Setting["PLC"], ':');
                TB_PLC_IP.Text = parts2[0];
                TB_PLC_Port.Text = parts2[1];
                Global.PLC_IP = TB_PLC_IP.Text + ":" + TB_PLC_Port.Text;
                Global.Server = TB_Server_IP.Text + ":" + TB_Server_Port.Text;
            }
        }
        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            string datagridname = datagrid.Name;


            // Lấy dữ liệu từ hàng được chọn
            if (datagridname == "List_Upper_Jig")
            {
                var selectedRow = List_Upper_Jig.SelectedItem as DataView_Jig;
                if (selectedRow != null)
                {
                    var data_ID = selectedRow.ID;
                    tb_ID_JigU.Text = data_ID.ToString();
                }
            }
            if (datagridname == "List_Middle_Jig")
            {
                var selectedRow = List_Middle_Jig.SelectedItem as DataView_Jig;
                if (selectedRow != null)
                {

                    var data_ID = selectedRow.ID;
                    tb_ID_JigM.Text = data_ID.ToString();
                }
            }
            if (datagridname == "List_Lower_Jig")
            {
                var selectedRow = List_Lower_Jig.SelectedItem as DataView_Jig;
                if (selectedRow != null)
                {

                    var data_ID = selectedRow.ID;
                    tb_ID_JigD.Text = data_ID.ToString();
                }
            }
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
            if (textboxName == "tb_ID_JigU")
            {
                Fill_Value_Mode(path.Jig_Up, "List_Upper_Jig", textBox.Text);
            }
            if (textboxName == "tb_ID_JigM")
            {
                Fill_Value_Mode(path.Jig_Mid, "List_Middle_Jig", textBox.Text);
            }
            if (textboxName == "tb_ID_JigD")
            {
                Fill_Value_Mode(path.Jig_Down, "List_Lower_Jig", textBox.Text);
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
        public void Load_View(DataGrid dataGrid, string path)
        {
            string datagridname = dataGrid.Name;
            List<DataView_Jig> items = new List<DataView_Jig>();
            int index = 1;
            try
            {
                string List_Show = File.ReadAllText(path);
                if (List_Show.Length > 0)
                {
                    JArray List_Show_array = JArray.Parse(List_Show);
                    foreach (JObject obj in List_Show_array)
                    {
                        if (datagridname == "List_Upper_Jig" || datagridname == "List_Lower_Jig")
                        {
                            items.Add(new DataView_Jig { No = index, ID = (string)obj["ID"], Thickness = (string)obj["Thickness"] });
                            index++;
                        }
                        if (datagridname == "List_Middle_Jig")
                        {
                            items.Add(new DataView_Jig { No = index, ID = (string)obj["ID"] });
                            index++;
                        }
                    }
                    dataGrid.ItemsSource = items;
                }
            }
            catch
            {

            }
        }
        private void Click_BTN_Set_SysEdit(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
            {
                //  PLC.PLC_Read = TB_PLC_IP.Text;
                TB_Server_IP.IsReadOnly = false;
                TB_Server_Port.IsReadOnly = false;
                TB_PLC_IP.IsReadOnly = false;
                TB_PLC_Port.IsReadOnly = false;
                MessageBox.Show("Có thể cài đặt thông số ");
                is_Forcus2 = true;
            }
            else
            {
                MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical/ STI-Service");
            }
        }

        private void Click_BTN_Set_SysSave(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
            {
                string buttonName = ((Button)sender).Name;
            object data_Setting = new
            {
                Server = TB_Server_IP.Text + ":" + TB_Server_Port.Text,
                PLC_IP = TB_PLC_IP.Text + ":" + TB_PLC_Port.Text,
            };
            string json = System.Text.Json.JsonSerializer.Serialize(data_Setting);
            File.WriteAllText(path.Setting, json);
            // PLC.PLC_Read = TB_PLC_IP.Text;
            // PLC.PLC_Write = TB_PLC_IP.Text;
            MessageBox.Show("Đã Lưu Thành Công");
            TB_Server_IP.IsReadOnly = true;
            TB_Server_Port.IsReadOnly = true;
            TB_PLC_IP.IsReadOnly = true;
            TB_PLC_Port.IsReadOnly = true;
            plc.StartTimer();

            is_Forcus2 = false;
        }
            else
            {
                MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical/ STI-Service");
            }
}
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return true;

        }

        private void Click_Para_Save(object sender, RoutedEventArgs e)
        {
            if (AreTextBoxesFilled())
            {
                MessageBoxResult result = MessageBox.Show("Xác Nhận Lưu Thông Số Máy", "Warring", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    var data = new
                    {

                        // Limit_Y1 = TB_LimitYUp.Text
                    };
                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);


                    // TB_LimitYUp.IsReadOnly = true;
                    is_Forcus = false;
                    MessageBox.Show("Lưu thông số cài đặt thành công");
                }
                else
                {
                    MessageBox.Show("Lưu thông số cài đặt thất bại");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông số");
            }
        }
        private void Click_Para_Edit(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName == "STI-Technical")
            {


            }
            if (MainWindow.UserName != "")
            {

            }
            MessageBox.Show("Có thể cài đặt thông số");
            is_Forcus = true;
        }
        private void Fill_Value_Mode(string path, string NameDatagrid, string id)
        {
            string jsons = File.ReadAllText(path); ;
            if (jsons.Length > 0)
            {
                JArray jsonArray = JArray.Parse(jsons);
                foreach (JObject obj in jsonArray)
                {
                    if (NameDatagrid == "List_Upper_Jig" & (string)obj["ID"] == id)
                    {
                        tb_ID_JigU.Text = (string)obj["ID"];
                        tb_Thichness_JigU.Text = (string)obj["Thickness"];
                        break;
                    }
                    if (NameDatagrid == "List_Middle_Jig" & (string)obj["ID"] == id)
                    {
                        tb_ID_JigM.Text = (string)obj["ID"];
                        break;
                    }
                    if (NameDatagrid == "List_Lower_Jig" & (string)obj["ID"] == id)
                    {
                        tb_ID_JigD.Text = (string)obj["ID"];
                        tb_Thichness_JigD.Text = (string)obj["Thickness"];
                        break;
                    }
                }
            }
        }
        private void Save_JigU()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            string json = File.ReadAllText(path.Jig_Up);
            System.DateTime dateTime = System.DateTime.Now;
            List_Data List = new List_Data();
            List.ID = tb_ID_JigU.Text;
            List.Thickness = float.Parse(tb_Thichness_JigU.Text);
            string list_Json = JsonConvert.SerializeObject(List);
            try
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.ID == tb_ID_JigU.Text)
                    {
                       // item.ID = tb_ID_JigU.Text;
                        item.Thickness = float.Parse(tb_Thichness_JigU.Text); 

                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(path.Jig_Up, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json.Length < 10)
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + list_Json + "]";
                        File.WriteAllText(path.Jig_Up, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                    else
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + "," + list_Json + "]";
                        File.WriteAllText(path.Jig_Up, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                }

            }
            catch
            {
                string jsons;
                jsons = "[" + list_Json + "]";
                File.WriteAllText(path.Jig_Up, jsons);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            json = File.ReadAllText(path.Jig_Up);
            Common.Load_View(List_Upper_Jig, path.Jig_Up);
        }


        private void Save_JigM()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            string json = File.ReadAllText(path.Jig_Mid);
            System.DateTime dateTime = System.DateTime.Now;
            List_Data List = new List_Data();
            List.ID = tb_ID_JigM.Text;
            //List.Thickness = float.Parse(tb_Thichness_JigU.Text);
            string list_Json = JsonConvert.SerializeObject(List);
            try
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.ID == tb_ID_JigM.Text)
                    {
                        // item.ID = tb_ID_JigU.Text;
                      //  item.Thickness = float.Parse(tb_Thichness_JigU.Text);

                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(path.Jig_Mid, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json.Length < 10)
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + list_Json + "]";
                        File.WriteAllText(path.Jig_Mid, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                    else
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + "," + list_Json + "]";
                        File.WriteAllText(path.Jig_Mid, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                }

            }
            catch
            {
                string jsons;
                jsons = "[" + list_Json + "]";
                File.WriteAllText(path.Jig_Mid, jsons);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            json = File.ReadAllText(path.Jig_Up);
            Common.Load_View(List_Middle_Jig, path.Jig_Mid);
        }

        private void Save_JigD()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            string json = File.ReadAllText(path.Jig_Down);
            System.DateTime dateTime = System.DateTime.Now;
            List_Data List = new List_Data();
            List.ID = tb_ID_JigD.Text;
            List.Thickness = float.Parse(tb_Thichness_JigD.Text);
            string list_Json = JsonConvert.SerializeObject(List);
            try
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.ID == tb_ID_JigD.Text)
                    {
                        // item.ID = tb_ID_JigU.Text;
                          item.Thickness = float.Parse(tb_Thichness_JigD.Text);

                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(path.Jig_Down, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json.Length < 10)
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + list_Json + "]";
                        File.WriteAllText(path.Jig_Down, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                    else
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + "," + list_Json + "]";
                        File.WriteAllText(path.Jig_Down, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                }

            }
            catch
            {
                string jsons;
                jsons = "[" + list_Json + "]";
                File.WriteAllText(path.Jig_Down, jsons);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            json = File.ReadAllText(path.Jig_Down);
            Common.Load_View(List_Lower_Jig, path.Jig_Down);
        }

        private void Clear_JigU()
        {
            string jsons = File.ReadAllText(path.Jig_Up);
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Jig: " + tb_ID_JigU.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & tb_ID_JigU.Text.Length > 0)
            {

                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(jsons, options);

                var newData = new List<List_Temp>();

                foreach (var item in data)
                {
                    if (item.ID != tb_ID_JigU.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(path.Jig_Up, newJsonString);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + tb_ID_JigU.Text + " cần xóa");
            }

            Common.Load_View(List_Upper_Jig, path.Jig_Up);
        }
        private void Clear_JigM()
        {
            string jsons = File.ReadAllText(path.Jig_Mid);
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Jig: " + tb_ID_JigM.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & tb_ID_JigM.Text.Length > 0)
            {

                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(jsons, options);

                var newData = new List<List_Temp>();

                foreach (var item in data)
                {
                    if (item.ID != tb_ID_JigM.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(path.Jig_Mid, newJsonString);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + tb_ID_JigM.Text + " cần xóa");
            }

            Common.Load_View(List_Middle_Jig, path.Jig_Mid);
        }

        private void Clear_JigD()
        {
            string jsons = File.ReadAllText(path.Jig_Down);
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Jig: " + tb_ID_JigD.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & tb_ID_JigD.Text.Length > 0)
            {

                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(jsons, options);

                var newData = new List<List_Temp>();

                foreach (var item in data)
                {
                    if (item.ID != tb_ID_JigD.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(path.Jig_Down, newJsonString);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + tb_ID_JigD.Text + " cần xóa");
            }

            Common.Load_View(List_Lower_Jig, path.Jig_Down);
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
            catch
            {
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
        private void Fill_Value_His()
        {

            string json_EN = File.ReadAllText(path.History_EN);
            string json_VN = File.ReadAllText(path.History_VN);
            if (json_EN.Length > 0  )
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
        public static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }
        private static bool Is_String(string input, string Compari_1)
        {
            return input.Contains(Compari_1);
        }

        private void btn_import_JigU_Click(object sender, RoutedEventArgs e)
        {
            excel.Import_BJ_Filepath("Model_Beer_Up", path.Beer_Up);
            Common.Load_View(List_Upper_Jig, path.Jig_Up);
        }

        private void btn_export_JigU_Click(object sender, RoutedEventArgs e)
        {
            excel.Export_BJ_File("Template_Model_Jig_Up", path.Jig_Up, "Model_Jig_Up");
        }

        private void btn_import_JigM_Click(object sender, RoutedEventArgs e)
        {
            excel.Import_BJ_Filepath("Model_Jig_Mid", path.Jig_Mid);
            Common.Load_View(List_Middle_Jig, path.Jig_Mid);
        }

        private void btn_export_JigM_Click(object sender, RoutedEventArgs e)
        {
            excel.Export_BJ_File("Template_Model_Jig_Mid", path.Jig_Mid, "Model_Jig_Mid");
        }

        private void btn_import_JigD_Click(object sender, RoutedEventArgs e)
        {
            excel.Import_BJ_Filepath("Model_Jig_Down", path.Jig_Down);
            Common.Load_View(List_Lower_Jig, path.Jig_Down);
        }

        private void btn_export_JigD_Click(object sender, RoutedEventArgs e)
        {

                excel.Export_BJ_File("Template_Model_Jig_Down", path.Jig_Down, "Model_Jig_Down");



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
            excel.Export_History_File("Template_History");
        }

        private void btn_Del_JigU_Click(object sender, RoutedEventArgs e)
        {
            Clear_JigU();
        }

        private void btn_Save_JigU_Click(object sender, RoutedEventArgs e)
        {
            Save_JigU();
        }

        private void btn_Del_JigM_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_JigM_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Del_JigD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_JigD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Del_HisE_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_HisE_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Set_SysEdit_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
            {

                //  PLC.PLC_Read = TB_PLC_IP.Text;
                TB_Server_IP.IsReadOnly = false;
                TB_Server_Port.IsReadOnly = false;
                TB_PLC_IP.IsReadOnly = false;
                TB_PLC_Port.IsReadOnly = false;

                is_Forcus2 = true;
            }
        }

        private void btn_Set_SysSave_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
            {


                object data_Setting = new
                {
                    Server = TB_Server_IP.Text + ":" + TB_Server_Port.Text,
                    PLC = TB_PLC_IP.Text + ":" + TB_PLC_Port.Text
                };
                string json = System.Text.Json.JsonSerializer.Serialize(data_Setting);
                File.WriteAllText(path.Setting, json);
                // PLC.PLC_Read = TB_PLC_IP.Text;
                // PLC.PLC_Write = TB_PLC_IP.Text;
                MessageBox.Show("Đã Lưu Thành Công");
                TB_Server_IP.IsReadOnly = true;
                TB_Server_Port.IsReadOnly = true;
                TB_PLC_IP.IsReadOnly = true;
                TB_PLC_Port.IsReadOnly = true;
                is_Forcus2 = false; 
            }
        }

        private void btn_off_bz_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
