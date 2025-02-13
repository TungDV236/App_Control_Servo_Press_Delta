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
using System.Windows.Threading;
using App_Control_Servo_Press_Delta.Class;
using OxyPlot.Series;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Monitor_Order.xaml
    /// </summary>
    public partial class Monitor_Order : Window
    {

        private DispatcherTimer timer;
        Common Common = new Common();
        private static string code_old;
        public Monitor_Order()
        {
            InitializeComponent();
            Order_Code.Focus();
            Order_Code.Clear();
            Loaded += Monitor_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Monitor_Unloaded;
        }
        private void Monitor_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
            timer.Start();

        }
        private void Monitor_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                  

                });
                if (code_old != Order_Code.Text & Order_Code.Text != null & Order_Code.Text!= "")
                {
                    code_old = Order_Code.Text;
                }
                else if (code_old == Order_Code.Text & Order_Code.Text != null & Order_Code.Text != "")
                {

                    FocusBorder.Focusable = true;
                    FocusBorder.Focus();
                }

                }
            catch
            {
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (textboxName == "TB_Model")
            {
               
                // model = Model_Model.Text;
            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (textboxName == "TB_Model")
            {

                // model = Model_Model.Text;
            }
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void exitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_exit.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_exit.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
        private void MouseDown_Close(object sender, RoutedEventArgs e)
        {
            this.Close();

        }


    }
}
