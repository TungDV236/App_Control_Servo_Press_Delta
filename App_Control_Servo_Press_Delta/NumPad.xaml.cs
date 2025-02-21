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
using Newtonsoft.Json;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for NumPad.xaml
    /// </summary>
    public partial class NumPad : Window
    {
        private TextBox _targetTextBox;
        Common Common = new Common();
        private static bool isHandled;
        private static bool flag1;
        private static bool Button_Down = false;
        public NumPad(TextBox targetTextBox)
        {
            InitializeComponent();
            Loaded += Numpad_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Numpad_Unloaded;  // Thêm sự kiện Loaded
            _targetTextBox = targetTextBox;

        }
        private void Numpad_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.TouchDown += Button_TouchDown;
                button.MouseDown += Button_MouseDown;
                button.PreviewMouseUp += Button_MouseUp;
                button.MouseLeave += Button_MouseLeave;
                button.PreviewMouseDown += Button_MouseDown;
            }
            Global.NumPad_Visiable = true;
        }
        private void Numpad_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.TouchDown -= Button_TouchDown;
                button.MouseDown -= Button_MouseDown;
                button.PreviewMouseUp -= Button_MouseUp;
                button.MouseLeave -= Button_MouseLeave;
                button.PreviewMouseDown -= Button_MouseDown;
            }
            Global.NumPad_Visiable = false;
        }
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Button_Down)
            {
                string content = (sender as Button).Content.ToString();
                Insert_Text(content);
                e.Handled = true;
                Button_Down = true;
            }    

            //   string content = (sender as Button).Content.ToString();
            //   Insert_Text(content);
            //   e.Handled = true;
        }
        private void Button_TouchDown(object sender, TouchEventArgs e)
        {
            if (!Button_Down)
            {
                string content = (sender as Button).Content.ToString();
                Insert_Text(content);
                e.Handled = true;
                Button_Down = true;
            }
            //  MessageBox.Show("nhấn ");
        }
        private void Button_MouseUp(object sender, RoutedEventArgs e)
        {
            Button_Down = false;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button_Down = false;
        }
        private void Insert_Text(string button_content)
        {
            string content = button_content;
            if (content != "Close" && content != "Clear" && content != "Enter")
            {
                Textbox_input.Text += content;
            }
            else if (content == "Close")
            {
                this.Close(); // Đóng cửa sổ khi nhấn OK
                Global.clear_forcus = true;
            }
            else if (content == "Clear")
            {
                Textbox_input.Text = ""; // Xóa nội dung TextBox
                Global.clear_forcus = true;
            }
            else if (content == "Enter")
            {
                _targetTextBox.Text = Textbox_input.Text;
                Global.clear_forcus = true;
                this.Close(); // Đóng cửa sổ khi nhấn OK
            }

        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }

}
