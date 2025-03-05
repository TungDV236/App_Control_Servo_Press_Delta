using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Concurrent;
using System.Web.UI;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;
using System.Net.Sockets;


namespace App_Control_Servo_Press_Delta.Class
{
    public class PLC
    {
        Link_Path path = new Link_Path();
        public static dynamic data_;
        public static bool flag = false;
        private static HttpClient client = new HttpClient();
        public static bool IsConnected;
        public static string Hostting_;
        private System.Threading.Timer timer;
        private readonly object timerLock = new object();
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>();
        Common Common = new Common();
        //
        public static string PLC_Write;
        private int Flag_PLC;

        public void StartTimer()
        {
            timer = new System.Threading.Timer(Timer_Tick, null, 0, 100);
            string json = File.ReadAllText(path.Setting);
            var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Global.PLC_IP = data_Setting["PLC"];
            // PLC_Write = data_Setting["PLC_IP"];
        }

        public void StopTimer()
        {
            lock (timerLock)
            {
                timer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async void Timer_Tick(object state)
        {
            if (Flag_PLC > 3)
            {
                IsConnected = false;
            }
            else
            {
                IsConnected = true;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string response = await client.GetStringAsync("http://" + Global.PLC_IP + "/api/data");
                    data_ = JsonConvert.DeserializeObject<dynamic>(response);
                    if (data_ != null)
                    {
                        Parse_Data(data_);
                    }
                    Flag_PLC = 0;
                }
            }
            catch
            {
                Flag_PLC++;
            }
        }



        private void Parse_Data(dynamic data)
        {
            try
            {
                // Input
                Data.Jog_Max_Force = data.Jog_Max_Force;
                Data.Jog_Distance_ABS = data.Jog_Distance_ABS;
                Data.Jog_Vel = data.Jog_Vel;
                Data.Go_Home_Vel = data.Go_Home_Vel;
                Data.Origin_Work_Pos = data.Origin_Work_Pos;
                Data.Origin_Work_Vel = data.Origin_Work_Vel;
                Data.Standby_Pos = data.Standby_Pos;
                Data.Standby_Vel = data.Standby_Vel;
                Data.Standby_Time = data.Standby_Time;
                Data.Mode1 = data.Mode1;
                Data.Press_Pos1 = data.Press_Pos1;
                Data.Press_Force1 = data.Press_Force1;
                Data.Press_Vel1 = data.Press_Vel1;
                Data.Press_Time1 = data.Press_Time1;
                Data.End_Max_Force_Limit1 = data.End_Max_Force_Limit1;
                Data.End_Min_Force_Limit1 = data.End_Min_Force_Limit1;
                Data.End_Max_Pos_Limit1 = data.End_Max_Pos_Limit1;
                Data.End_Min_Pos_Limit1 = data.End_Min_Pos_Limit1;
                Data.Mode2 = data.Mode2;
                Data.Press_Pos2 = data.Press_Pos2;
                Data.Press_Force2 = data.Press_Force2;
                Data.Press_Vel2 = data.Press_Vel2;
                Data.Press_Time2 = data.Press_Time2;
                Data.End_Max_Force_Limit2 = data.End_Max_Force_Limit2;
                Data.End_Min_Force_Limit2 = data.End_Min_Force_Limit2;
                Data.End_Max_Pos_Limit2 = data.End_Max_Pos_Limit2;
                Data.End_Min_Pos_Limit2 = data.End_Min_Pos_Limit2;
                Data.ofset_Machine = data.ofset_Machine;
                Data.Height_Jig_Base = data.Height_Jig_Base;
                Data.Height_Jig_Bottom = data.Height_Jig_Bottom;
                Data.Standard_Roto = data.Standard_Roto;
                Data.Height_Frame = data.Height_Frame;
                Data.Height_Shaft = data.Height_Shaft;
                Data.ID_Error1 = data.ID_Error1;
                Data.ID_Error2 = data.ID_Error2;
                Data.M_Home_Ep_J_P = data.M_Home_Ep_J_P;
                Data.M_Ep_U_J_P = data.M_Ep_U_J_P;
                Data.M_Ep_D_J_N = data.M_Ep_D_J_N;
                Data.M_Door_U_J_P = data.M_Door_U_J_P;
                Data.M_Door_D_J_N = data.M_Door_D_J_N;
                Data.M_Ep_ABS = data.M_Ep_ABS;
                Data.Working_Origin = data.Working_Origin;
                Data.Begin_Press = data.Begin_Press;
                Data.Done_Press = data.Done_Press;
                Data.Product_OK = data.Product_OK;
                Data.Product_NG = data.Product_NG;
                Data.Position_PV = data.Position_PV;
                Data.Force_PV = data.Force_PV;
                Data.IB0 = data.IB0;
                Data.XB0 = data.XB0;
                Data.XB1 = data.XB1;
                Data.XB2 = data.XB2;
                Data.QB0 = data.QB0;
                Data.YB1 = data.YB1;
                Data.YB2 = data.YB2;
                Data.Check_From_HMI = data.Check_From_HMI;
                Data.Check_To_HMI = data.Check_To_HMI;
                Data.Check_Done_Tranfer = data.Check_Done_Tranfer;
                Data.Total_NG = data.Total_NG;
                Data.Total_OK = data.Total_OK;




                //
            }
            catch (Exception e)
            {
                Common.Log_err("PLC", " Parse_Data", e.ToString());
                  MessageBox.Show(e.ToString());
            }
        }



        public async Task SendPostRequestAsync()
        {


            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string json = MainWindow._queue[0];
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://" + Global.PLC_IP + "/api/Control_PLC_1", content);

                    if (response.IsSuccessStatusCode)
                    {
                        if (MainWindow._queue.Count > 0)
                        {
                            MainWindow._queue.RemoveAt(0);
                        }
                        Console.WriteLine("Request sent successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }



    }
}

