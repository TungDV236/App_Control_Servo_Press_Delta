using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History_Alarm : UserControl
    {
        Common Common = new Common();
        Link_Path path = new Link_Path();
        private DispatcherTimer timer;
        public History_Alarm()
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
            LoadAls();

        }
        private void History_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Data.Alarm > 0)
            {

            }
            LoadAls();
        }

        private void LoadAls()
        {
            List<Items_Alarm> items_E = new List<Items_Alarm>();
            int index = 1;
            //try
            //{
            string List_Show = File.ReadAllText(path.Alarm);
            if (List_Show.Length > 0)
            {
                JArray List_Show_array = JArray.Parse(List_Show);
                foreach (JObject obj in List_Show_array)

                {
                    items_E.Add(new Items_Alarm { STT = index, Code = (string)obj["Code"], Content_ = (string)obj["Content_"], Solution = (string)obj["Solution"], Time = (string)obj["Time"] });
                    index++;
                }
                items_E.Reverse();
                for (int i = 0; i < items_E.Count; i++)
                {
                    items_E[i].STT = i + 1;
                }
                //  List_Error.ItemsSource = items_E;
                List_Alarm.ItemsSource = null;
                List_Alarm.ItemsSource = items_E;
                //  List_Error.ItemsSource =  List<li> Users { get; set; }
            }
            //}
            //catch
            //{ }
        }


        private void Clear_Al()
        {
            //
            string Err = "[]";
            File.WriteAllText(path.Alarm, "");
            File.WriteAllText(path.Alarm, Err);
        }

        private void bt_Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName != "")
            {
                Clear_Al();
                LoadAls();
            }
            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }
        }
    }
}
