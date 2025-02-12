using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using App_Control_Servo_Press_Delta.Class;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Auto_Step.xaml
    /// </summary>
    public partial class Auto_Step : UserControl
    {

        private static int _Process;
        public Auto_Step()
        {
            InitializeComponent();
        }
        private void Update_Data()
        {
            switch (Data.Process)
            {
                case 0:
                    bt_Step0.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step1.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step2.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step3.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step4.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    break;
                case 1:
                    bt_Step0.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    bt_Step1.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step2.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step3.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step4.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    break;
                case 2:
                    bt_Step0.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step1.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    bt_Step2.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step3.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step4.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    break;
                case 3:
                    bt_Step0.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step1.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step2.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    bt_Step3.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step4.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    break;
                case 4:
                    bt_Step0.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step1.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step2.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step3.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    bt_Step4.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    break;
                case 5:
                    bt_Step0.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step1.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step2.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step3.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    bt_Step4.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    break;
                default:
                    break;

            }
            _Process = Data.Process;

        }
    }

}
