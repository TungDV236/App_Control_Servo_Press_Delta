using Newtonsoft.Json.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;
using IOPath = System.IO.Path;
using System.Text.Json;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Collections;
using System.Data.SqlClient;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for History_Error.xaml
    /// </summary>
    public partial class History_Error : UserControl
    {
        History_UL History_UL = new History_UL();//-----
        private static ushort _value;

        //  MainWindow  Mainw = new MainWindow();
        Common Common = new Common();
        Link_Path path = new Link_Path();
        private DispatcherTimer timer;
        int cnt1 = 0;
        public History_Error()
        {
            InitializeComponent();
            Loaded += History_Loaded;  // Thêm sự kiện Loaded
            Unloaded += History_Unloaded;
        }
        private void History_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
            LoadErrs();
            Loadlog();

            // List_Err1 = History_UL.GetAllUsers();
        }

        private void History_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {


            LoadErrs();
            //  Main();

        }

        private void LoadErrs()
        {
            List<Items_Error> items_E = new List<Items_Error>();
            int index = 1;
            //try
            //{
            string List_Show = File.ReadAllText(path.Error_EN);
            if (List_Show.Length > 0)
            {
                JArray List_Show_array = JArray.Parse(List_Show);
                foreach (JObject obj in List_Show_array)

                {
                    items_E.Add(new Items_Error { STT = index, Code = (string)obj["Code"], Content_ = (string)obj["Description"], Solution = (string)obj["Solution"], Time = (string)obj["Time"] });
                    index++;
                }
                items_E.Reverse();
                for (int i = 0; i < items_E.Count; i++)
                {
                    items_E[i].STT = i + 1;
                }
                //  List_Error.ItemsSource = items_E;
                List_Err.ItemsSource = null;
                List_Err.ItemsSource = items_E;
                //  List_Error.ItemsSource =  List<li> Users { get; set; }
            }
            //}
            //catch
            //{ }
        }
        private void Loadlog()
        {
            List<Data_Log> items = new List<Data_Log>();
            int index = 1;
            try
            {
            string List_Show = File.ReadAllText(path.Log);
            if (List_Show.Length > 0)
            {
                JArray List_Show_array = JArray.Parse(List_Show);
                foreach (JObject obj in List_Show_array)

                {
                    items.Add(new Data_Log { No = index, User = (string)obj["Code"], Log = (string)obj["Log"], Time = (string)obj["Time"] });
                    index++;
                }
                items.Reverse();
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].No = i + 1;
                }
                //  List_Error.ItemsSource = items_E;
                List_History_Operation.ItemsSource = null;
                List_History_Operation.ItemsSource = items;
                //  List_Error.ItemsSource =  List<li> Users { get; set; }
            }
            }
            catch
            { }
        }
        private void Clear_Errs()
        {
            //
            string time = " ";
            string Err = "[]";
            File.WriteAllText(path.Error_EN, "");
            File.WriteAllText(path.Error_EN, Err);
        }

        private void bt_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName != "")
            {
                Clear_Errs();
                LoadErrs();

            }
            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }
        }
    }
}
