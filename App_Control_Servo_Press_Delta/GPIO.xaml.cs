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
            //
            ushort word1 = Data.Error1;
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
