
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

        Link_Path path = new Link_Path();
        Common Common = new Common();
        private DispatcherTimer timer;
        public static string Order_Code;
        private static bool Update_done = false;
        private static int check;
        private static double Height_Screen;
        private static bool Receive;
        PLC plc = new PLC();
        
        public Auto()
        {
            InitializeComponent();
            Loaded += Auto_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Auto_Unloaded;
            var model1 = CreatePlotModel( "Thông số", "Vị Trí (mm)", 400, "Lực Ép (N/m)", 4000);
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
            Init_data();
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
                    }
                    if (!PLC.IsConnected)
                    { check = 0; }
                    Update_Screen();


                });
                if (Global.DataPoints1 != null & Global.Pressing)
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
            timer.Tick -= Timer_Tick;
            timer.Stop();
        }
        private void Update_Screen()
        {
            if (Global.Update_Order)
            {
                Fill_Value_Mode();
                Global.Update_Order = false;
                
            }
            if (Data.Product_OK )
            {
                lb_Status.Background = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                lb_Status.Content = "OK";
                lb_Status.Visibility = Visibility.Visible;
            }
            else if (Data.Product_NG)
            {
                lb_Status.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                lb_Status.Content = "NG";
                lb_Status.Visibility = Visibility.Visible;
            }
            else
            {
                lb_Status.Visibility = Visibility.Hidden;
            }    
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;



        }
        private void Init_data()
        {
            Global.Data_Auto_FC1 = new List<DataFunC>
           {
                new DataFunC
                {
                    Mode = 0,
                    Press_Condition = "",
                    Press_Pos = 0,
                    Press_Force = 0,
                    Press_Vel = 0,
                    Press_Time = 0,
                    End_Max_Force_Limit = 0,
                    End_Min_Force_Limit = 0,
                    End_Max_Pos_Limit = 0,
                    End_Min_Pos_Limit = 0
                }
             };
            Global.Data_Auto_FC2 = new List<DataFunC>
           {
                new DataFunC
                {
                    Mode = 0,
                    Press_Condition = "",
                    Press_Pos = 0,
                    Press_Force = 0,
                    Press_Vel = 0,
                    Press_Time = 0,
                    End_Max_Force_Limit = 0,
                    End_Min_Force_Limit = 0,
                    End_Max_Pos_Limit = 0,
                    End_Min_Pos_Limit = 0
                }
             };
            Global.list_model = new List<List_Model>
           {
                new List_Model
                {
                     Model="0",
                    ID_Shaft="0",
                    ID_Rotor="0",
                    ID_Bearings_Up="0",
                    ID_Bearings_Down="0",
                    Jig_Up="0",
                    Jig_Mid="0",
                    Jig_Down="0",
                    Height_Stand=0,
                    Thickness_Jig_Up=0,
                    Thickness_Jig_Down=0,
                    Origin_Position=0,
                    Origin_Velocity=0,
                    Standby_Position=0,
                    Standby_Velocity=0,
                    Standby_Time=0,
                    Data_Func1 = new List<DataFunC>
                            {
                                new DataFunC
                                {
                                Mode =0,
                                Press_Condition="",
                                Press_Pos =0,
                                Press_Force =0,
                                Press_Vel =0,
                                Press_Time =0,
                                End_Max_Force_Limit =0,
                                End_Min_Force_Limit =0,
                                End_Max_Pos_Limit =0,
                                End_Min_Pos_Limit =0
                            } },
                    Data_Func2 = new List<DataFunC>
                            {
                                new DataFunC
                                {
                                Mode =0,
                                Press_Condition="",
                                Press_Pos =0,
                                Press_Force =0,
                                Press_Vel =0,
                                Press_Time =0,
                                End_Max_Force_Limit =0,
                                End_Min_Force_Limit =0,
                                End_Max_Pos_Limit =0,
                                End_Min_Pos_Limit =0
                            } }

             } };
        }
        private void Fill_Value_Mode()
        {
            try
            {
                bool flag = false;
                string json = File.ReadAllText(path.Model);
                if (json.Length > 0)
                {
                    List<List_Model> jsonArray = JsonConvert.DeserializeObject<List<List_Model>>(json);
                    //JArray jsonArray = JArray.Parse(json);
                    foreach (var obj in jsonArray)
                    {
                        if ((string)obj.Model == Global.Model)
                        {
                            if ((string)obj.ID_Rotor != Global.ID_Rotor)
                            {
                                MessageBox.Show(" Mã Rotor cài đặt Model không đúng");
                            }
                            else if ((string)obj.ID_Shaft != Global.ID_Shaft)
                            {
                                MessageBox.Show(" Mã Shaft cài đặt Model không đúng");
                            }
                            else if ((string)obj.ID_Bearings_Up != Global.ID_BearingsU)
                            {
                                MessageBox.Show(" Mã Bi trên cài đặt Model không đúng");
                            }
                            else if ((string)obj.ID_Bearings_Down != Global.ID_BearingsD)
                            {
                                MessageBox.Show(" Mã Bi dưới cài đặt Model không đúng");
                            }
                            else
                            {
                                Global.list_model[0].Model = obj.Model;
                                Global.list_model[0].ID_Rotor = obj.ID_Rotor;
                                Global.list_model[0].ID_Shaft = obj.ID_Shaft;
                                Global.list_model[0].ID_Bearings_Up = obj.ID_Bearings_Up;
                                Global.list_model[0].ID_Bearings_Down = obj.ID_Bearings_Down;
                                Global.list_model[0].Jig_Up = obj.Jig_Up;
                                Global.list_model[0].Jig_Mid = obj.Jig_Mid;
                                Global.list_model[0].Jig_Down = obj.Jig_Down;
                                Global.list_model[0].Height_Stand = obj.Height_Stand;
                                Global.list_model[0].Thickness_Jig_Up = obj.Thickness_Jig_Up;
                                Global.list_model[0].Thickness_Jig_Down = obj.Thickness_Jig_Down;
                                Global.list_model[0].Origin_Position = obj.Origin_Position;
                                Global.list_model[0].Origin_Velocity = obj.Origin_Velocity;
                                Global.list_model[0].Standby_Position = obj.Standby_Position;
                                Global.list_model[0].Standby_Time = obj.Standby_Time;
                                Global.list_model[0].Standby_Velocity = obj.Standby_Velocity;
                                Global.list_model[0].Data_Func1.Clear();
                                Global.list_model[0].Data_Func2.Clear();
                                Global.list_model[0].Data_Func1.AddRange(obj.Data_Func1);
                                Global.list_model[0].Data_Func2.AddRange(obj.Data_Func2);
                                tb_Order_Code.Text = Global.Order_Code;
                                tb_Model.Text = obj.Model;
                                tb_Rotor.Text = obj.ID_Rotor;
                                tb_Shaft.Text = obj.ID_Shaft;
                                tb_BearingU.Text = obj.ID_Bearings_Up;
                                tb_BearingD.Text = obj.ID_Bearings_Down;
                                tb_JigU.Text = obj.Jig_Up;
                                tb_JigM.Text = obj.Jig_Mid;
                                tb_JigD.Text = obj.Jig_Down;
                                tb_Stand_Height.Text = string.Format("{0:F2}", obj.Height_Stand);
                                tb_Origin_Position.Text = string.Format("{0:F2}", obj.Origin_Position);
                                tb_Origin_Velocity.Text = string.Format("{0:F2}", obj.Origin_Velocity);
                                tb_Standby_PST.Text = string.Format("{0:F2}", obj.Standby_Position);
                                tb_Velocity_Standby.Text = string.Format("{0:F2}", obj.Standby_Velocity);
                                tb_Standby_Time.Text = string.Format("{0:F2}", obj.Standby_Time);
                                tb_Pressing_condition1.Text = Global.list_model[0].Data_Func1[0].Press_Condition.ToString();
                                tb_Pressing_Position1.Text = Global.list_model[0].Data_Func1[0].Press_Pos.ToString();
                                tb_Pressing_Force1.Text = Global.list_model[0].Data_Func1[0].Press_Force.ToString();
                                tb_Pressing_Velocity1.Text = Global.list_model[0].Data_Func1[0].Press_Vel.ToString();
                                tb_Pressing_Time1.Text = Global.list_model[0].Data_Func1[0].Press_Time.ToString();
                                tb_Max_Force1.Text = Global.list_model[0].Data_Func1[0].End_Max_Force_Limit.ToString();
                                tb_Min_Force1.Text = Global.list_model[0].Data_Func1[0].End_Min_Force_Limit.ToString();
                                tb_Max_Position1.Text = Global.list_model[0].Data_Func1[0].End_Max_Pos_Limit.ToString();
                                tb_Min_Position1.Text = Global.list_model[0].Data_Func1[0].End_Min_Pos_Limit.ToString();

                                tb_Pressing_condition2.Text = Global.list_model[0].Data_Func2[0].Press_Condition.ToString();
                                tb_Pressing_Position2.Text = Global.list_model[0].Data_Func2[0].Press_Pos.ToString();
                                tb_Pressing_Force2.Text = Global.list_model[0].Data_Func2[0].Press_Force.ToString();
                                tb_Pressing_Velocity2.Text = Global.list_model[0].Data_Func2[0].Press_Vel.ToString();
                                tb_Pressing_Time2.Text = Global.list_model[0].Data_Func2[0].Press_Time.ToString();
                                tb_Max_Force2.Text = Global.list_model[0].Data_Func2[0].End_Max_Force_Limit.ToString();
                                tb_Min_Force2.Text = Global.list_model[0].Data_Func2[0].End_Min_Force_Limit.ToString();
                                tb_Max_Position2.Text = Global.list_model[0].Data_Func2[0].End_Max_Pos_Limit.ToString();
                                tb_Min_Position2.Text = Global.list_model[0].Data_Func2[0].End_Min_Pos_Limit.ToString();
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
                                    Height_Jig_Top = Global.list_model[0].Thickness_Jig_Up,
                                    Height_Jig_Bottom = Global.list_model[0].Thickness_Jig_Down,
                                    Standard_Roto = Global.list_model[0].Height_Stand
                                };
                                string jsonData = JsonConvert.SerializeObject(data);
                                MainWindow._queue.Add(jsonData);
                                Global.Check_Write_Model = true;
                            }

                            flag = true;
                        }
                    }
                    if (!flag)
                    {

                    }
                    flag = false;
                }
            }
            catch (Exception ex)

            {

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
        public PlotModel CreatePlotModel( string title_series, string title_x, double max_x, string title_y, double max_y)
        {
            var model = new PlotModel { };

            // Thêm trục X
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                FontSize = 15,
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
        private void Clear()
        {
            Global.list_model.Clear();
            Init_data();
            tb_Order_Code.Text = "";
            tb_Model.Text = "";
            tb_Rotor.Text = "";
            tb_Shaft.Text = "";
            tb_BearingU.Text = "";
            tb_BearingD.Text = "";
            tb_JigU.Text = "";
            tb_JigM.Text = "";
            tb_JigD.Text = "";
            tb_Stand_Height.Text = "";
            tb_Origin_Position.Text = "";
            tb_Origin_Velocity.Text = "";
            tb_Standby_PST.Text = "";
            tb_Velocity_Standby.Text = "";
            tb_Standby_Time.Text = "";
        }





        private void Auto_RotorID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            Monitor_Order monitor_order = new Monitor_Order(); 
            monitor_order.ShowDialog();
        }
    }
}