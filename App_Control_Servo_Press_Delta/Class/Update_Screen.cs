using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace App_Control_Servo_Press_Delta.Class
{
    public class Update_Screen
    {
        public void bt_Green(Button button, bool Status)
        {
            if (Status)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            }
            else
            {
                button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
        public void Inout(Button button, bool Status)
        {
            if (Status)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
        public void Lable(Label lable, bool Status)
        {
            if (Status)
            {
                lable.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                lable.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
        public void bt_Blue(Button button, bool Status, bool Reverse)
        {
            if (Reverse == false)
            {
                if (Status == true)
                {
                    button.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                }
                else
                {
                    button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
            }
            else
            {
                if (Status == false)
                {
                    button.Background = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                }
                else
                {
                    button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
            }
        }
    }
}
