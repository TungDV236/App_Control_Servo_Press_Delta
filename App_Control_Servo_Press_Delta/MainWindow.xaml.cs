using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using OxyPlot;
using App_Control_Servo_Press_Delta.Class;



namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region khai báo màn hình
        Auto Auto_Screen = new Auto();
        Manual Manual_Screen = new Manual();
        History History_Screen = new History();
        GPIO GPIO_Screen = new GPIO();
        Model Model_Screen = new Model();
        Setting Setting_Screen = new Setting();
        History_Error History_Error = new History_Error();
        History_Alarm History_al = new History_Alarm();
        Report Report_Screen = new Report();
        //

        #endregion
        #region khai báo Class
        History_UL History_UL = new History_UL();//-----
        Update_Screen ud = new Update_Screen();
        PLC PLC = new PLC();
        Link_Path path = new Link_Path();
        Common Common = new Common();
        #endregion
        #region khai báo dữ liệu
        List<List_History> List_History = new List<List_History>();
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        DataPoint newPoint1 = new DataPoint();
        #endregion
        #region Khai báo biến public
        public static string UserName = "";
        public static ObservableCollection<string> _queue;
        public double newY1;
        public ObservableCollection<string> Notifications { get; set; }
        #endregion
        #region khai bao biến private
        private static string[] Time_start1;
        private static string[] Time_start2;
        private static string[] Time_start3;
        private static string[] Time_stop1;
        private static string[] Time_stop2;
        private static string[] Time_stop3;
        private static uint Value_Al_old;
        private static uint Value_Err_old;
        private static string Status_PLC;
        private int position = 0;
        private bool Flag;
        private bool Flag1;
        private double _Force_max;
        private DispatcherTimer Update_Status;
        private DispatcherTimer Update_Sys;
        private int pointCount = 0;
        #endregion



        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            Notifications = new ObservableCollection<string>();
            DataContext = this;
            Global.DataPoints1 = new List<DataPoint>();

            var workingArea = SystemParameters.WorkArea;
            _queue = new ObservableCollection<string>();
            _queue.CollectionChanged += Queue_CollectionChanged;
            // Đặt kích thước và vị trí của cửa sổ
           // this.Left = workingArea.Left-5;
           // this.Top = workingArea.Top;
           // this.Width = workingArea.Width + 10;
           // this.Height = workingArea.Height + 5;
            // Program.Main();
        }
        private void Queue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_queue.Count > 0)
            {
                // Gửi HTTP POST request khi số lượng phần tử thay đổi
                PLC.SendPostRequestAsync();
            }
        }
        public static string GetMacAddress()
        {
            string macAddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up && !nic.Description.ToLower().Contains("virtual") && !nic.Description.ToLower().Contains("pseudo"))
                {
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) // Check if it's a Wi-Fi interface
                    {
                        byte[] macBytes = nic.GetPhysicalAddress().GetAddressBytes();
                        macAddress = string.Join(":", macBytes.Select(b => b.ToString("X2")));
                        break;
                    }

                }
            }
            return macAddress;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Maximized;
            //
            PLC.StartTimer();
            Update_Status = new DispatcherTimer();
            Update_Status.Interval = TimeSpan.FromMilliseconds(100);
            Update_Status.Tick += Update_Status_Tick100ms;
            Update_Status.Start();
            //
            Update_Sys = new DispatcherTimer();
            Update_Sys.Interval = TimeSpan.FromMilliseconds(1000);
            Update_Sys.Tick += Update_Status_Tick1000ms;
            Update_Sys.Start();
            //
            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(Auto_Screen);
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.PreviewMouseDown += Button_MouseDown;
                button.PreviewMouseUp += Button_MouseUp;
            }

            LanguageComboBox.SelectedIndex = 1;
            //
            //
            // LanguageComboBox.SelectedIndex = 1;
            //
            //Reset Jig

        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PLC.StopTimer();
            Update_Status.Stop();
            Update_Sys.Stop();
        }

        private void Update_Status_Tick100ms(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // ud.bt_Blue(bt_Origin, Data.Flag_Org, false);
                // ud.bt_Blue(bt_Reset, Data.Reset, false);

               // Position.Text = Math.Round(Data.Position, 2).ToString("F1");
               // Momen_PV.Text = Math.Round(Data.Momen_PV, 2).ToString("F1");
            });
            //   Scan_Err();
            if (Global.Start)
            {
                Update_Datachart();
            }
            else if (!Global.Start & Flag)
            {
                Flag = false;
                Flag1 = false;
            }
            animation(TB_Notification);

        }
        private void Update_Status_Tick1000ms(object sender, EventArgs e)
        {

            try
            {
                Scan();
                Dispatcher.Invoke(() =>
                {
                    Update_Screen();
                });
            }
            catch
            {

            }
        }
        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string languageTag = selectedItem.Content.ToString();
                ChangeAppLanguage(languageTag);
            }
        }
        private void ChangeAppLanguage(string languageTag)
        {

            var EnglishResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault(rd => rd.Source != null && rd.Source.ToString().Contains("Resources/en.xaml"));
            var vietnameseResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault(rd => rd.Source != null && rd.Source.ToString().Contains("Resources/vi.xaml"));

            // Thêm resource tiếng Anh


            if (languageTag == "English")
            {
                if (vietnameseResource != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(vietnameseResource);
                }
                var englishResource = new ResourceDictionary
                {
                    Source = new Uri("Resources/en.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(englishResource);
            }
            else if (languageTag == "Vietnamese")
            {
                if (EnglishResource != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(EnglishResource);
                }
                var VNResource = new ResourceDictionary
                {
                    Source = new Uri("Resources/vi.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(VNResource);
            }
        }
        private void Update_Screen()
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yyyy");
            lb_day.Content = formattedDate;
            string formattedtime = dateTime.ToString("HH:mm:ss");
            lb_time.Content = formattedtime;
            if (PLC.IsConnected)
            {
                lb_Connect.Foreground = System.Windows.Media.Brushes.Green;
                Status_PLC = "";
            }
            else
            {

                MainWindow._queue.Clear();
                lb_Connect.Foreground = System.Windows.Media.Brushes.Red;
                Status_PLC = formattedDate + " - " + formattedtime + " - " + "Mất kết Nối PLC";
            }
            //
            float cpuUsage = cpuCounter.NextValue();
            string formattedCpuUsage = cpuUsage.ToString("F2") + "%";

        }
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;


        }
        private void Button_MouseDown(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if (buttonName != "")
            {
                if (buttonName == "Off_Buzzer")
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
                if (buttonName == "Off_Buzzer")
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
        private void Click_BTN_Auto(object sender, RoutedEventArgs e)
        {


            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(Auto_Screen);
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void Click_BTN_Manual(object sender, RoutedEventArgs e)
        {
            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(Manual_Screen);
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        private void Click_BTN_History(object sender, RoutedEventArgs e)
        {
            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(History_Screen);
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        private void Click_BTN_Report(object sender, RoutedEventArgs e)
        {
            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(Report_Screen);
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        private void Click_BTN_GPIO1(object sender, RoutedEventArgs e)
        {
            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(GPIO_Screen);
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }


        private void Click_BTN_Model(object sender, RoutedEventArgs e)
        {
            //if (UserName != "")
            //{
            Pannel_Monitor.Children.Clear();
            Pannel_Monitor.Children.Add(Model_Screen);
            BTN_Model.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            //}
            //else
            //{
            //MessageBox.Show("Vui Lòng Đăng Nhập");
            //}
        }


        private void Click_BTN_Setting(object sender, RoutedEventArgs e)
        {
            if (UserName != "")
            {
                Pannel_Monitor.Children.Clear();
                Pannel_Monitor.Children.Add(Setting_Screen);
                BTN_Setting.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                BTN_Auto.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                BTN_Report.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                BTN_History.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                BTN_GPIO.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                BTN_Model.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                BTN_Manual.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }
        }
        private DispatcherTimer logoutTimer;
        private DateTime lastActivityTime;
        private void LoginWindow_LoginSuccessful(object sender, EventArgs e)
        {
            lb_Name.Content = UserName;
            logoutTimer = new DispatcherTimer();
            logoutTimer.Interval = TimeSpan.FromMinutes(10);
            logoutTimer.Tick += LogoutTimer_Tick;
            Login();
        }
        private void LogoutTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan idleTime = DateTime.Now - lastActivityTime;
            if (idleTime >= TimeSpan.FromMinutes(10))
            {
                //Logout();
            }
        }
        private void Login()
        {
            lastActivityTime = DateTime.Now;
            logoutTimer.Start();
        }
        private void Logout()
        {
            UserName = "";
            lb_Name.Content = UserName;
        }
        private void bt_Login_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.LoginSuccessful += LoginWindow_LoginSuccessful;
            loginWindow.ShowDialog();
        }

        private void bt_Logout_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }


        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MouseDown_Close(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn đóng cửa sổ giám sát không? ", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }

        }
        private void MouseDown_infor(object sender, RoutedEventArgs e)
        {
            Infor InforWindow = new Infor();
            InforWindow.Owner = this;
            InforWindow.Show();
        }
        public void Scan()
        {
            uint Value_al = (uint)((Data.Alarm2 << 16) | Data.Alarm1);
            uint Value_Err = (uint)((Data.Error2 << 16) | Data.Error1);
            string code_A = "";
            string code_E = "";
            // Phát hiện sự thay đổi của bit
            uint changed_A_Bits = Value_Al_old ^ Value_al;
            uint changed_E_Bits = Value_Err_old ^ Value_Err;
            //  Console.WriteLine("Changed bits:");

            if (Value_Err != Value_Err_old)
            {
                for (int i = 0; i < 32; i++)
                {
                    if ((changed_E_Bits & (1U << i)) != 0)
                    {
                        if ((Value_Err & (1U << i)) != 0)
                        {
                            code_E = Choose_Data_Err(i);
                            //   Console.WriteLine(" ma loi: " + code_E + "da xu ly");
                            //   Clear_History(code_E);
                            Add_Err(code_E);

                        }
                        else
                        {
                            code_E = Choose_Data_Err(i);
                            //  Console.WriteLine(" ma loi: " + code_E + "ton tai");
                            // Save_History(code_E);
                            Clear_His(code_E);
                        }
                    }
                }
                Value_Err_old = Value_Err;
                List<List_History> List_History_Copy = new List<List_History>(List_History);
                for (int i = 0; i < List_History_Copy.Count; i++)
                {
                    List_History_Copy[i].STT = i + 1;
                }
                List_History = List_History_Copy;
            }

            if (Value_al != Value_Al_old)
            {
                for (int i = 0; i < 32; i++)
                {
                    if ((changed_A_Bits & (1U << i)) != 0)
                    {
                        if ((Value_al & (1U << i)) != 0)
                        {
                            code_E = Choose_Data_Al(i);
                            //   Console.WriteLine(" ma loi: " + code_E + "da xu ly");
                            //   Clear_History(code_E);
                            Add_Al(code_E);

                        }
                        else
                        {
                            code_E = Choose_Data_Al(i);
                            //  Console.WriteLine(" ma loi: " + code_E + "ton tai");
                            // Save_History(code_E);
                            Clear_His(code_E);
                        }
                    }
                }
                Value_Al_old = Value_al;
                List<List_History> List_History_Copy = new List<List_History>(List_History);
                for (int i = 0; i < List_History_Copy.Count; i++)
                {
                    List_History_Copy[i].STT = i + 1;
                }
                List_History = List_History_Copy;
            }

        }

        public static string Choose_Data_Err(int i)
        {
            switch (i)
            {

                case 0:
                    return "E00";
                case 1:
                    return "E01";
                case 2:
                    return "E02";
                case 3:
                    return "E03";
                case 4:
                    return "E04";
                case 5:
                    return "E05";
                case 6:
                    return "E06";
                case 7:
                    return "E07";
                case 8:
                    return "E08";
                case 9:
                    return "E09";
                case 10:
                    return "E0A";
                case 11:
                    return "E0B";
                case 12:
                    return "E0C";
                case 13:
                    return "E0D";
                case 14:
                    return "E0E";
                case 15:
                    return "E0F";
                case 16:
                    return "E10";
                case 17:
                    return "E11";
                case 18:
                    return "E12";
                case 19:
                    return "E13";
                case 20:
                    return "E14";
                case 21:
                    return "E15";
                case 22:
                    return "E16";
                case 23:
                    return "E17";
                case 24:
                    return "E18";
                case 25:
                    return "E19";
                case 26:
                    return "E1A";
                case 27:
                    return "E1B";
                case 28:
                    return "E1C";
                case 29:
                    return "E1D";
                case 30:
                    return "E1E";
                case 31:
                    return "E1F";
                case 32:
                    return "A00";
                case 33:
                    return "A01";
                case 34:
                    return "A02";
                case 35:
                    return "A03";
                case 36:
                    return "A04";
                case 37:
                    return "A05";
                case 38:
                    return "A06";
                case 39:
                    return "A07";
                case 40:
                    return "A08";
                case 41:
                    return "A09";
                case 42:
                    return "A0A";
                case 43:
                    return "A0B";
                case 44:
                    return "A0C";
                case 45:
                    return "A0D";
                case 46:
                    return "A0E";
                case 47:
                    return "A0F";
                case 48:
                    return "A10";
                case 49:
                    return "A11";
                case 50:
                    return "A12";
                case 51:
                    return "A13";
                case 52:
                    return "A14";
                case 53:
                    return "A15";
                case 54:
                    return "A16";
                case 55:
                    return "A17";
                case 56:
                    return "A18";
                case 57:
                    return "A19";
                case 58:
                    return "A1A";
                case 59:
                    return "A1B";
                case 60:
                    return "A1C";
                case 61:
                    return "A1D";
                case 62:
                    return "A1E";
                case 63:
                    return "A1F";

                default:
                    return "Invalid option";
            }
        }
        public static string Choose_Data_Al(int i)
        {
            switch (i)
            {

                case 0:
                    return "A00";
                case 1:
                    return "A01";
                case 2:
                    return "A02";
                case 3:
                    return "A03";
                case 4:
                    return "A04";
                case 5:
                    return "A05";
                case 6:
                    return "A06";
                case 7:
                    return "A07";
                case 8:
                    return "A08";
                case 9:
                    return "A09";
                case 10:
                    return "A0A";
                case 11:
                    return "A0B";
                case 12:
                    return "A0C";
                case 13:
                    return "A0D";
                case 14:
                    return "A0E";
                case 15:
                    return "A0F";
                case 16:
                    return "A10";
                case 17:
                    return "A11";
                case 18:
                    return "A12";
                case 19:
                    return "A13";
                case 20:
                    return "A14";
                case 21:
                    return "A15";
                case 22:
                    return "A16";
                case 23:
                    return "A17";
                case 24:
                    return "A18";
                case 25:
                    return "A19";
                case 26:
                    return "A1A";
                case 27:
                    return "A1B";
                case 28:
                    return "A1C";
                case 29:
                    return "A1D";
                case 30:
                    return "A1E";
                case 31:
                    return "A1F";
                default:
                    return "Invalid option";
            }
        }
        private void Add_Err(string code_E)
        {

            List_History List_History_ = new List_History();
            System.DateTime dateTime = System.DateTime.Now;
            Link_Path linkpath = new Link_Path();
            string Fill_json = File.ReadAllText(linkpath.History);
            //   string json_ = File.ReadAllText(linkpath.Error);
            string json = File.ReadAllText(linkpath.Error);
            int cnt = 0;
            //try
            //{
            if (Fill_json.Length > 0)
            {
                JArray json_fillArray = JArray.Parse(Fill_json);
                foreach (JObject obj in json_fillArray)
                {
                    if ((string)obj["Code"] == code_E)
                    {
                        foreach (var data in List_History)
                        {
                            if (data.Code == code_E)
                            {
                                cnt = 1;
                                break;
                            }
                        }
                        if (cnt == 0)
                        {
                            List_History_.STT = 1;
                            List_History_.Code = (string)obj["Code"];
                            List_History_.Content_ = (string)obj["Content_"];
                            List_History_.Solution = (string)obj["Solution"];
                            List_History_.Time = dateTime.ToString();
                            string list_Error_Json = JsonConvert.SerializeObject(List_History_);
                            List_History.Add(List_History_);
                            if (json.Length < 50)
                            {
                                json = json.Remove(json.Length - 1);
                                json = json + list_Error_Json + "]";
                                File.WriteAllText(linkpath.Error, json);
                            }
                            else
                            {
                                json = json.Remove(json.Length - 1);
                                json = json + ",\r" + list_Error_Json + "]";
                                File.WriteAllText(linkpath.Error, json);
                                //   List_Alarm.ItemsSource = List_History;
                            }
                        }
                    }
                }
            }
        }
        private void Add_Al(string code_E)
        {

            List_History List_History_ = new List_History();
            System.DateTime dateTime = System.DateTime.Now;
            Link_Path linkpath = new Link_Path();
            string Fill_json = File.ReadAllText(linkpath.History);
            //   string json_ = File.ReadAllText(linkpath.Error);
            string json = File.ReadAllText(linkpath.Alarm);
            int cnt = 0;
            if (Fill_json.Length > 0)
            {
                JArray json_fillArray = JArray.Parse(Fill_json);
                foreach (JObject obj in json_fillArray)
                {
                    if ((string)obj["Code"] == code_E)
                    {
                        foreach (var data in List_History)
                        {
                            if (data.Code == code_E)
                            {
                                cnt = 1;
                                break;
                            }
                        }
                        if (cnt == 0)
                        {
                            List_History_.STT = 1;
                            List_History_.Code = (string)obj["Code"];
                            List_History_.Content_ = (string)obj["Content_"];
                            List_History_.Solution = (string)obj["Solution"];
                            List_History_.Time = dateTime.ToString();
                            string list_Alarm_Json = JsonConvert.SerializeObject(List_History_);
                            List_History.Add(List_History_);
                            if (json.Length < 50)
                            {
                                json = json.Remove(json.Length - 1);
                                json = json + list_Alarm_Json + "]";
                                File.WriteAllText(linkpath.Alarm, json);
                            }
                            else
                            {
                                json = json.Remove(json.Length - 1);
                                json = json + ",\r" + list_Alarm_Json + "]";
                                File.WriteAllText(linkpath.Alarm, json);
                                //   List_Alarm.ItemsSource = List_History;
                            }
                        }
                    }
                }
            }
        }
        private void Clear_His(string code_E)
        {
            var newData = new List<List_History>();

            foreach (var item in List_History)
            {
                if (item.Code != code_E)
                {

                    newData.Add(item);
                }
            }
            List_History = newData;
            //  History_Error.List_Error.ItemsSource = List_History;
            Console.WriteLine(" ma loi: " + code_E + "da xu ly");
            //    Common.Load_View_Model(List_History_);
        }
        private void animation(TextBox sender)
        {
            TextBox textBox = (TextBox)sender;

            string mylistString = Status_PLC + string.Join("           ", List_History.Select(o => $"{o.STT}" + " - " + $"{o.Code}" + " - " + $" {o.Content_}")) + "                                               ";

            string _mylistString = mylistString + mylistString + mylistString;

            // Di chuyển dòng chữ sang trái
            position++;
            if (position >= _mylistString.Length)
            {
                position = 0;
            }

            // Cập nhật nội dung của TextBox
            textBox.Text = _mylistString.Substring(position) + _mylistString.Substring(0, position);
        }
        private void Fill_para(string name)
        {
            string json = File.ReadAllText(path.Time_work);
            // string json = File.ReadAllText(linkpath.Model);
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["Name"] == "Ca 1")
                    {
                        Time_start1 = ((string)obj["Time_Start"]).Split(new char[] { ':' });
                        Time_stop1 = ((string)obj["Time_Stop"]).Split(new char[] { ':' });
                    }
                    else if ((string)obj["Name"] == "Ca 2")
                    {
                        Time_start2 = ((string)obj["Time_Start"]).Split(new char[] { ':' });
                        Time_stop2 = ((string)obj["Time_Stop"]).Split(new char[] { ':' });
                    }
                    else if ((string)obj["Name"] == "Ca 3")
                    {
                        Time_start3 = ((string)obj["Time_Start"]).Split(new char[] { ':' });
                        Time_stop3 = ((string)obj["Time_Stop"]).Split(new char[] { ':' });
                    }
                }
            }
        }
        private void exitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BTN_Exit.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BTN_Exit.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
        private void Infor_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.White; // Thay đổi màu nền khi di chuột qua
        }

        private void Infor_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
        public void Update_Datachart()
        {
            pointCount++;
            newY1 = Math.Sin(pointCount * 100);
            newPoint1 = new DataPoint(pointCount, newY1);
            Global.DataPoints1.Add(newPoint1);
            if (_Force_max < Data.Momen_PV)
            {
                _Force_max = Data.Momen_PV;
            }
            if (Global.DataPoints1 != null)
            {

                if (Global.Start & !Flag)
                {
                    if (AreTextBoxesFilled())
                    {
                        Data_Report_temp2.Force_Max = _Force_max.ToString();

                        if (Global.DataPoints1.Count > 2 & !Flag1)
                        {
                            Flag1 = true;
                            Save_Model();
                            Global.DataPoints1.Clear();

                        }
                        Flag = true;
                        pointCount = 0;
                    }
                    else
                    {
                        Global.DataPoints1.Clear();
                    }
                }




            }
        }

        private void Off_Buzzer_Click(object sender, RoutedEventArgs e)
        {
            Global.Start = !Global.Start;
        }
        private void Save_Model()
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yyyy");
            string FilePath = System.IO.Path.Combine("Log", formattedDate.Replace('/', '_') + "_Report.json");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            string ID = formattedDate.Replace("/", "") + formattedtime.Replace(":", "");
            Data_Report List_Report = new Data_Report();
            List_Report.Time = formattedtime;
            List_Report.OrderCode = Data_Report_temp2.OrderCode;
            List_Report.Model = Data_Report_temp2.Model;
            List_Report.TrucID = Data_Report_temp2.TrucID;
            List_Report.RotorID = Data_Report_temp2.RotorID;
            List_Report.Beer_Up = Data_Report_temp2.Beer_Up;
            List_Report.Beer_Down = Data_Report_temp2.Beer_Down;
            List_Report.Jig_Up = Data_Report_temp2.Jig_Up;
            List_Report.Jig_Mid = Data_Report_temp2.Jig_Mid;
            List_Report.Jig_Down = Data_Report_temp2.Jig_Down;
            List_Report.HStand = Data_Report_temp2.HStand;
            List_Report.Force = Data_Report_temp2.Force;
            List_Report.Force_Max = Data_Report_temp2.Force_Max;
            List_Report.Position = List_to_String(Global.DataPoints1);
            string list_Json = JsonConvert.SerializeObject(List_Report);
            try
            {
                string json = File.ReadAllText(FilePath);
                json = json.Remove(json.Length - 1);
                json = json + ",\n" + list_Json + "]";
                File.WriteAllText(FilePath, json);
                // MessageBox.Show("Đã Lưu  Thành Công");
            }
            catch
            {
                string json_;
                json_ = "[\n" + list_Json + "\n]";
                File.WriteAllText(FilePath, json_);
                //  MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }

        }
        private static string List_to_String(List<DataPoint> dataPoint)
        {
            string result = (string.Join(",", dataPoint.Select(o => $"{(o.X.ToString().Trim('.', ' ')).Replace(',', '.')}." + "_" + $"{(o.Y.ToString().Trim('.', ' ')).Replace(',', '.')}"))).Replace("._", "_");
            return result;
        }
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return !string.IsNullOrWhiteSpace(Data_Report_temp2.Model);
        }
    }
}
