using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Create_GPIO.xaml
    /// </summary>
    public partial class Create_GPIO : Window
    {
        private Button _targetButton;
        private string _Address;

        Common Common = new Common();
        public Create_GPIO(Button targetButton, string Address)
        {
            InitializeComponent();
            _targetButton = targetButton;
            _Address = Address;
            Update();
        }
        private void Update()
        {

            Name_IO_Old.Text = _targetButton.Content.ToString();
            IO_Address.Text = _Address.Insert(_Address.Length - 1, ".");
        }

        private void bt_update_Click(object sender, RoutedEventArgs e)
        {
          //  Common.Edit_IO(_targetButton.Name, Name_IO_New.Text);
            this.Close(); // Đóng cửa sổ khi nhấn OK
        }

        private void bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng cửa sổ khi nhấn OK
        }
        private void Grid_mousedown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void bt_import_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bt_export_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MouseDown_Close(object sender, RoutedEventArgs e)
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
        private void exitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btn_infor.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
    }
}
