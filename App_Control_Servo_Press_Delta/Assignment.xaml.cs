using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

using System.Text.Json;
using App_Control_Servo_Press_Delta;



namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Assignment.xaml
    /// </summary>
    public partial class Assignment : Window
    {
        private static string _json;
        System.DateTime dateTime = System.DateTime.Now;
        public Link_Path linkpath = new Link_Path();
        Common Common = new Common();
        public class Time_Work
        {
            public string Name { get; set; }
            public string Time_Start { get; set; }
            public string Time_Stop { get; set; }
        }
        public class Time_Work_Temp
        {
            public string Name { get; set; }
            public string Time_Start { get; set; }
            public string Time_Stop { get; set; }
        }
        public Assignment()
        {
            InitializeComponent();
            Loaded += Assignment_Loaded;
            Unloaded += Assignment_Unloaded;
        }
        private void Assignment_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var Combobox in Common.FindVisualChildren<ComboBox>(this))
            {
                Combobox.SelectionChanged += Combobox_Changed;
            }

            Fill();

        }
        private void Assignment_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            if (assignment.SelectedItem.ToString() != null)
            {
                Fill_para(assignment.SelectedItem.ToString());
            }

        }

        private void Fill_para(string name)
        {
            string json = File.ReadAllText(linkpath.Time_work);
            // string json = File.ReadAllText(linkpath.Model);
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["Name"] == name)
                    {
                        string[] Time_start = ((string)obj["Time_Start"]).Split(new char[] { ':' });
                        string[] Time_stop = ((string)obj["Time_Stop"]).Split(new char[] { ':' });
                        hh_start.Text = Time_start[0];
                        mm_start.Text = Time_start[1];
                        hh_stop.Text = Time_stop[0];
                        mm_stop.Text = Time_stop[1];

                    }
                }
            }
        }
        private void Fill()
        {
            string json = File.ReadAllText(linkpath.Time_work);
            // string json = File.ReadAllText(linkpath.Model);
            if (json.Length > 0)
            {
                assignment.Items.Clear();
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["Name"] != "")
                    {
                        assignment.Items.Add((string)obj["Name"]);
                    }
                }
            }
        }
        private void Save()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            string json_ = File.ReadAllText(linkpath.Time_work);

            Time_Work List = new Time_Work();
            List.Name = assignment.SelectedItem.ToString();
            List.Time_Start = hh_start.Text.ToString() + ":" + mm_start.Text.ToString();
            List.Time_Stop = hh_stop.Text.ToString() + ":" + mm_stop.Text.ToString();
            string list_Json = JsonConvert.SerializeObject(List);
            try
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<Time_Work_Temp[]>(json_, options);
                foreach (var item in data)
                {

                    if (item.Name == assignment.SelectedItem.ToString())
                    {
                        //  item.Name = assignment.SelectedItem.ToString();
                        item.Time_Start = hh_start.Text.ToString() + ":" + mm_start.Text.ToString();
                        item.Time_Stop = hh_stop.Text.ToString() + ":" + mm_stop.Text.ToString();
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(linkpath.Time_work, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        break;
                    }
                }
            }
            catch
            {
                string jsons;
                jsons = "[" + list_Json + "]";
                File.WriteAllText(linkpath.Time_work, jsons);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
        }

        private void BT_save(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void BT_exit(object sender, RoutedEventArgs e)
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
        private void infor_exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
