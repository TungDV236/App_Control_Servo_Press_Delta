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
        public static string Server;
        public static string PLC_Write;
        private int Flag_PLC;


        public void StartTimer()
        {
            timer = new System.Threading.Timer(Timer_Tick, null, 0, 200);
            string json = File.ReadAllText(path.Setting);
            var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Server = data_Setting["Server"];
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
                    string response = await client.GetStringAsync("http://" + Server + "/api/data");
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
                Data.M_Ep_J_P = data.M_Ep_J_P;
                Data.M_Ep_J_N = data.M_Ep_J_N;
                Data.M_Ep_ABS_J_P = data.M_Ep_ABS_J_P;
                Data.M_Ep_ABS_J_N = data.M_Ep_ABS_J_N;
                Data.M_Door_J_P = data.M_Door_J_P;
                Data.M_Door_J_N = data.M_Door_J_N;
                Data.M_Home_J_N = data.M_Home_J_N;
                Data.Off_Buzzer = data.Off_Buzzer;
                Data.On_Ep = data.On_Ep;
                Data.ORG_X = data.ORG_X;
                Data.Step_abs = data.Step_abs;
                Data.Process = data.Process;
                Data.Alarm1 = data.Alarm1;
                Data.Alarm2 = data.Alarm2;
                Data.Error1 = data.Error1;
                Data.Error2 = data.Error2;
                Data.Momen_set = data.Momen_set;
                Data.H_Stand = data.H_Stand;
                Data.Limit_U = data.Limit_U;
                Data.Limit_D = data.Limit_D;
                Data.Momen_max = data.Momen_max;
                Data.IB0 = data.IB0;
                Data.IB1 = data.IB1;
                Data.QB0 = data.QB0;
                Data.QB1 = data.QB1;
                Data.Momen_PV = data.Momen_PV;
                Data.Position = data.Position;
                Data.V_Auto = data.V_Auto;
                Data.V_man = data.V_man;

                Data.Check_Connect = data.Check_Connect;
                Data.Check_Update = data.Check_Update;

                //
            }
            catch (Exception e)
            {
                Common.Log_err("PLC", " Parse_Data", e.ToString());
                //  MessageBox.Show(e.ToString());
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
                    HttpResponseMessage response = await client.PostAsync("http://" + Server + "/api/Control_PLC_1", content);

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

