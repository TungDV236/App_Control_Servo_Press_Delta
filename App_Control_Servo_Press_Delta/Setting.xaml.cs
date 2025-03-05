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
using App_Control_Servo_Press_Delta.Popup;

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
            BTN_Setting_MCancel.Visibility = Visibility.Hidden;


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
 
                
            }
            catch
            {
            }
        }
        private void Update_Screen()
        {
            if (!is_Forcus)
            {
                tb_Base_jig_thickness.Text= Data.Height_Jig_Base.ToString();
                tb_Ofset_Shaft_Machine.Text = Data.ofset_Machine.ToString();
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

          

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Popup_BearingsUp Model_Beer_Up = new Popup_BearingsUp();
            Popup_BearingsDown Model_Beer_Down = new Popup_BearingsDown();
            Popup_JigUp Model_Jig_Up = new Popup_JigUp();
            Popup_JigMid Model_Jig_Mid = new Popup_JigMid();
            Popup_JigDown Model_Jig_Down = new Popup_JigDown();
            History_Config History_Config = new History_Config();
            string buttonName = ((Button)sender).Name;
            string PopupName = buttonName.Substring(4);
            if (Is_String(buttonName, "Set_Model"))
            {
                if (PopupName == "Model_Beer_Up")
                {
                    Model_Beer_Up.ShowDialog();
                }
                else if (PopupName == "Model_Beer_Down")
                {
                    Model_Beer_Down.ShowDialog();
                }
                else if (PopupName == "Model_Jig_Up")
                {
                    Model_Jig_Up.ShowDialog();
                }
                else if (PopupName == "Model_Jig_Mid")
                {
                    Model_Jig_Mid.ShowDialog();
                }
                else if (PopupName == "Model_Jig_Down")
                {
                    Model_Jig_Down.ShowDialog();
                }
                //  MessageBox.Show("bạn cần thêm mới model"+ comboBoxName);
            }
            else if (PopupName == "History")
            {
                History_Config.ShowDialog();
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



   
        public static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }
        private static bool Is_String(string input, string Compari_1)
        {
            return input.Contains(Compari_1);
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
            else MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical / STI-Service để cài đặt!");
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
            else MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical / STI-Service để cài đặt!");
        }



        private void btn_Set_ParaEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!Global.Pressing)
            {
                if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service")
                {

                    tb_Ofset_Shaft_Machine.IsReadOnly = false;
                    tb_Base_jig_thickness.IsReadOnly = false;

                }
                if (MainWindow.UserName != "")
                {

                    tb_Ofset_Shaft_Machine.IsReadOnly = true;
                    tb_Base_jig_thickness.IsReadOnly = true;
                }

                BTN_Setting_MCancel.Visibility = Visibility.Visible;
                is_Forcus = true;
            }
            else
            {
                MessageBox.Show("Máy đang hoạt dộng");
            }
        }

        private void btn_Set_ParaSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Global.Pressing)
            {
                if (AreTextBoxesFilled())
                {
                    MessageBoxResult result = MessageBox.Show("Xác Nhận Lưu Thông Số Máy", "Warring", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        var data = new
                        {
                            Height_Jig_Base = tb_Base_jig_thickness.Text,
                            ofset_Machine = tb_Ofset_Shaft_Machine.Text
                        };
                        string jsonData = JsonConvert.SerializeObject(data);
                        MainWindow._queue.Add(jsonData);
                        is_Forcus = false;
                        BTN_Setting_MCancel.Visibility = Visibility.Hidden;

                        tb_Ofset_Shaft_Machine.IsReadOnly = true;
                        tb_Base_jig_thickness.IsReadOnly = true;
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
            else
            {
                MessageBox.Show("Máy đang hoạt dộng");

            }
        }

        private void Click_Para_Cancel(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Xác Nhận Hủy chỉnh sửa Thông Số Máy", "Warring", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                is_Forcus = false;
                BTN_Setting_MCancel.Visibility = Visibility.Hidden;
                tb_Ofset_Shaft_Machine.IsReadOnly = true;
                tb_Base_jig_thickness.IsReadOnly = true;
                MessageBox.Show("Đã hủy chỉnh sửa thông số cài đặt");
            }
        }
    }
}
