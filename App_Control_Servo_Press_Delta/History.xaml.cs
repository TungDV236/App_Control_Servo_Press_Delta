using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : UserControl
    {
        public List<string> ChangedBitsLog { get; } = new List<string>();
        Link_Path linkpath = new Link_Path();
        private static ushort _value;
        private DispatcherTimer timer;
        History_Error Error_Screen = new History_Error();
        History_Alarm Alarm_Screen = new History_Alarm();
        //History_Config List_History_Screen = new History_Config();
        private int NoScreen;
        Update_Screen update = new Update_Screen();
        public History()
        {
            InitializeComponent();
            Loaded += History_Loaded;  // Thêm sự kiện Loaded
        }
        private void History_Loaded(object sender, RoutedEventArgs e)
        {
            Pannel_History.Children.Clear();
            Pannel_History.Children.Add(Error_Screen);
            NoScreen = 1;
            // Main();
        }


        private void Clicked_GPIO_Back(object sender, RoutedEventArgs e)
        {


            if (NoScreen <= 1)
            {
                NoScreen = 2;
            }
            else
            {
                NoScreen -= 1;
            }

            Visiable(NoScreen);
        }
        private void Clicked_GPIO_Next(object sender, RoutedEventArgs e)
        {

            if (MainWindow.UserName == "STI-Technical")
            {
                NoScreen = 3;
            }

            if (NoScreen >= 2)
            {
                NoScreen = 1;
            }
            else
            {
                NoScreen += 1;
            }
            Visiable(NoScreen);
        }
        private void Visiable(int input)
        {
            int UpScreen_show = input;

            switch (UpScreen_show)
            {
                case 1:

                    Pannel_History.Children.Clear();
                    Pannel_History.Children.Add(Error_Screen);
                    break;
                case 2:
                    Pannel_History.Children.Clear();
                    Pannel_History.Children.Add(Alarm_Screen);
                    break;
                default:
                    break;
            }

        }



    }
}
