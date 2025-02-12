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
using App_Control_Servo_Press_Delta.Popup;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        Common Common = new Common();
        Link_Path path = new Link_Path();
        private DispatcherTimer timer;
        Update_Screen ud = new Update_Screen();
        private static bool is_Forcus = false;
        private static bool is_Forcus2 = false;
        PLC plc = new PLC();
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
                TB_Force_Max.Text = Data.Momen_max.ToString();       //------------------------------------
                TB_Vman.Text = Data.V_man.ToString();            //------------------------------------
                TB_VAuto.Text = Data.V_Auto.ToString();
                TB_Limit_Up.Text = Data.Limit_U.ToString();
                TB_Limit_Down.Text = Data.Limit_D.ToString(); //------------------------------------

            }
            if (!is_Forcus2)
            {
                string json = File.ReadAllText(path.Setting);
                var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                string[] parts = SplitString(data_Setting["Server"], ':');
                TB_Server_IP.Text = parts[0];
                TB_Server_Port.Text = parts[1];
                string[] parts2 = SplitString(data_Setting["PLC_IP"], ':');
                TB_PLC_IP.Text = parts2[0];
                TB_PLC_Port.Text = parts2[1];
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
                MessageBox.Show("Vui Lòng nhập lại dữ liệu kiểu số");
                textBox.Text = "";
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Setting_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model_Beer_Up Model_Beer_Up = new Model_Beer_Up();
            Model_Beer_Down Model_Beer_Down = new Model_Beer_Down();
            Model_Jig_Up Model_Jig_Up = new Model_Jig_Up();
            Model_Jig_Mid Model_Jig_Mid = new Model_Jig_Mid();
            Model_Jig_Down Model_Jig_Down = new Model_Jig_Down();
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
            if (MainWindow.UserName == "STI-Technical")
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
                MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical ");
            }
        }

        private void Click_BTN_Set_SysSave(object sender, RoutedEventArgs e)
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
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return !string.IsNullOrWhiteSpace(TB_Force_Max.Text) &&
                   !string.IsNullOrWhiteSpace(TB_Vman.Text) &&
                   !string.IsNullOrWhiteSpace(TB_VAuto.Text);
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
                        Momen_max = TB_Force_Max.Text,
                        Limit_D = TB_Limit_Down.Text,
                        Limit_U = TB_Limit_Up.Text,
                        V_Auto = TB_VAuto.Text,
                        V_man = TB_Vman.Text
                        // Limit_Y1 = TB_LimitYUp.Text
                    };
                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);
                    TB_Force_Max.IsReadOnly = true;
                    TB_Vman.IsReadOnly = true;
                    TB_VAuto.IsReadOnly = true;
                    TB_Limit_Up.IsReadOnly = true;
                    TB_Limit_Down.IsReadOnly = true;

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
                TB_Force_Max.IsReadOnly = false;
                TB_Limit_Up.IsReadOnly = false;
                TB_Limit_Down.IsReadOnly = false;
                TB_VAuto.IsReadOnly = false;
                TB_Vman.IsReadOnly = false;

            }
            if (MainWindow.UserName != "")
            {
                TB_Force_Max.IsReadOnly = false;
                TB_Limit_Up.IsReadOnly = false;
                TB_Limit_Down.IsReadOnly = false;
                TB_VAuto.IsReadOnly = false;
                TB_Vman.IsReadOnly = false;
            }
            MessageBox.Show("Có thể cài đặt thông số");
            is_Forcus = true;
        }
        public static string[] SplitString(string input, char delimiter)
        {
            return input.Split(delimiter);
        }
        private static bool Is_String(string input, string Compari_1)
        {
            return input.Contains(Compari_1);
        }
    }
}
