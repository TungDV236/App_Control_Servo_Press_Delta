using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Manual.xaml
    /// </summary>
    public partial class Manual : UserControl
    {
        Common Common = new Common();
        private DispatcherTimer timer;
        PLC plc = new PLC();
        Update_Screen ud = new Update_Screen();
        private static bool is_Forcus = false;
        public Manual()
        {
            InitializeComponent();
            Loaded += Manual_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Manual_Unloaded;
        }
        private void Manual_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click += Button_Click;
                button.PreviewMouseDown += Button_MouseDown;
                button.PreviewMouseUp += Button_MouseUp;
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
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
        private void Manual_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Update_Screen()
        {
            if (!is_Forcus)
            {
                Step_abs.Text = Data.Step_abs.ToString();
            }
            ud.bt_Green(M_Home_J_N, Data.M_Home_J_N);
            ud.bt_Green(M_Ep_J_P, Data.M_Ep_J_P);
            ud.bt_Green(M_Ep_J_N, Data.M_Ep_J_N);
            ud.bt_Green(M_Ep_ABS, Data.M_Ep_ABS_J_P);
            ud.bt_Green(On_Ep, Data.On_Ep);
            ud.bt_Green(M_Door_J_P, Data.M_Door_J_P);
            ud.bt_Green(M_Door_J_N, Data.M_Door_J_N);

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (!double.TryParse(textBox.Text, out _) & (textBox.Text != ""))
            {
                // Nếu là số, xóa thông báo lỗi
                MessageBox.Show("Vui Lòng nhập lại dữ liệu kiểu số");
                textBox.Text = "";
            }
            is_Forcus = true;
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (!double.TryParse(textBox.Text, out _) & (textBox.Text != ""))
            {
                // Nếu là số, xóa thông báo lỗi
                var data = new
                {
                    Step_abs = Step_abs.Text,
                };
                string jsonData = JsonConvert.SerializeObject(data);
                MainWindow._queue.Add(jsonData);
            }
            is_Forcus = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string buttonName = ((Button)sender).Name;
            if (buttonName != "")
            {
                if (!Is_String(buttonName, "J_P", "J_N"))
                {
                    bool newValue = !ColorChecker.IsColorEqual(((Button)sender).Background, Color.FromRgb(100, 149, 237));
                    var data = new Dictionary<string, object>
                        {
                            { buttonName, newValue }
                        };
                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);
                }

            }
        }
        private void Button_MouseDown(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if (buttonName != "")
            {
                if (Is_String(buttonName, "J_P", "J_N"))
                {
                    var data = new Dictionary<string, object>
                        {
                            { buttonName, true }

                        };

                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);
                }
            }

        }
        private void Button_MouseUp(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if (buttonName != "")
            {
                if (Is_String(buttonName, "J_P", "J_N"))
                {
                    var data = new Dictionary<string, object>
                        {
                            { buttonName, false }
                        };
                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);
                    //       MessageBox.Show("Button was Tiến X click");
                }

            }
        }
        private static bool Is_String(string input, string Compari_1, string Compari_2)
        {
            return input.Contains(Compari_1) || input.Contains(Compari_2);
        }

        private void Click_BTN_Set_SysEdit(object sender, RoutedEventArgs e)
        {

        }
    }
}
