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
using System.Windows.Shapes;
using App_Control_Servo_Press_Delta.Class;

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
using ViewModel;

namespace App_Control_Servo_Press_Delta.Popup
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart_Report : Window
    {
        public Chart_Report()
        {
            InitializeComponent();
            Loaded += Chart_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Chart_Unloaded;
            DataContext = new MainWindow_VM();
        }
        private void Chart_Loaded(object sender, RoutedEventArgs e)
        {
            tb_Title.Text = Global.Language ==  "EN" ? "Product Activity Graph :" + Global.Order_Code_Report : "Đồ thị hoạt động sản phẩm:"  + Global.Order_Code_Report;
            var model1 = CreatePlotModel(Global.Language == "EN" ? "Parameter" : "Thông số", Global.Language == "EN" ? "Position (mm)" : "Vị Trí (mm)", 400, Global.Language == "EN" ? "Force (N)" : "Lực Ép (N)", 4000);

            plotView1.Model = model1;
            UpdateAxes(plotView1.Model.Series[0] as LineSeries, Global.DataPoints_Chart);
            plotView1.InvalidatePlot(true);

        }
        private void Chart_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        public PlotModel CreatePlotModel(string title_series, string title_x, double max_x, string title_y, double max_y)
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
        public void UpdateAxes(LineSeries series, List<DataPoint> dataPoint)
        {
            series.Points.Clear();
            series.Points.AddRange(dataPoint);
        }

    }
}
