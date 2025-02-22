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
using System.Net.Sockets;



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
        GPIO GPIO_Screen = new GPIO();
        Model Model_Screen = new Model();
        Setting Setting_Screen = new Setting();
        History_Error History_Error = new History_Error();
        Report Report_Screen = new Report();
        //

        #endregion
        #region khai báo Class
        History_UL History_UL = new History_UL();//-----
        Update_Screen ud = new Update_Screen();
        PLC PLC = new PLC();
        Link_Path path = new Link_Path();
        Common Common = new Common();
        Socket_client socket = new Socket_client();

        #endregion
        #region khai báo dữ liệu
        List<List_History> List_History = new List<List_History>();
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        DataPoint newPoint1 = new DataPoint();
        #endregion
        #region Khai báo biến public
        public static string UserName = "";
        public static ObservableCollection<string> _queue;
        public static ObservableCollection<string> Queue_sever;
        private DispatcherTimer logoutTimer;
        private DateTime lastActivityTime;
        public ObservableCollection<string> Notifications { get; set; }
        #endregion
        #region khai bao biến private
        private static uint Value_Al_old;
        private static uint Value_Err_old;
        private static string Status_PLC;
        private int position = 0;
        private DispatcherTimer Update_Status;
        private DispatcherTimer Update_Sys;
        #endregion

        public static bool Error_UnKnow_Order;
        public double newY1;
        private bool Flag;
        private bool Flag1;
        private double _Force_max;
        private int pointCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            DataContext = this;

            Global.DataPoints1 = new List<DataPoint>();
            var workingArea = SystemParameters.WorkArea;
            _queue = new ObservableCollection<string>();
            _queue.CollectionChanged += Queue_CollectionChanged;
            socket.ConnectToServer();
            // Đặt kích thước và vị trí của cửa sổ
             this.Left = workingArea.Left -7;
             this.Top = workingArea.Top;
             this.Width = workingArea.Width + 11;
             this.Height = workingArea.Height + 5;
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


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Maximized;
            //
            PLC.StartTimer();
            Update_Status = new DispatcherTimer();
            Update_Status.Interval = TimeSpan.FromMilliseconds(200);
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
            Global.Language = "VN";
            Global.Infor = lb_Version.Content.ToString();
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
           Animation(TB_Notification);
            if(Data.Begin_Press)
            {
                Global.Pressing= true;
                Global.Force_Max = 0;
                Global.Position_Force_Max = 0;
            }
            if(Data.Done_Press)
            {
                Global.Pressing = false;
            }
            if (Global.Pressing)
            {
                Update_Datachart();
                if (Data.Force_PV > Global.Force_Max)
                {
                    Global.Force_Max = Data.Force_PV;
                    Global.Position_Force_Max = Data.Position_PV;
                }    
            }
            else if (!Global.Pressing & Flag)
            {
                Flag = false;
                Flag1 = false;
            }
            if (Global.Check_Write_Model)
            {
                Check_Write_data_Setting();
            }

        }
        private void Update_Status_Tick1000ms(object sender, EventArgs e)
        {

            try
            {
                Scan();
                Dispatcher.Invoke(() =>
                {
                    Update_Screen();
                    tb_Position.Text= Math.Round(Data.Position_PV , 2).ToString("F3");
                    tb_Force.Text = Math.Round(Data.Force_PV, 2).ToString("F3");
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
                Global.Language = "EN";
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
                Global.Language = "VN";
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
                if (Global.Language =="EN")
                {

                    Status_PLC = formattedDate + " - " + formattedtime + " - " + "Disconnect PLC";
                }
                if (Global.Language == "VN")
                {
                    Status_PLC = formattedDate + " - " + formattedtime + " - " + "Mất kết Nối PLC";
                }    
                
            }
            if (Socket_client.IsConnected)
            {
                lb_server_Connect.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                lb_server_Connect.Foreground = System.Windows.Media.Brushes.Red;
            }
            //
            float cpuUsage = cpuCounter.NextValue();
            string formattedCpuUsage = cpuUsage.ToString("F2") + "%";
            Per_CPU.Content = formattedCpuUsage;
            if(Global.Order_Code_Write_done =="" & Global.Pressing == true)
            {
                Error_UnKnow_Order = true;
            }
            else Error_UnKnow_Order = false;

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
            Pannel_Monitor.Children.Add(History_Error);
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




        public void Scan()
        {

            uint Value_Err = (uint)((Data.ID_Error2 << 16) | Data.ID_Error1);

            string code_E = "";
            // Phát hiện sự thay đổi của bit

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
                            Add_Err(code_E,path.History_EN,path.Error_EN);
                            Add_Err(code_E, path.History_VN, path.Error_VN);
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
        private void Add_Err(string code_E, string path_His, string path_Error)
        {

            List_History List_History_ = new List_History();
            System.DateTime dateTime = System.DateTime.Now;
            string Fill_json = File.ReadAllText(path_His);
            //   string json_ = File.ReadAllText(linkpath.Error);
            string json = File.ReadAllText(path_Error);
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
                            List_History_.Description = (string)obj["Description"];
                            List_History_.Solution = (string)obj["Solution"];
                            List_History_.Time = dateTime.ToString();
                            string list_Error_Json = JsonConvert.SerializeObject(List_History_);
                            List_History.Add(List_History_);
                            if (json.Length < 50)
                            {
                                json = json.Remove(json.Length - 1);
                                json = json + list_Error_Json + "]";
                                File.WriteAllText(path_Error, json);
                            }
                            else
                            {
                                json = json.Remove(json.Length - 1);
                                json = json + ",\r" + list_Error_Json + "]";
                                File.WriteAllText(path_Error, json);
                                //   List_Alarm.ItemsSource = List_History;
                            }
                        }
                    }
                }
            }
        }
        public void Update_Datachart()
        {

            pointCount++;
            newY1 = Math.Sin(pointCount * 100) + 50;
            double _Force_PV = Data.Force_PV;
            double _Position_PV = Data.Position_PV;
            newPoint1 = new DataPoint(_Position_PV, _Force_PV);
            Global.DataPoints1.Add(newPoint1);

            if (Global.DataPoints1 != null)
            {

                if (Global.Pressing & !Flag)
                {
                    if (AreTextBoxesFilled())
                    {

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
            List_Report.Force_Max = Data_Report_temp2.Force_Max;
            if (Data.Product_NG)
            {
                List_Report.Status = "NG";
            }
            else if (Data.Product_OK)
            {
                List_Report.Status = "OK";
            }
            else
            {
                List_Report.Status = "Unknow";
            }    
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
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return !string.IsNullOrWhiteSpace(ID_Model.Orrder_Code);
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
        private void Animation(TextBox sender)
        {
            TextBox textBox = (TextBox)sender;

            string mylistString = Status_PLC + string.Join("           ", List_History.Select(o => $"{o.STT}" + " - " + $"{o.Code}" + " - " + $" {o.Description}")) + "                                               ";

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
        public void Check_Write_data_Setting()
        {
            if ((Math.Round(Data.Mode1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].Mode, 3)) ||
                (Math.Round(Data.Press_Pos1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].Press_Pos, 3)) ||
                (Math.Round(Data.Press_Force1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].Press_Force, 3)) ||
                (Math.Round(Data.Press_Vel1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].Press_Vel, 3)) ||
                (Math.Round(Data.Press_Time1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].Press_Time, 3)) ||
                 (Math.Round(Data.End_Max_Force_Limit1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].End_Max_Force_Limit, 3)) ||
                 (Math.Round(Data.End_Min_Force_Limit1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].End_Min_Force_Limit, 3)) ||
                 (Math.Round(Data.End_Max_Pos_Limit1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].End_Max_Pos_Limit, 3)) ||
                 (Math.Round(Data.End_Min_Pos_Limit1, 3) != Math.Round(Global.list_model[0].Data_Func1[0].End_Min_Pos_Limit, 3)) ||
                 (Math.Round(Data.Mode2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].Mode, 3)) ||
                 (Math.Round(Data.Press_Pos2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].Press_Pos, 3)) ||
                 (Math.Round(Data.Press_Force2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].Press_Force, 3)) ||
                (Math.Round(Data.Press_Vel2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].Press_Vel, 3)) ||
                (Math.Round(Data.Press_Time2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].Press_Time, 3)) ||
                 (Math.Round(Data.End_Max_Force_Limit2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].End_Max_Force_Limit, 3)) ||
                 (Math.Round(Data.End_Min_Force_Limit2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].End_Min_Force_Limit, 3)) ||
                 (Math.Round(Data.End_Max_Pos_Limit2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].End_Max_Pos_Limit, 3)) ||
                 (Math.Round(Data.End_Min_Pos_Limit2, 3) != Math.Round(Global.list_model[0].Data_Func2[0].End_Min_Pos_Limit, 3))) 
            {
                var data = new
                {
                    Mode1 = Global.list_model[0].Data_Func1[0].Mode,
                    Press_Pos1 = Global.list_model[0].Data_Func1[0].Press_Pos,
                    Press_Force1 = Global.list_model[0].Data_Func1[0].Press_Force,
                    Press_Vel1 = Global.list_model[0].Data_Func1[0].Press_Vel,
                    Press_Time1 = Global.list_model[0].Data_Func1[0].Press_Time,
                    End_Max_Force_Limit1 = Global.list_model[0].Data_Func1[0].End_Max_Force_Limit,
                    End_Min_Force_Limit1 = Global.list_model[0].Data_Func1[0].End_Min_Force_Limit,
                    End_Max_Pos_Limit1 = Global.list_model[0].Data_Func1[0].End_Max_Pos_Limit,
                    End_Min_Pos_Limit1 = Global.list_model[0].Data_Func1[0].End_Min_Pos_Limit,
                    Mode2 = Global.list_model[0].Data_Func2[0].Mode,
                    Press_Pos2 = Global.list_model[0].Data_Func2[0].Press_Pos,
                    Press_Force2 = Global.list_model[0].Data_Func2[0].Press_Force,
                    Press_Vel2 = Global.list_model[0].Data_Func2[0].Press_Vel,
                    Press_Time2 = Global.list_model[0].Data_Func2[0].Press_Time,
                    End_Max_Force_Limit2 = Global.list_model[0].Data_Func2[0].End_Max_Force_Limit,
                    End_Min_Force_Limit2 = Global.list_model[0].Data_Func2[0].End_Min_Force_Limit,
                    End_Max_Pos_Limit2 = Global.list_model[0].Data_Func2[0].End_Max_Pos_Limit,
                    End_Min_Pos_Limit2 = Global.list_model[0].Data_Func2[0].End_Min_Pos_Limit,
                    Standard_Roto = Global.list_model[0].Height_Stand
                };
                string jsonData = JsonConvert.SerializeObject(data);
                MainWindow._queue.Add(jsonData);
            }
            else 
            
            
            
            if (!Data.Check_Done_Tranfer)
            {
                var data = new
                {
                    Check_Done_Tranfer = true
                };
                string jsonData = JsonConvert.SerializeObject(data);
                MainWindow._queue.Add(jsonData);
                // Console.WriteLine($"read : {Data.Write_Model_Done}");
            }
            else
            {

                Global.Check_Write_Model = false;
                Global.Write_Done = true;
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




    }
}
