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
using System.Windows.Shapes;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Infor.xaml
    /// </summary>
    public partial class Infor : Window
    {
        public Infor()
        {
            InitializeComponent();
        }

        private void infor_exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Website_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Mở trang web trong trình duyệt mặc định
            var url = "https://stivietnam.com";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
