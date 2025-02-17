
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.IO;
using IOPath = System.IO.Path;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using OxyPlot;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Auto.xaml
    /// </summary>
    public partial class Auto : UserControl
    {
        Link_Path linkpath = new Link_Path();
        Common Common = new Common();
        private DispatcherTimer timer;
        public static string Order_Code;
        private static bool Update_done = false;
        private static int check;
        private static double Height_Screen;
        PLC plc = new PLC();
        public Auto()
        {
            InitializeComponent();
            Loaded += Auto_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Auto_Unloaded;
            var model1 = CreatePlotModel("Đồ Thị Hoạt Động Sản Phẩm:" + Global.OrderCode, "Thông số", "Vị Trí (mm)", 400, "Lực Ép (N/m)", 4000);
            plotView1.Model = model1;
        }

        private void Auto_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
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
                    if (PLC.IsConnected & check < 5)
                    {
                        check++;
                        Load_Model(false);
                    }
                    if (!PLC.IsConnected)
                    { check = 0; }
                    Update_Screen();
                    // if (Data.Process != _Process)
                    // {
                    //     //Update_Data();
                    // }


                });
                Check_Update_done();
                if (Global.DataPoints1 != null & Global.Start)
                {
                    UpdateAxes(plotView1.Model.Series[0] as LineSeries, plotView1, Global.DataPoints1);
                }
                // Cập nhật đồ thị
                plotView1.InvalidatePlot(true);

            }
            catch
            {
            }
        }
        private void Auto_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Update_Screen()
        {


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TextBox textBox = (TextBox)sender;
            // string textboxName = textBox.Name;
            // if ( (textboxName == "Auto_Order_Code") & Auto_Order_Code.Text.Length >2 & Auto_Order_Code.Text.Contains(" "))
            // {
            //     Strim_ordercode(Order_Code);
            // }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;



        }
        public void Strim_ordercode(string ordercode)
        {
          //  string[] parts;
          //  string _TrucID = "";
          //  string _RotorID = "";
          //  if (Auto_Order_Code.Text != null)
          //  {
          //      parts = Auto_Order_Code.Text.Split(new char[] { '-', ';', ',' });
          //      if (parts.Length >= 2)
          //      {
          //          _TrucID = parts[0];
          //          _RotorID = parts[1];
          //          //Auto_Order_Code.Text = ordercode;
          //          Fill_Value_Mode(_TrucID, _RotorID);
          //          Load_Model(true);
          //      }
          //      else
          //      {
          //          MessageBox.Show("Mã OrderCode không hợp lệ!");
          //          Global.OrderCode = "";
          //      }
          //  }
        }
        private void Load_Model(bool Message)
        {
          //  if (!AreTextBoxesFilled())
          //  {
          //      if (Message)
          //      {
          //          MessageBox.Show("Vui Lòng Kiểm tra lại các thông số không có trong dữ liệu đầu vào!");
          //      }
          //      var data = new
          //      {
          //          H_Stand = 1.0,
          //          Momen_set = 1.0,
          //          Check_Update = true,
          //
          //      };
          //      string jsonData = JsonConvert.SerializeObject(data);
          //      MainWindow._queue.Add(jsonData);
          //      Global.OrderCode = "";
          //  }
          //  else
          //  {
          //      // dòng này để đẩy dữ liệu xuống PLC, nhớ có 1 bit check done dữ liệu
          //      if (Message)
          //      {
          //          MessageBox.Show("Scan Order code thành công");
          //          Global.OrderCode = Auto_Order_Code.Text;
          //      }
          //      var data = new
          //      {
          //          H_Stand = Auto_Hstand.Text,
          //          Momen_set = Auto_force.Text,
          //          Check_Update = true,
          //
          //      };
          //      string jsonData = JsonConvert.SerializeObject(data);
          //      MainWindow._queue.Add(jsonData);
          //      Update_done = true;
          //  }
        }
        private void Check_Update_done()
        {
            if (Update_done & Data.Check_Update)
            {
                MessageBox.Show("Đã Update Thành Công");
                Update_done = false;
            }
        }
        private void Fill_Value_Mode(string TrucID, string RotorID)
        {

            string json = File.ReadAllText(linkpath.Model);
            int flag = 0;
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["TrucID"] == TrucID & (string)obj["RotorID"] == RotorID)
                    {
                        Data_Report_temp2.Model = (string)obj["Model"];
                        Data_Report_temp2.TrucID = (string)obj["TrucID"];
                        Data_Report_temp2.RotorID = (string)obj["RotorID"];
                        Data_Report_temp2.Beer_Up = Fill_ID(linkpath.Beer_Up, (string)obj["Beer_Up"]);
                        Data_Report_temp2.Beer_Down = Fill_ID(linkpath.Beer_Down, (string)obj["Beer_Down"]);
                        Data_Report_temp2.Jig_Up = Fill_ID(linkpath.Jig_Up, (string)obj["Jig_Up"]);
                        Data_Report_temp2.Jig_Mid = Fill_ID(linkpath.Jig_Mid, (string)obj["Jig_Mid"]);
                        Data_Report_temp2.Jig_Down = Fill_ID(linkpath.Jig_Down, (string)obj["Jig_Down"]);
                        Data_Report_temp2.HStand = (string)obj["HStand"];
                        Data_Report_temp2.Force = (string)obj["Force"];
                        flag = 1;
                        break;
                    }


                }
                if (flag == 0)
                {
                    Data_Report_temp2.OrderCode = "";
                    MessageBox.Show("Mã Order code không hợp lệ!");
                }


            }
        }
        private static string Fill_ID(string linkpath_json, string Value)
        {
            string json = File.ReadAllText(linkpath_json);
            var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
            var data = System.Text.Json.JsonSerializer.Deserialize<Beer_Jig[]>(json, options);

            var newData = new List<Beer_Jig>();

            foreach (var item in data)
            {
                if (item.ID == Value)
                {
                    return item.ID;
                }
            }
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
            // Write back to file
            File.WriteAllText(linkpath_json, newJsonString);
            return "";
        }
        public PlotModel CreatePlotModel(string title, string title_series, string title_x, double max_x, string title_y, double max_y)
        {
            var model = new PlotModel { };

            // Thêm trục X
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                FontSize = 15,
                Title = title_x,
                // Minimum = 0,
                // Maximum = 1
            });

            // Thêm trục Y
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                FontSize = 15,
                Title = title_y,
                // Minimum = 0,
                // Maximum = 1
            });

            // Tạo series dữ liệu
            var series = new LineSeries
            {
                Title = title_series,
                MarkerType = MarkerType.Circle
            };
            model.Series.Add(series);

            return model;
        }


        public void UpdateAxes(LineSeries series, PlotView plotView, List<DataPoint> dataPoint)
        {
            series.Points.Clear();
            for (int i = 0; i < dataPoint.Count; i++)
            {
                series.Points.Add(new DataPoint(dataPoint[i].X, dataPoint[i].Y)); // i là trục x, data[i] là trục y
            }
            if (series.Points.Count > 2)
            {
                plotView1.Model.Axes[0].Minimum = series.Points.Min(p => p.X);
                plotView1.Model.Axes[0].Maximum = series.Points.Max(p => p.X);
                plotView1.Model.Axes[1].Minimum = series.Points.Min(p => p.Y);
                plotView1.Model.Axes[1].Maximum = series.Points.Max(p => p.Y);
            }
        }





        
        private void Auto_RotorID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Monitor_Order monitor_order = new Monitor_Order(); 
            monitor_order.ShowDialog();
        }
    }
}