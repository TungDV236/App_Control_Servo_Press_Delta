using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta.Popup;
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
using System.IO;
using static App_Control_Servo_Press_Delta.LoginWindow;
using OxyPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Newtonsoft.Json;
using System.Text.Json;
using System.Windows.Threading;
using System.Globalization;
using System.Reflection;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        Link_Path linkpath = new Link_Path();
        Common Common = new Common();
        private DispatcherTimer timer;
        TimeSpan Time_Stop;
        TimeSpan Time_Start;
        public List<Position> List_Position { get; set; }
        static int cnt = 0;
        public Report()
        {
            InitializeComponent();
            LoadTimeComboboxes();
            SetDefaultValues();
            Loaded += Report_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Report_Unloaded;
            datePicker.SelectedDate = DateTime.Today;
            List_Position = new List<Position>();
            Global.List_Position_all = new List<Position>();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (cnt < 1)
            {
                Read_time();
                trim_date(datePicker.SelectedDate.Value);
                fill_time();
                cnt++;
            }

        }
        private void Report_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); // Cập nhật mỗi 100ms
            timer.Tick += Timer_Tick;
            timer.Start(); // Bắt đầu timer
            YourMethod();
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.LostFocus += TextBox_LostFocus;
            }
            foreach (var Combobox in Common.FindVisualChildren<ComboBox>(this))
            {
                Combobox.SelectionChanged += Combobox_Changed;
            }

        }
        private void Report_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void DatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Lấy ngày đã chọn
            if (datePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = datePicker.SelectedDate.Value;

                string[] part = selectedDate.ToString().Split(' ');
                Select_date.Text = part[0];
                // selectedDateText.Text = $"Ngày đã chọn: {selectedDate.ToShortDateString()}"; // Hiển thị ngày
                Common.Load_View_Report(List_Report, part[0].Replace('/', '_'));
                fill_time();
            }
        }
        private void YourMethod()
        {

            List_Report.AddHandler(DataGrid.SelectionChangedEvent, new SelectionChangedEventHandler(Model_SelectionChanged));
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            DateTime parsedDate;
            bool isValid = DateTime.TryParseExact(textBox.Text, "dd/mm/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
            if (isValid)
            {
                DateTime date = DateTime.Parse(textBox.Text + " 11:59:59 PM");
                //  MessageBox.Show($"{textBox.Text} đúng định dạng !");
                datePicker.SelectedDate = date;
            }
            else
            {
                MessageBox.Show($"{textBox.Text} sai định dạng !");
                string[] part = DateTime.Today.ToString().Split(' ');
                textBox.Text = part[0];
                datePicker.SelectedDate = DateTime.Today;
            }

        }
        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRow = List_Report.SelectedItem as DataView_Report;
            if (selectedRow != null)
            {
                Fill_Value_Mode(selectedRow.Time.ToString());
            }
        }
        private void Fill_Value_Mode(string time)
        {
            DateTime selectedDate = datePicker.SelectedDate.Value;
            string[] part = selectedDate.ToString().Split(' ');
            // selectedDateText.Text = $"Ngày đã chọn: {selectedDate.ToShortDateString()}"; // Hiển thị ngày
            string filepath = part[0].Replace('/', '_') + "_Report.json";
            string json = File.ReadAllText(System.IO.Path.Combine("Log", filepath));
            int flag = 0;
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["Time"] == time)
                    {
                        Data_Report_temp.Time = (string)obj["Time"];
                        Data_Report_temp.OrderCode = (string)obj["OrderCode"];
                        Data_Report_temp.Model = (string)obj["Model"];
                        Data_Report_temp.TrucID = (string)obj["TrucID"];
                        Data_Report_temp.RotorID = (string)obj["RotorID"];
                        Data_Report_temp.Beer_Up = (string)obj["Beer_Up"];
                        Data_Report_temp.Beer_Down = (string)obj["Beer_Down"];
                        Data_Report_temp.Jig_Up = (string)obj["Jig_Up"];
                        Data_Report_temp.Jig_Mid = (string)obj["Jig_Mid"];
                        Data_Report_temp.Jig_Down = (string)obj["Jig_Down"];
                        Data_Report_temp.HStand = (string)obj["HStand"];
                        Data_Report_temp.Force = (string)obj["Force"];
                        Data_Report_temp.Force_Max = (string)obj["Force_Max"];
                        strim_Position((string)obj["Position"]);
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {

                }


            }
        }
        private void strim_Position(string data)
        {
            string[] pairs = data.Split(',');

            // Tạo List để lưu trữ các cặp mã và giá trị
            List_Position.Clear();
            foreach (var pair in pairs)
            {
                // Phân tách từng cặp số bằng dấu gạch dưới
                var parts = pair.Split('_');
                if (parts.Length == 2)
                {

                    float Momen = 0;
                    float PTS = 0;
                    if (float.TryParse(parts[0].Replace('.', ','), out float result))
                    {
                        // Làm tròn đến 2 chữ số thập phân
                        Momen = (float)Math.Round(result, 2);
                    }
                    if (float.TryParse(parts[1].Replace('.', ','), out float result1))
                    {
                        // Làm tròn đến 2 chữ số thập phân
                        PTS = (float)Math.Round(result1, 2);
                    }
                    List_Position.Add(new Position(PTS, Momen));
                }
            }
        }
        private void calendar_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Lấy ngày đã chọn
            datePicker.IsDropDownOpen = false;
        }
        private void trim_date(DateTime dateTime)
        {
            string[] part = dateTime.ToString().Split(' ');
            // selectedDateText.Text = $"Ngày đã chọn: {selectedDate.ToShortDateString()}"; // Hiển thị ngày
            Common.Load_View_Report(List_Report, part[0].Replace('/', '_'));
            fill_time();
        }



        private void Move_datetime(object sender, RoutedEventArgs e)
        {
            datePicker.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }
        private void LoadTimeComboboxes()
        {
            // Tải giờ vào ComboBox
            for (int i = 0; i < 24; i++)
            {
                Hour_start.Items.Add(i.ToString("D2")); // D2 định dạng số với 2 chữ số
                Hour_Stop.Items.Add(i.ToString("D2")); // D2 định dạng số với 2 chữ số
            }

            // Tải phút và giây vào ComboBox
            for (int i = 0; i < 60; i++)
            {
                Minute_start.Items.Add(i.ToString("D2"));
                Second_start.Items.Add(i.ToString("D2"));
                Minute_Stop.Items.Add(i.ToString("D2"));
                Second_Stop.Items.Add(i.ToString("D2"));
            }
        }
        private void SetDefaultValues()
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedtime = dateTime.ToString("HH:mm:ss");
            string[] pairs = formattedtime.Split(':');
            // Cài đặt giá trị hiển thị ban đầu
            Hour_start.SelectedItem = "00";   // Giờ mặc định
            Minute_start.SelectedItem = "00";  // Phút mặc định
            Second_start.SelectedItem = "00";  // Giây mặc định
            Hour_Stop.SelectedItem = pairs[0];   // Giờ mặc định
            Minute_Stop.SelectedItem = pairs[1];  // Phút mặc định
            Second_Stop.SelectedItem = pairs[2];  // Giây mặc định
        }
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            Read_time();
            //   MessageBox.Show($"Bạn cần lọc thời gian từ: {Time_Start} đến {Time_Stop}");
            fill_time();
        }
        private void Read_time()
        {
            string hour_start = Hour_start.SelectedItem?.ToString() ?? "00";
            string minute_start = Minute_start.SelectedItem?.ToString() ?? "00";
            string second_start = Second_start.SelectedItem?.ToString() ?? "00";
            string hour_stop = Hour_Stop.SelectedItem?.ToString() ?? "00";
            string minute_stop = Minute_Stop.SelectedItem?.ToString() ?? "00";
            string second_stop = Second_Stop.SelectedItem?.ToString() ?? "00";
            Time_Start = new TimeSpan(int.Parse(hour_start), int.Parse(minute_start), int.Parse(second_start));
            Time_Stop = new TimeSpan(int.Parse(hour_stop), int.Parse(minute_stop), int.Parse(second_stop));
        }
        private void fill_time()
        {
            var filteredRecords = Global.List_report.Where(r =>
            {
                TimeSpan recordTime;
                return TimeSpan.TryParse(r.Time, out recordTime) && recordTime >= Time_Start && recordTime <= Time_Stop;
            }).ToList();

            // Hiển thị kết quả
            string mylistString = "";
            int index = 1;
            Global.List_report_temp.Clear();
            foreach (var record in filteredRecords)
            {
                Global.List_report_temp.Add(new DataView_Report { STT = index, Model = record.Model, TrucID = record.TrucID, RotorID = record.RotorID, Force_Max = record.Force_Max, Force = record.Force, Time = record.Time });
                mylistString = mylistString + record.Time;
                index++;

            }
            List_Report.ItemsSource = null;
            List_Report.ItemsSource = Global.List_report_temp;
            //   MessageBox.Show($"Thời gian đã lọc được là  {mylistString}");
            // Hiển thị kết quả
        }
        private void Click_bt_Export_model(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = datePicker.SelectedDate.Value;
            string[] part = selectedDate.ToString().Split(' ');
            // selectedDateText.Text = $"Ngày đã chọn: {selectedDate.ToShortDateString()}"; // Hiển thị ngày
            Excel excel = new Excel();
            try
            {
                if (Data_Report_temp.Time != null)
                {
                    excel.Export_Chart_File(part[0].Replace('/', '_') + "_" + Data_Report_temp.Time.Replace(':', '_'), List_Position);
                }
                else
                {

                    MessageBox.Show("Vui Lòng lựa chọn dữ liệu cần xuất Excel!");
                }

            }
            catch { }

        }
        private void Click_bt_Export_All(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = datePicker.SelectedDate.Value;
            string[] part = selectedDate.ToString().Split(' ');
            // selectedDateText.Text = $"Ngày đã chọn: {selectedDate.ToShortDateString()}"; // Hiển thị ngày
            Excel excel = new Excel();
            try
            {
                if (Global.List_report_all != null)
                {
                    excel.Export_Report_All_File(part[0].Replace('/', '_'));
                }
                else
                {

                    MessageBox.Show("Vui Lòng lựa chọn dữ liệu cần xuất Excel!");
                }

            }
            catch { }
        }
    }

}
