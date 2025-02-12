using Newtonsoft.Json.Linq;     
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using IOPath = System.IO.Path;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Text.Json;
using MaterialDesignThemes.Wpf;
using App_Control_Servo_Press_Delta.Class;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using static App_Control_Servo_Press_Delta.LoginWindow;


namespace App_Control_Servo_Press_Delta
{

    public class Items_IO
    {
        public string IO_Name { get; set; }
        public string IO_Define { get; set; }
    }
    public class Items_IO_temp
    {
        public string IO_Name { get; set; }
        public string IO_Define { get; set; }
    }
    public class Items_Error
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }
    }


    public class Items_Error_temp
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }

    }
    public class Items_Err
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }

    }

    public class Items_Alarm
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }
    }


    public class Items_Alarm_temp
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }

    }
    public class Items_Al
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }

    }

    public class List_History
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }
    }
    public class List_History_temp
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
        public string Time { get; set; }
    }
    public class DataView_History
    {
        public int STT { get; set; }
        public string Code { get; set; }
        public string Content_ { get; set; }
        public string Solution { get; set; }
    }
    public class History_UL

    {

        public static bool E11;
        public static bool E12;
        public static bool E13;
        public static bool E14;
        public static bool E15;
        public static bool E16;
        public static bool E17;
        public static bool E18;
        public static bool E19;
        public static bool E1A;
        public static bool E1B;
        public static bool E1C;
        public static bool E1D;
        public static bool E1E;
        public static bool E1F;
        //
        public static bool E21;
        public static bool E22;
        public static bool E23;
        public static bool E24;
        public static bool E25;
        public static bool E26;
        public static bool E27;
        public static bool E28;
        public static bool E29;
        public static bool E2A;
        public static bool E2B;
        public static bool E2C;
        public static bool E2D;
        public static bool E2E;
        public static bool E2F;
        //
        public static bool E31;
        public static bool E32;
        public static bool E33;
        public static bool E34;
        public static bool E35;
        public static bool E36;
        public static bool E37;
        public static bool E38;
        public static bool E39;
        public static bool E3A;
        public static bool E3B;
        public static bool E3C;
        public static bool E3D;
        public static bool E3E;
        public static bool E3F;
        //
        public static bool E41;
        public static bool E42;
        public static bool E43;
        public static bool E44;
        public static bool E45;
        public static bool E46;
        public static bool E47;
        public static bool E48;
        public static bool E49;
        public static bool E4A;
        public static bool E4B;
        public static bool E4C;
        public static bool E4D;
        public static bool E4E;
        public static bool E4F;

        public static bool A11;
        public static bool A12;
        public static bool A13;
        public static bool A14;
        public static bool A15;
        public static bool A16;
        public static bool A17;
        public static bool A18;
        public static bool A19;
        public static bool A1A;
        public static bool A1B;
        public static bool A1C;
        public static bool A1D;
        public static bool A1E;
        public static bool A1F;
        //                 
        public static bool A21;
        public static bool A22;
        public static bool A23;
        public static bool A24;
        public static bool A25;
        public static bool A26;
        public static bool A27;
        public static bool A28;
        public static bool A29;
        public static bool A2A;
        public static bool A2B;
        public static bool A2C;
        public static bool A2D;
        public static bool A2E;
        public static bool A2F;
        //                 
        public static bool A31;
        public static bool A32;
        public static bool A33;
        public static bool A34;
        public static bool A35;
        public static bool A36;
        public static bool A37;
        public static bool A38;
        public static bool A39;
        public static bool A3A;
        public static bool A3B;
        public static bool A3C;
        public static bool A3D;
        public static bool A3E;
        public static bool A3F;
        //                 
        public static bool A41;
        public static bool A42;
        public static bool A43;
        public static bool A44;
        public static bool A45;
        public static bool A46;
        public static bool A47;
        public static bool A48;
        public static bool A49;
        public static bool A4A;
        public static bool A4B;
        public static bool A4C;
        public static bool A4D;
        public static bool A4E;
        public static bool A4F;








    }
}
