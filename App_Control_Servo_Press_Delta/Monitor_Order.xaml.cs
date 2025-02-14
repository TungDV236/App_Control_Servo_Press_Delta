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
        private  string code_old;
        private  bool Check_Order;
        private  int count_close;
        private  int check1;
        private  int check2;
        private  int check3;
        private  int check4;
        private  int check5;
        private  int step_scan;
        Socket_client socket = new Socket_client();
        public Monitor_Order()
        {
            InitializeComponent();
            Order_Code.Focus();
            Order_Code.Clear();
            step_scan = 1;
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
            Global.Check_done_Order = false;

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
                else if (code_old == Order_Code.Text & Order_Code.Text != null & Order_Code.Text != "" & step_scan == 1)
                {
                    check1++;
                    if (check1 >= 2)
                    {
                        tb_Rotor_Scan.Focus();
                        step_scan = 2;
                    }
                }
                if (tb_Rotor_Scan.Text != null & tb_Rotor_Scan.Text != "" & step_scan==2 )
                {
                    check2++;
                    if (check2 >= 2)
                    {
                        tb_Shaft_Scan.Focus();
                        step_scan = 3;
                    }

                }
                if (tb_Shaft_Scan.Text != null & tb_Shaft_Scan.Text != "" & step_scan == 3 )
                {
                    check3++;
                    if (check3 >= 2)
                    {
                        tb_BearingU_Scan.Focus();
                        step_scan = 4;
                    }
                }
                if (tb_BearingU_Scan.Text != null & tb_BearingU_Scan.Text != "" & step_scan == 4)
                {
                    check4++;
                    if (check4 >= 2)
                    {
                        tb_BearingD_Scan.Focus();
                        step_scan = 5;
                    }
                }
                if (tb_BearingD_Scan.Text != null & tb_BearingD_Scan.Text != "" & step_scan == 5)
                {
                    check5++;
                    if (check5 >= 2)
                    {
                        FocusBorder.Focusable = true;
                        FocusBorder.Focus();
                        step_scan = 0;
                    }
                }
                if (Global.Receive)
                {
                    tb_Rotor.Text = ID_Model.ID_Rotor.ToString();
                    tb_Shaft.Text = ID_Model.ID_Shaft.ToString();
                    tb_BearingU.Text = ID_Model.ID_Bearing_Upper.ToString();
                    tb_BearingD.Text = ID_Model.ID_Bearing_Lower.ToString();
                    Global.Receive = false;
                    Check_Order = true;
                }
                if (Order_Code.Text == null || Order_Code.Text == "")
                {
                    btn_state_ID_Rotor.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    btn_state_ID_Rotor.Content = "NG";
                    btn_state_ID_Shaft.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    btn_state_ID_Shaft.Content = "NG";
                    btn_state_BearingU.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    btn_state_BearingU.Content = "NG";
                    btn_state_BearingD.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    btn_state_BearingD.Content = "NG";
                }
                if (Check_Order)
                {
                    if (tb_Rotor.Text == tb_Rotor_Scan.Text & tb_Rotor_Scan != null & tb_Rotor_Scan.Text != "")
                    {
                        btn_state_ID_Rotor.Background = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                        btn_state_ID_Rotor.Content = "OK";
                    }
                    else
                    {
                        btn_state_ID_Rotor.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                        btn_state_ID_Rotor.Content = "NG";
                    }
                    if (tb_Shaft.Text == tb_Shaft_Scan.Text & tb_Shaft_Scan != null & tb_Shaft_Scan.Text != "")
                    {
                       btn_state_ID_Shaft.Background = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                        btn_state_ID_Shaft.Content = "OK";
                    }
                    else
                    {
                        btn_state_ID_Shaft.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                        btn_state_ID_Shaft.Content = "NG";
                    }
                    if (tb_BearingU.Text == tb_BearingU_Scan.Text & tb_BearingU_Scan != null & tb_BearingU_Scan.Text != "")
                    {
                        btn_state_BearingU.Background = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                        btn_state_BearingU.Content = "OK";
                    }
                    else
                    {
                        btn_state_BearingU.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                        btn_state_BearingU.Content = "NG";
                    }
                    if (tb_BearingD.Text == tb_BearingD_Scan.Text & tb_BearingD_Scan != null & tb_BearingD_Scan.Text != "")
                    {
                        btn_state_BearingD.Background = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                        btn_state_BearingD.Content = "OK";
                    }
                    else
                    {
                        btn_state_BearingD.Background = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                        btn_state_BearingD.Content = "NG";
                    }
                    if (btn_state_BearingD.Content.ToString() == "OK" & btn_state_ID_Rotor.Content.ToString() == "OK" & btn_state_ID_Shaft.Content.ToString() == "OK" & btn_state_BearingU.Content.ToString() == "OK")
                    {
                        Global.Check_done_Order = true;
                        Check_Order = false;
                    }    

                }
                if (Global.Check_done_Order)
                {
                    count_close++;
                    if (count_close >= 15)
                    {


                        this.Close();
                    }
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
            if (textboxName == "Order_Code" & textboxName!=""& textboxName!=null)
            {
                socket.Emit_Server(Order_Code.Text);
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
