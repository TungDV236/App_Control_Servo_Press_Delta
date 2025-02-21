using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
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
using System.Diagnostics;
using System.Windows.Threading;
using App_Control_Servo_Press_Delta.Class;
using System.ComponentModel;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        private DispatcherTimer timer;
        public static string Username = "";
        public string Password = "";
        Link_Path path = new Link_Path();
        public bool Is_Login = false;
        public event EventHandler LoginSuccessful;
        Common Common = new Common();
        private static bool keyboardIsOpen;
        public class Data
        {
            public string Name { get; set; }
        }
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
            Unloaded += LoginWindow_Unloaded;
            string json_ = File.ReadAllText(path.User_List);
            List<Data> dataList = JsonConvert.DeserializeObject<List<Data>>(json_);

            List<string> names = dataList.Select(item => item.Name).ToList();
            txtUsername.ItemsSource = names;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
            //  throw new NotImplementedException();
        }

        private void LoginWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer = null;
            }
            keyboardIsOpen = false;
            //  throw new NotImplementedException();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                CheckKeyboardStatus();
            }
            catch
            {
            }
        }
        private void Keydown_Login(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Mất focus khi nhấn Enter
                BtnLogin_Click(bt_Login,null);
            }
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Username = txtUsername.Text;
            Password = txtPassword.Password;

            try
            {
                string json = File.ReadAllText(path.User_List);
                if (json.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(json);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["Name"] == Username && (string)obj["Pass"] == Password)
                        {
                            Is_Login = true;
                            MainWindow.UserName = Username;
                            LoginSuccessful?.Invoke(this, EventArgs.Empty);
                            Close();
                            break;
                        }
                    }
                    if (!Is_Login) MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng");
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Log_err("LoginWindown", "Read path User_List", ex.ToString());
                // MessageBox.Show(e.ToString());
            }
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
        private void Text_GotFocus(object sender, RoutedEventArgs e)
        {
             Common.Open_KeyBoard();
        }

        private void exitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
        private void CheckKeyboardStatus()
        {
            var arrProcs = Process.GetProcessesByName("osk");
            var focusedElement = Keyboard.FocusedElement;
            if ((arrProcs.Length == 0) & keyboardIsOpen)
            {

                Console.WriteLine("Bàn phím ảo đang đóng.");
                Keyboard.ClearFocus();
                BtnLogin_Click(bt_Login, null);
                Global.clear_forcus = false;
                Console.WriteLine("Đã lostforcus");
                keyboardIsOpen = false;
                //  }

            }//
            else if (!(arrProcs.Length == 0) & !keyboardIsOpen)
            {
                keyboardIsOpen = true;
            }


        }  //
    }
}
