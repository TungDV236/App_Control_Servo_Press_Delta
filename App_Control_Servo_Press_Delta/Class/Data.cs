using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace App_Control_Servo_Press_Delta.Class
{
    public class Global
    {

        public static List<Position> List_Position_all { get; set; }
        public static List<DataView_Report> List_report { get; set; }
        public static List<Data_Report_all> List_report_all { get; set; }
        public static List<DataView_Report> List_report_temp { get; set; }
        public static List<DataPoint> DataPoints1 { get; set; } // Lưu trữ điểm cho biểu đồ 1
        public static bool Start { get; set; }
        public static bool Receive { get; set; }
        public static bool Check_done_Order { get; set; }
        public static bool Update_Order { get; set; }
        public static bool clear_forcus { get; set; }
        public static string OrderCode { get; set; }
        public static string Language { get; set; }
        public static List<DataFunC> Function1 { get; set; }
        public static List<DataFunC> Function2 { get; set; }

        public static List<DataFunC> Data_Auto_FC1 { get; set; }
        public static List<DataFunC> Data_Auto_FC2 { get; set; }
        public static List<List_Model> list_model { get; set; }

        public static string Server  { get; set; }
        public static string PLC_IP { get; set; }
        public static string Order_Code { get; set; }
        public static string ID_Rotor { get; set; }
        public static string ID_Shaft { get; set; }
        public static string ID_BearingsU { get; set; }
        public static string ID_BearingsD { get; set; }
        public static string Model { get; set; }
        public static double Force_Max { get; set; }
        public static double Position_Force_Max { get; set; }
        public static bool Pressing { get; set; }
        public static bool Check_Write_Model { get; set; }
        public static bool Write_Done  { get; set; }
    }
    public class Data_Report
    {
        public int STT { get; set; }
        public string Time { get; set; }
        public string OrderCode { get; set; }
        public string Model { get; set; }
        public string TrucID { get; set; }
        public string RotorID { get; set; }
        public string Beer_Up { get; set; }
        public string Beer_Down { get; set; }
        public string Force_Max { get; set; }
        public string Status { get; set; }
    }
    public class DataView_Report
    {
        public int STT { get; set; }
        public string Time { get; set; }
        public string Model { get; set; }
        public string TrucID { get; set; }
        public string RotorID { get; set; }
        public string Force { get; set; }
        public string Force_Max { get; set; }
    }

    public class Data_Report_temp
    {
        public static int STT { get; set; }
        public static string Time { get; set; }
        public static string OrderCode { get; set; }
        public static string Model { get; set; }
        public static string TrucID { get; set; }
        public static string RotorID { get; set; }
        public static string Beer_Up { get; set; }
        public static string Beer_Down { get; set; }
        public static string Jig_Up { get; set; }
        public static string Jig_Mid { get; set; }
        public static string Jig_Down { get; set; }
        public static string HStand { get; set; }
        public static string Force { get; set; }
        public static string Force_Max { get; set; }
    }
    public class Data_Report_temp2
    {
        public static int STT { get; set; }
        public static string Time { get; set; }
        public static string OrderCode { get; set; }
        public static string Model { get; set; }
        public static string TrucID { get; set; }
        public static string RotorID { get; set; }
        public static string Beer_Up { get; set; }
        public static string Beer_Down { get; set; }
        public static string Jig_Up { get; set; }
        public static string Jig_Mid { get; set; }
        public static string Jig_Down { get; set; }
        public static string HStand { get; set; }
        public static string Force { get; set; }
        public static string Force_Max { get; set; }
    }
    public class Data_Report_all
    {
        public int STT { get; set; }
        public string Time { get; set; }
        public string OrderCode { get; set; }
        public string Model { get; set; }
        public string TrucID { get; set; }
        public string RotorID { get; set; }
        public string Beer_Up { get; set; }
        public string Beer_Down { get; set; }
        public string Jig_Up { get; set; }
        public string Jig_Mid { get; set; }
        public string Jig_Down { get; set; }
        public string HStand { get; set; }
        public string Force { get; set; }
        public string Force_Max { get; set; }
        public string Position { get; set; }
    }
    public class Position
    {

        public float PST;
        public float Force;
        public Position(float pts, float force)
        {
            PST = pts;
            Force = force;
        }
    }
    public class Data
    {
        //Input

        public static uint IB0 { get; set; }
        public static uint XB0 { get; set; }
        public static uint XB1 { get; set; }
        public static uint XB2 { get; set; }
        //Output
        public static uint QB0 { get; set; }
        public static uint YB0 { get; set; }
        public static uint YB1 { get; set; }
        public static uint YB2 { get; set; }
        public static double Jog_Max_Force { get; set; }
        public static double Jog_Distance_ABS { get; set; }
        public static double Jog_Vel { get; set; }
        public static double Go_Home_Vel { get; set; }
        public static double Origin_Work_Pos { get; set; }
        public static double Origin_Work_Vel { get; set; }
        public static double Standby_Pos { get; set; }
        public static double Standby_Vel { get; set; }
        public static double Standby_Time { get; set; }
        public static double Mode1 { get; set; }
        public static double Press_Pos1 { get; set; }
        public static double Press_Force1 { get; set; }
        public static double Press_Vel1 { get; set; }
        public static double Press_Time1 { get; set; }
        public static double End_Max_Force_Limit1 { get; set; }
        public static double End_Min_Force_Limit1 { get; set; }
        public static double End_Max_Pos_Limit1 { get; set; }
        public static double End_Min_Pos_Limit1 { get; set; }
        public static double Mode2 { get; set; }
        public static double Press_Pos2 { get; set; }
        public static double Press_Force2 { get; set; }
        public static double Press_Vel2 { get; set; }
        public static double Press_Time2 { get; set; }
        public static double End_Max_Force_Limit2 { get; set; }
        public static double End_Min_Force_Limit2 { get; set; }
        public static double End_Max_Pos_Limit2 { get; set; }
        public static double End_Min_Pos_Limit2 { get; set; }
        public static double Height_Jig_Top { get; set; }
        public static double Height_Jig_Mid { get; set; }
        public static double Height_Jig_Bottom { get; set; }
        public static double Standard_Roto { get; set; }
        public static double Height_Frame { get; set; }
        public static double Height_Shaft { get; set; }
        public static ushort ID_Error1 { get; set; }
        public static ushort ID_Error2 { get; set; }
        public static bool M_Home_Ep_J_P { get; set; }
        public static bool M_Ep_J_P { get; set; }
        public static bool M_Ep_J_N { get; set; }
        public static bool M_Door_J_P { get; set; }
        public static bool M_Door_J_N { get; set; }
        public static bool M_Ep_ABS { get; set; }
        public static bool Working_Origin { get; set; }
        public static bool Begin_Press { get; set; }
        public static bool Done_Press { get; set; }
        public static bool Product_OK { get; set; }
        public static bool Product_NG { get; set; }
        public static double Position_PV { get; set; }
        public static double Force_PV { get; set; }
        public static bool Check_From_HMI { get; set; }
        public static bool Check_To_HMI { get; set; }
        public static bool Check_Done_Tranfer { get; set; }



    }
    public class DataView_Model
    {
        public int STT { get; set; }
        public string Model { get; set; }
        public string RotorID { get; set; }
        public string TrucID { get; set; }
        // public string Time { get; set; }
    }
    public class DataView_Jig
    {
        public int No { get; set; }
        public string ID { get; set; }
        public string Thickness { get; set; }
    }
    public class DataView_PressingCondition
    {
        public int No { get; set; }
        public string PressingCondition { get; set; }
    }
    public class ID_Model
    {
        public static string Orrder_Code { get; set; }
        public static string ID_Shaft { get; set; }
        public static string ID_Rotor { get; set; }
        public static string ID_Bearing_Upper { get; set; }
        public static string ID_Bearing_Lower { get; set; }
        public static string Model { get; set; }
        public static string Quality { get; set; }

    }
    public class DataFunC
    {
        public float Mode { get; set; }
        public string Press_Condition { get; set; }
        public float Press_Pos { get; set; }
        public float Press_Force { get; set; }
        public float Press_Vel { get; set; }
        public float Press_Time { get; set; }
        public float End_Max_Force_Limit { get; set; }
        public float End_Min_Force_Limit { get; set; }
        public float End_Max_Pos_Limit { get; set; }
        public float End_Min_Pos_Limit { get; set; }
    }
    public class DataFunC2
    {
        public float Mode { get; set; }
        public string Press_Condition { get; set; }
        public float Press_Pos { get; set; }
        public float Press_Force { get; set; }
        public float Press_Vel { get; set; }
        public float Press_Time { get; set; }
        public float End_Max_Force_Limit { get; set; }
        public float End_Min_Force_Limit { get; set; }
        public float End_Max_Pos_Limit { get; set; }
        public float End_Min_Pos_Limit { get; set; }
    }
    public class List_Model
    {
        public string Model { get; set; }
        public string ID_Shaft { get; set; }
        public string ID_Rotor { get; set; }
        public string ID_Bearings_Up { get; set; }
        public string ID_Bearings_Down { get; set; }
        public string Jig_Up { get; set; }
        public string Jig_Mid { get; set; }
        public string Jig_Down { get; set; }
        public float Height_Stand { get; set; }
        public float Thickness_Jig_Up { get; set; }
        public float Thickness_Jig_Down { get; set; }

        public float Origin_Position { get; set; }
        public float Origin_Velocity { get; set; }
        public float Standby_Position  { get; set; }
        public float Standby_Velocity { get; set; }
        public float Standby_Time { get; set; }
        public List<DataFunC> Data_Func1 { get; set; }
        public List<DataFunC> Data_Func2 { get; set; }
    }
    public class List_Model_Temp
    {
        public string Model { get; set; }
        public string ID_Shaft { get; set; }
        public string ID_Rotor { get; set; }
        public string ID_Bearings_Up { get; set; }
        public string ID_Bearings_Down { get; set; }
        public string Jig_Up { get; set; }
        public string Jig_Mid { get; set; }
        public string Jig_Down { get; set; }
        public float Height_Stand { get; set; }
        public float Thickness_Jig_Up { get; set; }
        public float Thickness_Jig_Down { get; set; }

        public float Origin_Position { get; set; }
        public float Origin_Velocity { get; set; }
        public float Standby_Position { get; set; }
        public float Standby_Velocity { get; set; }
        public float Standby_Time { get; set; }
        public List<DataFunC> Data_Func1 { get; set; }
        public List<DataFunC> Data_Func2 { get; set; }
    }
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
    public class Beer_Jig
    {
        public string ID { get; set; }
        public float Thickness { get; set; }
    }
    public class Beer_UP
    {
        public string ID { get; set; }
    }
    public class Beer_Down
    {
        public string ID { get; set; }
    }
    public class Jig_Up
    {
        public string ID { get; set; }
        public float Thickness { get; set; }
    }
    public class Jig_Mid
    {
        public string ID { get; set; }
    }
    public class Jig_Down
    {
        public string ID { get; set; }
        public float Thickness { get; set; }
    }
    public class List_Data
    {
        public string ID { get; set; }
        public float Thickness { get; set; }
    }
    public class List_Temp
    {
        public string ID { get; set; }
        public float Thickness { get; set; }
    }

}

