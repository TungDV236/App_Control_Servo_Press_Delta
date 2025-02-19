using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.IO;

namespace App_Control_Servo_Press_Delta.Class
{
    public class Socket_client
    {
        private SocketIOClient.SocketIO client;
        public int Flag_Server;
        public static bool IsConnected;
        public static dynamic data_;
        Link_Path path = new Link_Path();

        public async void ConnectToServer()
        {
            try
            {

                string json = File.ReadAllText(path.Setting);
                var data_Setting = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                Global.Server = data_Setting["Server"];
                client = new SocketIOClient.SocketIO("http://"+ Global.Server);
                client.OnConnected += async (sender, e) =>
                {
                    IsConnected = true;
                };
                client.OnDisconnected += async (sender, e) =>
                {
                    IsConnected = false;
                };

                client.On("get_data_order_code", response =>
                {
                    string serverMessage = response.GetValue<string>();
                    MessageBox.Show("Phản hồi từ server (JSON): " + serverMessage);

                });

                await client.ConnectAsync();
                await Task.Delay(1000); // Chờ để đảm bảo kết nối hoàn tất
                                        // await client.EmitAsync("get_data_order_code", response =>
                                        // {
                                        //
                                        //     string jsonString = response.GetValue<System.Text.Json.JsonElement>().GetRawText();
                                        //
                                        //     Dispatcher.Invoke(() =>
                                        //     {
                                        //         MessageBox.Show("Phản hồi từ server (JSON): " + jsonString);
                                        //     });
                                        // }, "{\"Orrder_Code\":\"M00000007323F,01,41,AHNRZ17391,001\"}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        public async Task Emit_Server(string Code)
        {

            using (SocketIOClient.SocketIO client1 = new SocketIOClient.SocketIO("http://"+ Global.Server))
            {
                // Kết nối với server
                await client1.ConnectAsync();

                if (!string.IsNullOrEmpty(Code))
                {
                    object data_Setting = new
                    {
                        Order_Code = Code,
                    };

                    string json = System.Text.Json.JsonSerializer.Serialize(data_Setting);

                    try
                    {
                        await  client1.EmitAsync("get_data_order_code", response1 =>
                        {

                            string jsonString = response1.GetValue<System.Text.Json.JsonElement>().GetRawText();


                            Console.WriteLine($"Phản hồi từ máy chủ: {jsonString}\n");
                            //  MessageBox.Show("Phản hồi từ server (JSON): " + jsonString);

                            var data_respond = response1.GetValue<System.Text.Json.JsonElement>().GetRawText();

                            data_ = JsonConvert.DeserializeObject<dynamic>(data_respond);
                            if (data_ != null)
                            {
                                Parse_Data(data_);
                                //   MainWindow_VM.Queue_Server.RemoveAt(0);
                            }
                        }, json);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                    }
                }
            }
        }
        public async Task Emit_Server1(string Code)
        {
            using (SocketIOClient.SocketIO client1 = new SocketIOClient.SocketIO("http://127.0.0.1:3000"))
            {
                // Kết nối với server
                await client1.ConnectAsync();

                if (!string.IsNullOrEmpty(Code))
                {
                    object data_Setting = new
                    {
                        Order_Code = Code,
                    };

                    string json = System.Text.Json.JsonSerializer.Serialize(data_Setting);
                    var tcs = new TaskCompletionSource<string>();

                    try
                    {
                        // Lắng nghe phản hồi từ server
                        client1.On("get_data_order_code", response1 =>
                        {
                            string jsonString = response1.GetValue<System.Text.Json.JsonElement>().GetRawText();
                            Console.WriteLine($"Phản hồi từ máy chủ: {jsonString}");

                            // Thiết lập kết quả cho TaskCompletionSource
                            tcs.SetResult(jsonString);
                        });

                        // Gửi dữ liệu đến server
                        await client1.EmitAsync("get_data_order_code", json);

                        // Chờ đợi phản hồi từ server
                        string response = await tcs.Task;

                        // Xử lý phản hồi
                        var data_ = JsonConvert.DeserializeObject<dynamic>(response);
                        if (data_ != null)
                        {
                            Parse_Data(data_);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                    }
                    finally
                    {
                        // Đảm bảo loại bỏ listener để tránh rò rỉ bộ nhớ
                        client1.Off("get_data_order_code");
                    }
                }
            }
        }

        private void Parse_Data(dynamic data)
        {
            try
            {
                // Input
                ID_Model.Orrder_Code = data.Orrder_Code;
                ID_Model.ID_Shaft = data.ID_Shaft;
                ID_Model.ID_Rotor = data.ID_Rotor;
                ID_Model.ID_Bearing_Upper = data.ID_Bearing_Upper;
                ID_Model.ID_Bearing_Lower = data.ID_Bearing_Lower;
                ID_Model.Model = data.Model;
                Global.Receive = true;
                ID_Model.Quality = data.Quality;
                Console.WriteLine($"Orrder_Code: {ID_Model.Orrder_Code}");
                Console.WriteLine($"ID_Shaft: {ID_Model.ID_Shaft}");
                Console.WriteLine($"ID_Rotor: {ID_Model.ID_Rotor}");
                Console.WriteLine($"ID_Bearing_Upper: {ID_Model.ID_Bearing_Upper}");
                Console.WriteLine($"ID_Bearing_Lower: {ID_Model.ID_Bearing_Lower}");
                Console.WriteLine($"ID_Bearing_Lower: {ID_Model.Quality}");

                //
            }
            catch (Exception e)
            {
                //  Common.Log_err("PLC", " Parse_Data", e.ToString());
                //  MessageBox.Show(e.ToString());
            }
        }
    }
}
