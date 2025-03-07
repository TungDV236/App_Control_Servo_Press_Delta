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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using App_Control_Servo_Press_Delta.Class;
using Newtonsoft.Json;
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
        private string code_old;
        private bool Check_Order;
        private int count_close;
        private int check0;
        private int check1;
        private int check2;
        private int check3;
        private int check4;
        private int check5;
        private int step_scan;
        Link_Path path = new Link_Path();
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
            Global.Update_Order = false;
            Global.Done_Visiable = false;

        }
        private void Monitor_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }
            timer.Tick -= Timer_Tick;
            timer.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {


                });

                if (code_old != Order_Code.Text & Order_Code.Text != null)
                {

                    code_old = Order_Code.Text;
                    if (Order_Code.Text == "")
                    {
                        tb_Rotor.Text = "";
                        tb_Shaft.Text = "";
                    }    
                        if (Order_Code.Text != "")
                    {
                        step_scan = 1;
                    }

                }
                else if (code_old == Order_Code.Text & Order_Code.Text != null & Order_Code.Text != "" & step_scan == 1)
                {
                    check1++;
                    if (check1 == 2)
                    {
                        tb_Rotor.Focus();
                    }
                }
                if (tb_Rotor.Text != null & tb_Rotor.Text != "" & tb_Shaft.Text != null & tb_Shaft.Text != "" & check0 < 3)
                {
                    check0++;
                    if (check0 == 2)
                    {
                        tb_Rotor_Scan.Focus();
                        step_scan = 2;
                    }
                }
                if (tb_Rotor_Scan.Text != null & tb_Rotor_Scan.Text != "" & step_scan == 2)
                {
                    check2++;
                    if (check2 >= 2)
                    {
                        tb_Shaft_Scan.Focus();
                        step_scan = 3;
                    }

                }
                if (tb_Shaft_Scan.Text != null & tb_Shaft_Scan.Text != "" & step_scan == 3)
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
                    if (ID_Model.Orrder_Code == Order_Code.Text)
                    {
                        tb_Rotor.Text = ID_Model.ID_Rotor.ToString();
                        tb_Shaft.Text = ID_Model.ID_Shaft.ToString();
                        Quality.Text = ID_Model.Quality.ToString();
                        Global.Receive = false;
                        Check_Order = true;
                    }
                    else
                    {
                        Global.Receive = false;
                        step_scan = 0;
                        check1 = 0;
                        MessageBox.Show("OrderCode gửi từ Server có ID " + ID_Model.Orrder_Code + "không trùng với OrderCode Scan" + Order_Code.Text + "Vui lòng kiểm tra lại !");
                        tb_Rotor.Text = "";
                        tb_Shaft.Text = "";
                        Order_Code.Text = "";
                        Order_Code.Focus();
                    }

                }
                if (Order_Code.Text == null || Order_Code.Text == "")
                {
                    tb_Rotor_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    tb_Shaft_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    tb_BearingU_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    tb_BearingD_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                }
                if (Check_Order)
                {
                    if (tb_Rotor.Text == tb_Rotor_Scan.Text & tb_Rotor_Scan != null & tb_Rotor_Scan.Text != "")
                    {
                        tb_Rotor_Scan.Foreground = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                    }
                    else
                    {
                        tb_Rotor_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    }
                    if (tb_Shaft.Text == tb_Shaft_Scan.Text & tb_Shaft_Scan != null & tb_Shaft_Scan.Text != "")
                    {
                        tb_Shaft_Scan.Foreground = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                    }
                    else
                    {
                        tb_Shaft_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    }
                    if (tb_BearingU.Text == tb_BearingU_Scan.Text & tb_BearingU_Scan != null & tb_BearingU_Scan.Text != "")
                    {
                        tb_BearingU_Scan.Foreground = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                    }
                    else
                    {
                        tb_BearingU_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    }
                    if (tb_BearingD.Text == tb_BearingD_Scan.Text & tb_BearingD_Scan != null & tb_BearingD_Scan.Text != "")
                    {
                        tb_BearingD_Scan.Foreground = new SolidColorBrush(Color.FromRgb(5, 222, 37));
                    }
                    else
                    {
                        tb_BearingD_Scan.Foreground = new SolidColorBrush(Color.FromRgb(222, 5, 5));
                    }
                    if (ColorChecker.IsColorEqual(tb_Rotor_Scan.Foreground, Color.FromRgb(5, 222, 37)) & ColorChecker.IsColorEqual(tb_Shaft_Scan.Foreground, Color.FromRgb(5, 222, 37)) & ColorChecker.IsColorEqual(tb_BearingU_Scan.Foreground, Color.FromRgb(5, 222, 37)) & ColorChecker.IsColorEqual(tb_BearingD_Scan.Foreground, Color.FromRgb(5, 222, 37)))
                    {
                        Global.Check_done_Order = true;
                        Check_Order = false;
                    }

                }
                if (Global.Check_done_Order)
                {
                    Global.ID_Rotor = tb_Rotor_Scan.Text;
                    Global.ID_Shaft = tb_Shaft_Scan.Text;
                    Global.ID_BearingsU = tb_BearingU_Scan.Text;
                    Global.ID_BearingsD = tb_BearingD_Scan.Text;
                    Global.Order_Code = Order_Code.Text;
                    count_close++;

                    Global.Update_Order = true;

                    if (count_close >= 15 & Global.Done_Visiable)
                    {
                        Global.Fill_Done = false;
                        Global.Update_Order = false;
                        Global.Done_Visiable = false;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            // if (textboxName == "Order_Code")
            // {
            //  int caretIndex = textBox.CaretIndex;
            //  textBox.Text = textBox.Text.ToUpper();
            //  textBox.CaretIndex = caretIndex;
            // model = Model_Model.Text;
            //  }

        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (textboxName == "Order_Code" & textboxName != "" & textboxName != null)
            {
                socket.Emit_Server(Order_Code.Text);
            }
            if ((textboxName == "tb_Rotor" || textboxName == "tb_Shaft") & (tb_Rotor.Text != null & tb_Rotor.Text != "" & tb_Shaft.Text != null & tb_Shaft.Text != ""))
            {
                Fill_Value_Mode();
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
            BTN_Exit.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BTN_Exit.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
        private void MouseDown_Close(object sender, RoutedEventArgs e)
        {
            if (!Global.Write_Done)
            {
                Global.Check_Write_Model = false;
                Global.Fill_Done = false;
                Global.Write_Done = false;
                Global.Update_Order = false;
                this.Close();
            }

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
                        if ((string)obj.ID_Rotor == ID_Model.ID_Rotor.ToString() && (string)obj.ID_Shaft == ID_Model.ID_Shaft.ToString())
                        {
                            tb_BearingU.Text = obj.ID_Bearings_Up;
                            tb_BearingD.Text = obj.ID_Bearings_Down;
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        MessageBox.Show("Không có Model trùng khớp với mã trục :" + ID_Model.ID_Shaft.ToString() + ", và mã vòng bi : " + ID_Model.ID_Rotor.ToString() + " , vui lòng kiểm tra lại Model!");
                    }
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
            }
        }

    }
}
