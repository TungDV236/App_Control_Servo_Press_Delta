
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
        private static bool Check_Log = false;
        private static bool Fill_Log = false;
        private static int check;
        PLC plc = new PLC();
        
        public Auto()
        {
            InitializeComponent();
            Loaded += Auto_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Auto_Unloaded;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void Auto_Loaded(object sender, RoutedEventArgs e)
        {

                var model1 = CreatePlotModel(Global.Language=="EN" ? "Parameter" : "Thông số", Global.Language == "EN" ? "Position (mm)" : "Vị Trí (mm)", 400, Global.Language == "EN" ? "Force (N)" :  "Lực Ép (N)", 4000);
 
            plotView1.Model = model1;
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }
            timer.Tick += Timer_Tick;
            timer.Start();
            Init_data();
        }
        private void Auto_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.LostFocus -= TextBox_LostFocus;
            }
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
                    Global.Auto_Order_Code = tb_Order_Code.Text;
                    if (Global.Clear_Auto)
                    {
                        Clear();
                        Global.Clear_Auto=false;
                    }    

                });
                if (Global.DataPoints1 != null & Global.Pressing)
                {
                    UpdateAxes(plotView1.Model.Series[0] as LineSeries, plotView1, Global.DataPoints1);
                }
                // Cập nhật đồ thị
                plotView1.InvalidatePlot(true);
            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
            }
        }

        private void Update_Screen()
        {
            if (Global.Update_Order & !Global.Fill_Done)
            {

                Fill_Value_Mode();
                Global.Fill_Done = true;
            }
            if (Global.Write_Done  & !Check_Log)
            {

                Visiable_Order();
                Global.Write_Done = false;
                Check_Log = true;

            }
            if (!Global.Check_done_Order & Check_Log)
            {
                Check_Log = false;

            }
            if (Data.Product_OK )
            {
                lb_Status.Background = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                lb_Status.Content = "OK";
                lb_Status.Visibility = Visibility.Visible;
                tb_Auto_Pressed_Force_Max.Text = Global.Force_Max.ToString();
                tb_Auto_Position_Force_Max.Text = Global.Position_Force_Max.ToString();
            }
            else if (Data.Product_NG)
            {
                lb_Status.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                lb_Status.Content = "NG";
                lb_Status.Visibility = Visibility.Visible;
                tb_Auto_Pressed_Force_Max.Text = Global.Force_Max.ToString();
                tb_Auto_Position_Force_Max.Text = Global.Position_Force_Max.ToString();
            }
            else
            {
                lb_Status.Visibility = Visibility.Hidden;
                tb_Auto_Pressed_Force_Max.Text = "0";
                tb_Auto_Position_Force_Max.Text ="0";
            }    
            tb_Auto_NG.Text = Data.Total_NG.ToString();
            tb_Auto_Pass.Text = Data.Total_OK.ToString();
            tb_Auto_Total.Text = (Data.Total_NG + Data.Total_OK ).ToString();
            if (!Global.Pressing)
            {
                tb_Auto_Pressed_Force_Max.Text = Global.Force_Max.ToString();
                tb_Auto_Pressed_Force_Max.Text = Global.Force_Max.ToString();
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
            
            Global.Auto_Thickness_Bearings_D = 0;
            Global.Auto_Thickness_Bearings_U = 0;
            Global.Auto_Thickness_Jig_Up = 0;
            Global.Auto_Thickness_Jig_Down = 0;
            Global.Auto_Press_Pos1 = 0;
            Global.Auto_Press_Pos2 = 0;
            Global.Data_Auto_FC1 = new List<DataFunC>
           {
                new DataFunC
                {
                    Mode = 0,
                    Press_Condition = "",
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
                    Press_Force = 0,
                    Press_Vel = 0,
                    Press_Time = 0,
                    End_Max_Force_Limit = 0,
                    End_Min_Force_Limit = 0,
                    End_Max_Pos_Limit = 0,
                    End_Min_Pos_Limit = 0
                }
             };
            Global.Data_FC_temp = new List<DataFunC>
           {
                new DataFunC
                {
                    Mode = 0,
                    Press_Condition = "",
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
                        if ((string)obj.ID_Rotor == Global.ID_Rotor && (string)obj.ID_Shaft == Global.ID_Shaft)
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
                                Global.Auto_Thickness_Jig_Up = Fill_Bearings_JigUD(path.Jig_Up, obj.Jig_Up);
                                Global.Auto_Thickness_Jig_Down = Fill_Bearings_JigUD(path.Jig_Down, obj.Jig_Down);
                                Global.Auto_Thickness_Bearings_U = Fill_Bearings_JigUD(path.Bearings_Up, obj.ID_Bearings_Up);
                                Global.Auto_Thickness_Bearings_D = Fill_Bearings_JigUD(path.Bearings_Down, obj.ID_Bearings_Down);
                                Global.Auto_Ofset_Model = obj.Ofset_position1;
                                Global.Auto_Pre_press_Bearings_distance = obj.Pre_press_Bearings_distance;
                                Global.Auto_After_press_bearings_distance = obj.After_press_bearings_distance;
                                Global.list_model[0].Origin_Position = obj.Origin_Position;
                                Global.list_model[0].Origin_Velocity = obj.Origin_Velocity;
                                Global.list_model[0].Standby_Position = obj.Standby_Position;
                                Global.list_model[0].Standby_Time = obj.Standby_Time;
                                Global.list_model[0].Standby_Velocity = obj.Standby_Velocity;
                                Global.list_model[0].Data_Func1.Clear();
                                Global.list_model[0].Data_Func2.Clear();
                                Global.list_model[0].Data_Func1.AddRange(obj.Data_Func1);
                                Global.list_model[0].Data_Func2.AddRange(obj.Data_Func2); 
                            Global.Auto_Press_Pos1 = Caculate_Position_Distance(Global.list_model[0].Data_Func1[0].Mode, Global.Auto_Thickness_Bearings_D.ToString(), Global.Auto_After_press_bearings_distance.ToString(),
                                                       Global.Auto_Thickness_Bearings_U.ToString(), Global.Auto_Pre_press_Bearings_distance.ToString(), Global.Auto_Ofset_Model.ToString(), Global.list_model[0].Standby_Position.ToString());
                            Global.Auto_Press_Pos2 = Caculate_Position_Distance(Global.list_model[0].Data_Func2[0].Mode, Global.Auto_Thickness_Bearings_D.ToString(), Global.Auto_After_press_bearings_distance.ToString(),
                                                       Global.Auto_Thickness_Bearings_U.ToString(), Global.Auto_Pre_press_Bearings_distance.ToString(), Global.Auto_Ofset_Model.ToString(), Global.list_model[0].Standby_Position.ToString());

                            var data = new
                            {
                                Origin_Work_Pos = Global.list_model[0].Origin_Position,
                                Origin_Work_Vel = Global.list_model[0].Origin_Velocity,
                                Standby_Pos = Global.list_model[0].Standby_Position,
                                Standby_Vel = Global.list_model[0].Standby_Velocity,
                                Standby_Time = Global.list_model[0].Standby_Time,
                                    Mode1 = Global.list_model[0].Data_Func1[0].Mode,
                                    Press_Pos1 = Global.Auto_Press_Pos1,
                                    Press_Force1 = Global.list_model[0].Data_Func1[0].Press_Force,
                                    Press_Vel1 = Global.list_model[0].Data_Func1[0].Press_Vel,
                                    Press_Time1 = Global.list_model[0].Data_Func1[0].Press_Time,
                                    End_Max_Force_Limit1 = Global.list_model[0].Data_Func1[0].End_Max_Force_Limit,
                                    End_Min_Force_Limit1 = Global.list_model[0].Data_Func1[0].End_Min_Force_Limit,
                                    End_Max_Pos_Limit1 = Global.list_model[0].Data_Func1[0].End_Max_Pos_Limit,
                                    End_Min_Pos_Limit1 = Global.list_model[0].Data_Func1[0].End_Min_Pos_Limit,
                                    Mode2 = Global.list_model[0].Data_Func2[0].Mode,
                                    Press_Pos2 = Global.Auto_Press_Pos2,
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
                                Global.Check_Write_Model = true;
                            Global.Count_check = 0;



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

                Common.Log_err( ex.ToString());
            }
        }
        private void Visiable_Order ()
        {

            tb_Order_Code.Text = Global.Order_Code;
            tb_Model.Text = Global.list_model[0].Model;
            tb_Rotor.Text = Global.list_model[0].ID_Rotor;
            tb_Shaft.Text = Global.list_model[0].ID_Shaft;
            tb_BearingU.Text = Global.list_model[0].ID_Bearings_Up;
            tb_BearingD.Text = Global.list_model[0].ID_Bearings_Down;
            tb_JigU.Text = Global.list_model[0].Jig_Up;
            tb_JigM.Text = Global.list_model[0].Jig_Mid;
            tb_JigD.Text = Global.list_model[0].Jig_Down;

            tb_Stand_Height.Text = string.Format("{0:F2}", Global.list_model[0].Height_Stand);
            tb_Origin_Position.Text = string.Format("{0:F2}", Global.list_model[0].Origin_Position);
            tb_Origin_Velocity.Text = string.Format("{0:F2}", Global.list_model[0].Origin_Velocity);
            tb_Standby_PST.Text = string.Format("{0:F2}", Global.list_model[0].Standby_Position);
            tb_Velocity_Standby.Text = string.Format("{0:F2}", Global.list_model[0].Origin_Velocity);
            tb_Standby_Time.Text = string.Format("{0:F2}", Global.list_model[0].Standby_Time);
            tb_Pressing_condition1.Text = Global.list_model[0].Data_Func1[0].Press_Condition.ToString();
            tb_Pressing_Position1.Text = Global.Auto_Press_Pos1.ToString();
            tb_Pressing_Force1.Text = Global.list_model[0].Data_Func1[0].Press_Force.ToString();
            tb_Pressing_Velocity1.Text = Global.list_model[0].Data_Func1[0].Press_Vel.ToString();
            tb_Pressing_Time1.Text = Global.list_model[0].Data_Func1[0].Press_Time.ToString();
            tb_Max_Force1.Text = Global.list_model[0].Data_Func1[0].End_Max_Force_Limit.ToString();
            tb_Min_Force1.Text = Global.list_model[0].Data_Func1[0].End_Min_Force_Limit.ToString();
            tb_Max_Position1.Text = Global.list_model[0].Data_Func1[0].End_Max_Pos_Limit.ToString();
            tb_Min_Position1.Text = Global.list_model[0].Data_Func1[0].End_Min_Pos_Limit.ToString();

            tb_Pressing_condition2.Text = Global.list_model[0].Data_Func2[0].Press_Condition.ToString();
            tb_Pressing_Position2.Text = Global.Auto_Press_Pos2.ToString(); 
            tb_Pressing_Force2.Text = Global.list_model[0].Data_Func2[0].Press_Force.ToString();
            tb_Pressing_Velocity2.Text = Global.list_model[0].Data_Func2[0].Press_Vel.ToString();
            tb_Pressing_Time2.Text = Global.list_model[0].Data_Func2[0].Press_Time.ToString();
            tb_Max_Force2.Text = Global.list_model[0].Data_Func2[0].End_Max_Force_Limit.ToString();
            tb_Min_Force2.Text = Global.list_model[0].Data_Func2[0].End_Min_Force_Limit.ToString();
            tb_Max_Position2.Text = Global.list_model[0].Data_Func2[0].End_Max_Pos_Limit.ToString();
            tb_Min_Position2.Text = Global.list_model[0].Data_Func2[0].End_Min_Pos_Limit.ToString();
            Global.Order_Code_Write_done = tb_Order_Code.Text;
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            List<Data_Log> data_Logs = new List<Data_Log>
            {
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Scan_Order_Code:   " + Global.Order_Code , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Stand_Height:   " + Global.list_model[0].Height_Stand , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Origin_Position:   " + Global.list_model[0].Origin_Position, Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Origin_Velocity:   " + Global.list_model[0].Origin_Velocity , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Standby_Position:   " + Global.list_model[0].Standby_Position , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Standby_Velocity:   " + Global.list_model[0].Standby_Velocity , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Standby_Time:   " + Global.list_model[0].Standby_Time , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Mode1:   " + Global.list_model[0].Data_Func1[0].Mode , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Position1:   " + Global.Auto_Press_Pos1 , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Force1:   " + Global.list_model[0].Data_Func1[0].Press_Force , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Velocity1:   " + Global.list_model[0].Data_Func1[0].Press_Vel , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Time1:   " + Global.list_model[0].Data_Func1[0].Press_Time , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Max_Force1:   " + Global.list_model[0].Data_Func1[0].End_Max_Force_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Min_Force1:   " + Global.list_model[0].Data_Func1[0].End_Min_Force_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Max_Position1:   " + Global.list_model[0].Data_Func1[0].End_Max_Pos_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Min_Position1:   " + Global.list_model[0].Data_Func1[0].End_Min_Pos_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Mode2:   " + Global.list_model[0].Data_Func2[0].Mode , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Position2:   " + Global.Auto_Press_Pos2 , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Force2:   " + Global.list_model[0].Data_Func2[0].Press_Force , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Velocity2:   " + Global.list_model[0].Data_Func2[0].Press_Vel , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Press_Time2:   " + Global.list_model[0].Data_Func2[0].Press_Time , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Max_Force2:   " + Global.list_model[0].Data_Func2[0].End_Max_Force_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Min_Force2:   " + Global.list_model[0].Data_Func2[0].End_Min_Force_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Max_Position2:   " + Global.list_model[0].Data_Func2[0].End_Max_Pos_Limit , Time = formattedDate +" "+formattedtime},
                new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Write Min_Position2:   " + Global.list_model[0].Data_Func2[0].End_Min_Pos_Limit , Time = formattedDate +" "+formattedtime}
            };

            // Chuyển danh sách sang định dạng JSON
            string json_Log = JsonConvert.SerializeObject(data_Logs, Formatting.Indented);
            Common.Log_Operation_Json(json_Log, path.Log);
            Global.Done_Visiable = true;
        }


        public PlotModel CreatePlotModel( string title_series, string title_x, double max_x, string title_y, double max_y)
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
            tb_Pressing_condition1.Text = "";
            tb_Pressing_Position1.Text = "";
            tb_Pressing_Force1.Text = "";
            tb_Pressing_Velocity1.Text = "";
            tb_Pressing_Time1.Text = "";
            tb_Max_Position1.Text = "";
            tb_Min_Position1.Text = "";
            tb_Max_Force1.Text = "";
            tb_Min_Force1.Text = "";
            tb_Pressing_condition2.Text = "";
            tb_Pressing_Position2.Text = "";
            tb_Pressing_Force2.Text = "";
            tb_Pressing_Velocity2.Text = "";
            tb_Pressing_Time2.Text = "";
            tb_Max_Position2.Text = "";
            tb_Min_Position2.Text = "";
            tb_Max_Force2.Text = "";
            tb_Min_Force2.Text = "";
            Global.Order_Code_Write_done = "";
            Global.Order_Code = "";
        }
        private static float Caculate_Position_Distance(float mode, string Thickness_BearingsD, string Distance_Bearings_After, string Thickness_BearingsU, string Distance_Bearings_Before, string ofset_Model, string standby_position)
        {

            float Position = Global.Height_Shaft_Press
                + (float)Data.ofset_Machine
                - (float)Data.Height_Jig_Base
                - Global.Auto_Thickness_Jig_Down
                - float.Parse(Thickness_BearingsD)
                - float.Parse(Distance_Bearings_After)
                - float.Parse(Thickness_BearingsU)
                - Global.Auto_Thickness_Jig_Up
                + float.Parse(ofset_Model);
            float Distance = Global.Height_Shaft_Press
                + (float)Data.ofset_Machine
                - (float)Data.Height_Jig_Base
                - Global.Model_Thickness_Jig_Down
                - float.Parse(Thickness_BearingsD)
                - float.Parse(Distance_Bearings_After)
                - float.Parse(Thickness_BearingsU)
                - Global.Model_Thickness_Jig_Up
                + float.Parse(ofset_Model)
                - float.Parse(standby_position);
            Global.Standby_Position = Global.Height_Shaft_Press
                + (float)Data.ofset_Machine
                - (float)Data.Height_Jig_Base
                - Global.Model_Thickness_Jig_Down
                - float.Parse(Thickness_BearingsD)
                - float.Parse(Distance_Bearings_Before)
                - float.Parse(Thickness_BearingsU)
                - Global.Model_Thickness_Jig_Up
                + float.Parse(ofset_Model);

            switch (mode)
            {

                case 1:
                    return Position;
                case 2:
                    return 0;
                case 3:
                    return Distance;
                case 4:
                    return Position;
                case 5:
                    return Position;

            }
            return 0;
        }


        private static float Fill_Bearings_JigUD(string path, string id)
        {
            try
            {
                string jsons = File.ReadAllText(path); ;
                if (jsons.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(jsons);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["ID"] == id)
                        {
                            return (float)obj["Thickness"];
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return -1;
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