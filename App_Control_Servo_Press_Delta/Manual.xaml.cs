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
using System.Windows.Input;
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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
                textBox.KeyDown += TextBox_KeyDown;
            }
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click += Button_Click;
                button.PreviewMouseDown += Button_MouseDown;
                button.PreviewMouseUp += Button_MouseUp;
            }

            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Manual_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.KeyDown -= TextBox_KeyDown;
            }
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click -= Button_Click;
                button.PreviewMouseDown -= Button_MouseDown;
                button.PreviewMouseUp -= Button_MouseUp;
            }
            timer.Tick += Timer_Tick;
            timer.Start();
            timer = null;
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
                Jog_Max_Force.Text = Math.Round(Data.Jog_Max_Force, 2).ToString();
                Jog_Distance_ABS.Text = Math.Round(Data.Jog_Distance_ABS, 2).ToString();
                Jog_Vel.Text = Math.Round(Data.Jog_Vel, 2).ToString();
                Go_Home_Vel.Text = Math.Round(Data.Go_Home_Vel, 2).ToString();
                // Console.WriteLine("Đã cập nhật");
            }
            if (Data.M_Ep_ABS)
            {
                if (Global.Language == "EN")
                {
                    M_Ep_ABS.Content = "Inch";
                }
                if (Global.Language == "VN")
                {
                    M_Ep_ABS.Content = "Tuyệt đối";
                }
            }    
            else
            {
                if (Global.Language == "EN")
                {
                    M_Ep_ABS.Content = "Jog";
                }
                if (Global.Language == "VN")
                {
                    M_Ep_ABS.Content = "Tương đối";
                }
            }    

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            is_Forcus = true;

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

        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Mất focus khi nhấn Enter
                Keyboard.ClearFocus();
            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            //  FocusBorder.Focusable = true; // Đảm bảo Border có thể nhận focus
            // FocusBorder.Focus();
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
            else
            {
                try
                { 
                    if (textboxName == "Go_Home_Vel")
                    {
                        Common.Log_data("Man", textboxName, Data.Go_Home_Vel.ToString(), textBox.Text);
                    }
                    if (textboxName == "Jog_Vel")
                    {
                        Common.Log_data("Man", textboxName, Data.Jog_Vel.ToString(), textBox.Text);
                    }
                    if (textboxName == "Jog_Distance_ABS")
                    {
                        Common.Log_data("Man", textboxName, Data.Jog_Distance_ABS.ToString(), textBox.Text);
                    }
                    if (textboxName == "Jog_Max_Force")
                    {
                        Common.Log_data("Man", textboxName, Data.Jog_Max_Force.ToString(), textBox.Text);
                    }
                }
                catch { }
                if (double.TryParse(textBox.Text, out double doubleValue))
                {
                    var data = new Dictionary<string, object>
                {
                        { textboxName, doubleValue }
                    };

                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);
                }
            }
            is_Forcus = false;
            Keyboard.ClearFocus();
            FocusBorder.Focusable = true;
            FocusBorder.Focus();
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
