using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for GPIO.xaml
    /// </summary>
    public partial class GPIO : UserControl
    {
        private DispatcherTimer timer;
        Update_Screen update = new Update_Screen();
        Common Common = new Common();
        Excel excel = new Excel();
        bool ud = false;
        public GPIO()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
            Update_screen();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            try
            {
                Dispatcher.Invoke(() =>
                {
                    Update_IO();
                    /*  Debug.Content = Data.IB0.ToString()
                        + "/" + Data.IB1.ToString()
                        + "/" + Data.IB2.ToString()
                        + "/" + Data.IB3.ToString()
                        + "/" + Data.IB4.ToString()
                        + "/" + Data.IB5.ToString()
                        + "/" + Data.IB6.ToString()
                        + "/" + Data.IB7.ToString()
                        + "/" + Data.QB0.ToString()
                        + "/" + Data.QB1.ToString()
                        + "/" + Data.QB2.ToString()
                        + "/" + Data.QB3.ToString()
                        + "/" + Data.QB4.ToString()
                        + "/" + Data.QB5.ToString()
                        + "/" + Data.QB6.ToString()
                        + "/" + Data.QB7.ToString(); */
                    if (!ud)
                    {
                        Update_screen();

                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        void Update_IO()
        {
            bool[] I0 = ByteToBits(Data.IB0);
            update.Inout(I0_0, I0[0]);
            update.Inout(I0_1, I0[1]);
            update.Inout(I0_2, I0[2]);
            update.Inout(I0_3, I0[3]);
            update.Inout(I0_4, I0[4]);
            update.Inout(I0_5, I0[5]);
            update.Inout(I0_6, I0[6]);
            update.Inout(I0_7, I0[7]);
            //
            bool[] X0 = ByteToBits(Data.XB0);
            update.Inout(X0_0, X0[0]);
            update.Inout(X0_1, X0[1]);
            update.Inout(X0_2, X0[2]);
            update.Inout(X0_3, X0[3]);
            update.Inout(X0_4, X0[4]);
            update.Inout(X0_5, X0[5]);
            update.Inout(X0_6, X0[6]);
            update.Inout(X0_7, X0[7]);
            //
            bool[] X1 = ByteToBits(Data.XB1);
            update.Inout(X1_0, X1[0]);
            update.Inout(X1_1, X1[1]);
            update.Inout(X1_2, X1[2]);
            update.Inout(X1_3, X1[3]);
            update.Inout(X1_4, X1[4]);
            update.Inout(X1_5, X1[5]);
            update.Inout(X1_6, X1[6]);
            update.Inout(X1_7, X1[7]);
            //
            bool[] X2 = ByteToBits(Data.XB2);
            update.Inout(X2_0, X2[0]);
            update.Inout(X2_1, X2[1]);
            update.Inout(X2_2, X2[2]);
            update.Inout(X2_3, X2[3]);
            update.Inout(X2_4, X2[4]);
            update.Inout(X2_5, X2[5]);
            update.Inout(X2_6, X2[6]);
            update.Inout(X2_7, X2[7]);
            //
            //
            bool[] Q0 = ByteToBits(Data.QB0);
            update.Inout(Q0_0, Q0[0]);
            update.Inout(Q0_1, Q0[1]);
            update.Inout(Q0_2, Q0[2]);
            update.Inout(Q0_3, Q0[3]);
            update.Inout(Q0_4, Q0[4]);
            update.Inout(Q0_5, Q0[5]);
            update.Inout(Q0_6, Q0[6]);
            update.Inout(Q0_7, Q0[7]);
            //
            bool[] Y0 = ByteToBits(Data.YB0);
            update.Inout(Y0_0, Y0[0]);
            update.Inout(Y0_1, Y0[1]);
            update.Inout(Y0_2, Y0[2]);
            update.Inout(Y0_3, Y0[3]);
            update.Inout(Y0_4, Y0[4]);
            update.Inout(Y0_5, Y0[5]);
            update.Inout(Y0_6, Y0[6]);
            update.Inout(Y0_7, Y0[7]);
            //
            bool[] Y1 = ByteToBits(Data.YB1);
            update.Inout(Y1_0, Y1[0]);
            update.Inout(Y1_1, Y1[1]);
            update.Inout(Y1_2, Y1[2]);
            update.Inout(Y1_3, Y1[3]);
            update.Inout(Y1_4, Y1[4]);
            update.Inout(Y1_5, Y1[5]);
            update.Inout(Y1_6, Y1[6]);
            update.Inout(Y1_7, Y1[7]);
            //
            bool[] Y2 = ByteToBits(Data.YB2);
            update.Inout(Y2_0, Y2[0]);
            update.Inout(Y2_1, Y2[1]);
            update.Inout(Y2_2, Y2[2]);
            update.Inout(Y2_3, Y2[3]);
            update.Inout(Y2_4, Y2[4]);
            update.Inout(Y2_5, Y2[5]);
            update.Inout(Y2_6, Y2[6]);
            update.Inout(Y2_7, Y2[7]);
            //
        }


        static bool[] ByteToBits(uint value)
        {
            bool[] bits = new bool[10];

            for (int i = 0; i < 8; i++)
            {
                bits[i] = (value & (1 << i)) != 0;
            }
            return bits;
        }

        void Update_screen()
        {

            var buttons = Common.FindVisualChildren2<Button>(this);
            foreach (var button in buttons)
            {
                if (button.Name.StartsWith("GP"))
                {
                    // Thực hiện hành động với nút nhấn
                    //  button.Background = new SolidColorBrush(Colors.LightGreen);
                    button.Content = Common.Search_IO(button.Name);
                    ud = true;
                }
            }
        }
        private void Edit_IO(object sender, RoutedEventArgs e)
        {

            if (MainWindow.UserName == "STI-Technical")
            {
                Button button = sender as Button;
                string BTN_name = (sender as Button).Name;
                Create_GPIO Create_GPIO = new Create_GPIO(button, button.Name.Substring(2));
                Create_GPIO.ShowDialog(); // Hiển thị cửa sổ như hộp thoại
            }
            else MessageBox.Show("Vui lòng đăng nhập tài khoản STI-Technical để cấu hình");
            Update_screen();
        }


    }
}
