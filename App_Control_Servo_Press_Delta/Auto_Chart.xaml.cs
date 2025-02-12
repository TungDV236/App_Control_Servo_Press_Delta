
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
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
using OxyPlot.Wpf;
using static App_Control_Servo_Press_Delta.LoginWindow;
using App_Control_Servo_Press_Delta.Class;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Auto_Chart.xaml
    /// </summary>
    public partial class Auto_Chart : UserControl
    {
        private DispatcherTimer timer;

        // Data_Chart data_Chart = new Data_Chart();
        private LineSeries series1;
        private int pointCount = 0; // Số lượng điểm đã thêm

        public double newY1;

        private bool Flag;
        private bool Flag1;
        public Auto_Chart()
        {
            InitializeComponent();
            var model1 = CreatePlotModel("Đồ Thị Hoạt Động Sản Phẩm:" + Global.OrderCode, "Thông số", "Vị Trí (mm)", 400, "Lực Ép (N/m)", 4000);
            plotView1.Model = model1;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); // Cập nhật mỗi 100ms
            timer.Tick += Timer_Tick;
            timer.Start(); // Bắt đầu timer
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
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Global.DataPoints1 != null & Global.Start)
            {
                UpdateAxes(plotView1.Model.Series[0] as LineSeries, plotView1, Global.DataPoints1);
            }
            // Cập nhật đồ thị
            plotView1.InvalidatePlot(true);
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
    }
}
