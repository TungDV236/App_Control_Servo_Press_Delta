
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta.Popup
{
    /// <summary>
    /// Interaction logic for Model_Beer_Up.xaml
    /// </summary>
    public partial class Model_Beer_Down : Window
    {


        public Link_Path linkpath = new Link_Path();
        Excel excel = new Excel();
        Common Common = new Common();
        public string path;
        public string json;
        public Model_Beer_Down()
        {
            InitializeComponent();
            Loaded += Model_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Model_Unloaded;
        }
        private void Model_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
            }
            List_Models.AddHandler(DataGrid.SelectionChangedEvent, new SelectionChangedEventHandler(Model_SelectionChanged));

            path = linkpath.Beer_Down;
            json = File.ReadAllText(linkpath.Beer_Down);
            Common.Load_View(List_Models, path);
        }

        private void Model_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (textboxName == "TB_Model")
            {
                Fill_Value_Mode();
                // model = Model_Model.Text;
            }
        }
        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRow = List_Models.SelectedItem as DataView;
            if (selectedRow != null)
            {
                // Lấy dữ liệu từ hàng được chọn

                var data_Model = selectedRow.ID;
                TB_Model.Text = data_Model.ToString();
            }
        }

        private void Fill_Value_Mode()
        {
            string jsons = json;
            if (jsons.Length > 0)
            {
                JArray jsonArray = JArray.Parse(jsons);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["ID"] == TB_Model.Text)
                    {
                        TB_Model.Text = (string)obj["ID"];
                        break;
                    }
                }
            }
        }
        private void Fill_ID(string jsons, ComboBox ComboBox, bool flag)
        {

            // string json = File.ReadAllText(linkpath.Model);
            if (jsons.Length > 0)
            {
                ComboBox.Items.Clear();
                JArray jsonArray = JArray.Parse(jsons);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["ID"] != "")
                    {
                        ComboBox.Items.Add((string)obj["ID"]);
                    }
                }
                if (flag == false)
                {
                    ComboBox.Items.Add("Thêm mới");
                }


            }
        }
        private void Fill_Property_ID(string jsons, string Combobox_select, int flag, out string Model, out string Thickness)
        {
            Model = "";
            Thickness = "";
            if (jsons.Length > 0)
            {
                JArray jsonArray = JArray.Parse(jsons);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["ID"] == Combobox_select)
                    {
                        if (flag == 1)
                        {
                            Model = (string)obj["ID"];
                        }
                        else if (flag == 2)
                        {
                            Model = (string)obj["ID"];
                            Thickness = (string)obj["Thickness"];
                        }

                    }
                }
            }

        }
        private void Save_Model()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            string json_ = json;
            System.DateTime dateTime = System.DateTime.Now;
            List_Data List = new List_Data();
            List.ID = TB_Model.Text;
            string list_Json = JsonConvert.SerializeObject(List);
            try
            {
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(json_, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.ID == TB_Model.Text)
                    {
                        item.ID = TB_Model.Text;

                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(path, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json_.Length < 10)
                    {
                        json_ = json_.Remove(json_.Length - 1);
                        json_ = json_ + list_Json + "]";
                        File.WriteAllText(path, json_);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                    else
                    {
                        json_ = json_.Remove(json_.Length - 1);
                        json_ = json_ + "," + list_Json + "]";
                        File.WriteAllText(path, json_);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                }

            }
            catch
            {
                string jsons;
                jsons = "[" + list_Json + "]";
                File.WriteAllText(path, jsons);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            json = File.ReadAllText(path);
            Common.Load_View(List_Models, path);
        }

        private void Clear_Model()
        {
            string jsons = json;
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Model: " + TB_Model.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & TB_Model.Text.Length > 0)
            {

                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(jsons, options);

                var newData = new List<List_Temp>();

                foreach (var item in data)
                {
                    if (item.ID != TB_Model.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(path, newJsonString);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + TB_Model.Text + " cần xóa");
            }
            json = File.ReadAllText(path);
            Common.Load_View(List_Models, path);
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
        private void Click_bt_Import(object sender, RoutedEventArgs e)
        {

            excel.Import_BJ_Filepath("Model_Beer_Down", linkpath.Beer_Down);
            Common.Load_View(List_Models, path);
        }

        private void Click_bt_Export(object sender, RoutedEventArgs e)
        {
            excel.Export_BJ_File("Template_Model_Beer_Down", linkpath.Beer_Down, "Model_Beer_Down");
        }

        private void Click_bt_Del_Model(object sender, RoutedEventArgs e)
        {
            Clear_Model();
        }

        private void Click_bt_Save_Model(object sender, RoutedEventArgs e)
        {
            Save_Model();
        }
        private void exitButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BTN_Exit.Background = Brushes.Red; // Thay đổi màu nền khi di chuột qua
        }

        private void exitButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BTN_Exit.Background = Brushes.Transparent; // Đặt lại màu nền khi chuột rời đi
        }
    }
}
