using App_Control_Servo_Press_Delta.Class;
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
using App_Control_Servo_Press_Delta.Popup;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        Link_Path linkpath = new Link_Path();
        Common Common = new Common();
        Excel excel = new Excel();
        private DispatcherTimer timer;
        DateTime Time_Stop;
        DateTime Time_Start;
        public List<Position> List_Position { get; set; }
        List<Data_Report> List_Report_Change = new List<Data_Report>();
        static int cnt = 0;
        public Report()
        {
            InitializeComponent();
            LoadTimeComboboxes();
            SetDefaultValues();
            Loaded += Report_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Report_Unloaded;
            datePicker_start.SelectedDate = DateTime.Today;
            datePicker_stop.SelectedDate = DateTime.Today;
            List_Position = new List<Position>();
            Global.List_Position_all = new List<Position>();
            Global.DataPoints_Chart = new List<DataPoint>();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (cnt < 1)
            {
                Read_time();
                trim_date(datePicker_start.SelectedDate.Value);
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
        private void DatePicker_SelectedDateChanged_start(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            // Lấy ngày đã chọn
            if (datePicker_start.SelectedDate.HasValue )
            {
                DateTime selectedDate = datePicker_start.SelectedDate.Value;
                string[] part = selectedDate.ToString().Split(' ');
                Select_date_start.Text = part[0];

            }

        }
        private void DatePicker_SelectedDateChanged_stop(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Lấy ngày đã chọn
            if (datePicker_stop.SelectedDate.HasValue )
            {
                DateTime selectedDate = datePicker_stop.SelectedDate.Value;
                string[] part = selectedDate.ToString().Split(' ');
                Select_date_stop.Text = part[0];

            }
        }
        private void YourMethod()
        {

        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            DateTime parsedDate; 
            bool isValid = DateTime.TryParseExact((textBox.Text), "dd/mm/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
            if (isValid &( textboxName == "Select_date_start" || textboxName == "Select_date_stop"))
            {
                DateTime date = DateTime.Parse(textBox.Text + " 11:59:59 PM");
                //  MessageBox.Show($"{textBox.Text} đúng định dạng !");
                datePicker_start.SelectedDate = date;
            }
            else if (!isValid & (textboxName == "Select_date_start" || textboxName == "Select_date_stop"))
            {
                MessageBox.Show($"{textBox.Text} sai định dạng !");
                string[] part = DateTime.Today.ToString().Split(' ');
                textBox.Text = part[0];
                datePicker_start.SelectedDate = DateTime.Today;
            }

        }
        private void Report_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fill_Value_Mode();
        }
        private void Init_data()
        {



        }
        private void Fill_Value_Mode()
        {
            try
            {

                var selectedRow = List_Report.SelectedItem as Data_Report;
                if (selectedRow != null)
                {
                    List_Report_Change.Clear();
                    List_Report_Change = Global.List_report.Where(item => item.Time == selectedRow.Time).ToList();
                    Global.Order_Code_Report = List_Report_Change[0].OrderCode;
                    Global.DataPoints_Chart.Clear();
                    Global.DataPoints_Chart.AddRange(List_Report_Change[0].Chart);
                }
            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
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
        private void calendar_SelectedDateChanged_start(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Lấy ngày đã chọn
            datePicker_start.IsDropDownOpen = false;
        }
        private void trim_date(DateTime dateTime)
        {
            string[] part = dateTime.ToString().Split(' ');
            // selectedDateText.Text = $"Ngày đã chọn: {selectedDate.ToShortDateString()}"; // Hiển thị ngày
            Common.Load_View_Report(List_Report, part[0].Replace('/', '_'));
        }
        private void calendar_SelectedDateChanged_stop(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Lấy ngày đã chọn

        }



        private void Move_datetime_start(object sender, RoutedEventArgs e)
        {
            datePicker_start.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }
        private void Move_datetime_stop(object sender, RoutedEventArgs e)
        {
           // datePicker_start.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
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
        }
        private void Read_time()
        {
            DateTime parsedDateStart;
            DateTime parsedDateStop;
            if (DateTime.TryParseExact(Select_date_start.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDateStart))
            {
                int hour_start = int.TryParse(Hour_start.SelectedItem?.ToString(), out int hStart) ? hStart : 0;
                int minute_start = int.TryParse(Minute_start.SelectedItem?.ToString(), out int mStart) ? mStart : 0;
                int second_start = int.TryParse(Second_start.SelectedItem?.ToString(), out int sStart) ? sStart : 0;
                Time_Start = new DateTime(parsedDateStart.Year, parsedDateStart.Month, parsedDateStart.Day, hour_start, minute_start, second_start);
            }
            if (DateTime.TryParseExact(Select_date_stop.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDateStop))
            {

                int hour_stop = int.TryParse(Hour_Stop.SelectedItem?.ToString(), out int hStop) ? hStop : 0;
                int minute_stop = int.TryParse(Minute_Stop.SelectedItem?.ToString(), out int mStop) ? mStop : 0;
                int second_stop = int.TryParse(Second_Stop.SelectedItem?.ToString(), out int sStop) ? sStop : 0;
                Time_Stop = new DateTime(parsedDateStop.Year, parsedDateStop.Month, parsedDateStop.Day, hour_stop, minute_stop, second_stop);
            }
        }


        private void Click_bt_chart(object sender, RoutedEventArgs e)
        {

            Chart_Report chart = new Chart_Report();

            chart.ShowDialog();
        }

        private void Click_bt_Export(object sender, RoutedEventArgs e)
        {
            if (Global.List_report !=null)
            {

                excel.Export_Report_All_File("Template_Report", "Chọn vị trí lưu File", ConvertListToString(Global.List_report));
            }   
            else
            {
                MessageBox.Show("Vui Lòng lựa chọn dữ liệu cần xuất Excel!");
            }    
        }

        private void Click_search(object sender, RoutedEventArgs e)
        {
            Read_time();
            Common.Load_Fill_View_Report(List_Report, Time_Start, Time_Stop, tb_Model_search.Text);
        }
        static string ConvertListToString(List<Data_Report> items)
        {
            // Sử dụng String.Join để chuyển đổi danh sách thành chuỗi
            return string.Join(", ", items);
        }
    }

}
