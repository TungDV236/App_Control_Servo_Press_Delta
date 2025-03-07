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
using static App_Control_Servo_Press_Delta.LoginWindow;

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
        private static bool Button_Down = false;

        Link_Path path = new Link_Path();
        private static bool flag2;
        private bool keyboardIsOpen = false;
        public Manual()
        {
            InitializeComponent();
            Loaded += Manual_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Manual_Unloaded;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
        }
        private void Manual_Loaded(object sender, RoutedEventArgs e)
        {
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
                button.MouseLeave += Button_MouseLeave;
                button.TouchDown += Button_TouchDown;
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
                button.MouseDown -= Button_MouseDown;
                button.PreviewMouseUp -= Button_MouseUp;
                button.MouseLeave -= Button_MouseLeave;
                button.TouchDown -= Button_TouchDown;
            }
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Update_Screen();
                    ud.bt_Green(M_Home_Ep_J_P, Data.M_Home_Ep_J_P);
                    ud.bt_Green(M_Ep_U_J_P, Data.M_Ep_U_J_P);
                    ud.bt_Green(M_Ep_D_J_N, Data.M_Ep_D_J_N);
                    ud.bt_Green(M_Door_U_J_P, Data.M_Door_U_J_P);
                    ud.bt_Green(M_Door_D_J_N, Data.M_Door_D_J_N);
                });

                CheckKeyboardStatus();
            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
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
            check_status_btn();

        }
        private void check_status_btn()
        {
            //  if (Data.M_Home_Ep_J_P != Global.M_Home_Ep_J_P)
            //  {
            //      var data = new
            //      {
            //          M_Home_Ep_J_P = Global.M_Home_Ep_J_P
            //      };
            //      string jsonData = JsonConvert.SerializeObject(data);
            //      MainWindow._queue.Add(jsonData);
            //  }
            //  if (Data.M_Ep_U_J_P != Global.M_Ep_J_P)
            //  {
            //      var data = new
            //      {
            //          M_Ep_U_J_P = Global.M_Ep_J_P
            //      };
            //      string jsonData = JsonConvert.SerializeObject(data);
            //      MainWindow._queue.Add(jsonData);
            //  }
            //  if (Data.M_Ep_D_J_N != Global.M_Ep_J_N)
            //  {
            //      var data = new
            //      {
            //          M_Ep_D_J_N = Global.M_Ep_J_N
            //      };
            //      string jsonData = JsonConvert.SerializeObject(data);
            //      MainWindow._queue.Add(jsonData);
            //  }
            //  if (Data.M_Door_U_J_P != Global.M_Door_J_P)
            //  {
            //      var data = new
            //      {
            //          M_Door_U_J_P = Global.M_Door_J_P
            //      };
            //      string jsonData = JsonConvert.SerializeObject(data);
            //      MainWindow._queue.Add(jsonData);
            //  }
            //  if (Data.M_Door_D_J_N != Global.M_Door_J_N)
            //  {
            //      var data = new
            //      {
            //          M_Door_D_J_N = Global.M_Door_J_N
            //      };
            //      string jsonData = JsonConvert.SerializeObject(data);
            //      MainWindow._queue.Add(jsonData);
            //  }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            string input = textBox.Text;
            // If the text is null or empty, replace it with "0"
            //   NumPad numberPad = new NumPad(textBox);
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
                textBox.SelectAll(); // Select all text to allow easy replacement
            }
            if (!double.TryParse(textBox.Text, out _) & (textBox.Text != ""))
            {
                MessageBox.Show("Vui Lòng nhập lại dữ liệu kiểu số");
                textBox.Text = "";
            }
            if (input.Contains(","))
            {
                input = input.Replace(",", ".");
                // Cập nhật lại giá trị trong TextBox
                textBox.Text = input;
                // Đặt con trỏ về cuối
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            is_Forcus = true;

            if (!flag2 & !Global.NumPad_Visiable)
            {
                NumPad numberPad = new NumPad(textBox);
                Point position = this.PointToScreen(new Point(0, 0));

                numberPad.Left = position.X + 600;
                numberPad.Top = position.Y + 360;
                numberPad.Show(); // Hiển thị cửa sổ như hộp thoại
                                  // MessageBox.Show("TextBox_GotFocus2");
                flag2 = true;
                // Common.Open_KeyBoard(); 
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Mất focus khi nhấn Enter
                Keyboard.ClearFocus();
                FocusBorder.Focusable = true;
                FocusBorder.Focus();
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
                if (double.TryParse(textBox.Text, out double doubleValue) & textBox.Name != "Jog_Max_Force")
                {
                    var data = new Dictionary<string, object>
                {
                        { textboxName, doubleValue }
                    };

                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);

                    //  Common.Log_Operation("Write " + textboxName + ":   " + doubleValue, path.Log_EN);
                    //  Common.Log_Operation("Nhập thông số:  " + textboxName + ":   " + doubleValue, path.Log_VN);
                }
                if (double.TryParse(textBox.Text, out double doubleValue1) & textBox.Name == "Jog_Max_Force")
                {
                    if (doubleValue1 <= 3300)
                    {
                        var data = new Dictionary<string, object>
                        {
                        { textboxName, doubleValue }
                         };
                        string jsonData = JsonConvert.SerializeObject(data);
                        MainWindow._queue.Add(jsonData);
                    }
                    else
                    {
                        var data = new Dictionary<string, object>
                        {
                        { textboxName, 3300}
                         };
                        string jsonData = JsonConvert.SerializeObject(data);
                        MainWindow._queue.Add(jsonData);
                    }


                }



                Common.Log_Operation("Write " + textboxName + ":   " + doubleValue, path.Log_EN);
                Common.Log_Operation("Nhập thông số : " + textboxName + ":   " + doubleValue, path.Log_VN);
            }
            is_Forcus = false;
            flag2 = false;
            Keyboard.ClearFocus();
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string buttonName = ((Button)sender).Name;
            string Taglog;
            if (buttonName != "")
            {
                Taglog = buttonName;
                if (Is_String(buttonName, "J_P", "J_N"))
                {
                    Taglog = buttonName.Substring(0, buttonName.Length - 4);
                }
                Common.Log_Operation("Press Button:   " + Taglog, path.Log_EN);
                Common.Log_Operation("Nhấn nút:   " + Taglog, path.Log_VN);

            }
            Console.WriteLine(buttonName + "Click");
        }
        private void Button_TouchDown(object sender, TouchEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
           if (buttonName != "" & !Button_Down)
           {
               if (Is_String(buttonName, "J_P", "J_N"))
               {
                   var data = new Dictionary<string, object>
                       {
                           { buttonName, true }
          
                       };
          
                   string jsonData = JsonConvert.SerializeObject(data);
                   MainWindow._queue.Add(jsonData);
                   if (buttonName == "M_Home_Ep_J_P")
                   {
                       Global.M_Home_Ep_J_P = true;
                   }
                   if (buttonName == "M_Ep_U_J_P")
                   {
                       Global.M_Ep_J_P = true;
                   }
                   if (buttonName == "M_Ep_D_J_N")
                   {
                       Global.M_Ep_J_N = true;
                   }
                   if (buttonName == "M_Door_U_J_P")
                   {
                       Global.M_Door_J_P = true;
                   }
                   if (buttonName == "M_Door_D_J_N")
                   {
                       Global.M_Door_J_N = true;
                   }
          
               }
               Button_Down = true;
           }
        }
        private void Button_MouseDown(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if (buttonName != "" & !Button_Down)
            {
                if (Is_String(buttonName, "J_P", "J_N"))
                {
                    var data = new Dictionary<string, object>
                        {
                            { buttonName, true }
               
                        };
               
                    string jsonData = JsonConvert.SerializeObject(data);
                    MainWindow._queue.Add(jsonData);
                    if (buttonName == "M_Home_Ep_J_P")
                    {
                        Global.M_Home_Ep_J_P = true;
                    }
                    if (buttonName == "M_Ep_U_J_P")
                    {
                        Global.M_Ep_J_P = true;
                    }
                    if (buttonName == "M_Ep_D_J_N")
                    {
                        Global.M_Ep_J_N = true;
                    }
                    if (buttonName == "M_Door_U_J_P")
                    {
                        Global.M_Door_J_P = true;
                    }
                    if (buttonName == "M_Door_D_J_N")
                    {
                        Global.M_Door_J_N = true;
                    }
                }
                Button_Down = true;
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
                    if (buttonName == "M_Home_Ep_J_P")
                    {
                        Global.M_Home_Ep_J_P = false;
                    }
                    if (buttonName == "M_Ep_U_J_P")
                    {
                        Global.M_Ep_J_P = false;
                    }
                    if (buttonName == "M_Ep_D_J_N")
                    {
                        Global.M_Ep_J_N = false;
                    }
                    if (buttonName == "M_Door_U_J_P")
                    {
                        Global.M_Door_J_P = false;
                    }
                    if (buttonName == "M_Door_D_J_N")
                    {
                        Global.M_Door_J_N = false;
                    }
                }
               
                Button_Down = false;
            }
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if (buttonName != "")
            {
                if (Is_String(buttonName, "J_P", "J_N"))
                {
                    //  var data = new Dictionary<string, object>
                    //      {
                    //          { buttonName, false }
                    //      };
                    //  string jsonData = JsonConvert.SerializeObject(data);
                    //  // MainWindow._queue.Add(jsonData);
                    //  MainWindow._queue.Add(jsonData);
                    //  if (buttonName == "M_Home_Ep_J_P")
                    //  {
                    //      Global.M_Home_Ep_J_P = false;
                    //  }
                    //  if (buttonName == "M_Ep_U_J_P")
                    //  {
                    //      Global.M_Ep_J_P = false;
                    //  }
                    //  if (buttonName == "M_Ep_D_J_N")
                    //  {
                    //      Global.M_Ep_J_N = false;
                    //  }
                    //  if (buttonName == "M_Door_U_J_P")
                    //  {
                    //      Global.M_Door_J_P = false;
                    //  }
                    //  if (buttonName == "M_Door_D_J_N")
                    //  {
                    //      Global.M_Door_J_N = false;
                    //  }
                }

                Button_Down = false;
            }
        }
        private void CheckKeyboardStatus()
        {
            var arrProcs = Process.GetProcessesByName("osk");
            var focusedElement = Keyboard.FocusedElement;


            if ((arrProcs.Length == 0) & keyboardIsOpen)
            {

                Console.WriteLine("Bàn phím ảo đang đóng.");
                if (focusedElement is TextBox textBox)
                {

                    Keyboard.ClearFocus();
                    //  TextBox_LostFocus(textBox, null);
                    Global.clear_forcus = false;
                    Console.WriteLine("Đã lostforcus");
                    keyboardIsOpen = false;
                    FocusBorder.Focusable = true;
                    FocusBorder.Focus();
                    //  }

                }//
            }
            else if (!(arrProcs.Length == 0) & !keyboardIsOpen)
            {
                keyboardIsOpen = true;
            }
            if (Global.clear_forcus)
            {
                if (focusedElement is TextBox textBox)
                {
                    //  textBox.Text = Global.Textbox_string ;
                    //  TextBox_LostFocus(textBox, null);
                    Keyboard.ClearFocus();
                    Global.clear_forcus = false;
                    Console.WriteLine("Đã lostforcus");
                    Global.Textbox_string = "";
                    keyboardIsOpen = false;
                    FocusBorder.Focusable = true;
                    FocusBorder.Focus();
                    //  }

                }//
            }

        }  //
        private static bool Is_String(string input, string Compari_1, string Compari_2)
        {
            return input.Contains(Compari_1) || input.Contains(Compari_2);
        }

        private void Click_BTN_Set_SysEdit(object sender, RoutedEventArgs e)
        {

        }

        private void M_Ep_ABS_Click(object sender, RoutedEventArgs e)
        {
            bool newValue = !Data.M_Ep_ABS;
            var data = new Dictionary<string, object>
                        {
                            { "M_Ep_ABS" , newValue }
                        };
            string jsonData = JsonConvert.SerializeObject(data);
            MainWindow._queue.Add(jsonData);
        }


    }
}
