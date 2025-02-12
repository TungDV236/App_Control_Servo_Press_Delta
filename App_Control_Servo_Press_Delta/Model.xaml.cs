using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using IOPath = System.IO.Path;
using App_Control_Servo_Press_Delta.Popup;
using Microsoft.Win32;
using System.ComponentModel;
using App_Control_Servo_Press_Delta.Class;
using App_Control_Servo_Press_Delta;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Model.xaml
    /// </summary>

    public partial class Model : UserControl
    {
        Link_Path linkpath = new Link_Path();
        Common Common = new Common();
        Excel excel = new Excel();
        public static string model;
        public static string Model_check;
        public static string message = "";
        public Model()
        {
            InitializeComponent();
            Loaded += Model_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Model_Unloaded;
        }
        private void YourMethod()
        {

            List_Models.AddHandler(DataGrid.SelectionChangedEvent, new SelectionChangedEventHandler(Model_SelectionChanged));
        }

        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRow = List_Models.SelectedItem as DataView_Model;
            if (selectedRow != null)
            {
                // Lấy dữ liệu từ hàng được chọn

                var data_Model = selectedRow.Model;
                var data_RotoID = selectedRow.RotorID;
                Model_Model.Text = data_Model.ToString();
            }
        }


        private void Model_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
            }
            foreach (var Combobox in Common.FindVisualChildren<ComboBox>(this))
            {
                Combobox.SelectionChanged += Combobox_Changed;
            }
            //
            YourMethod();
            //
            //
            Fill_ID(File.ReadAllText(linkpath.Beer_Up), Model_Beer_Up, false);
            Fill_ID(File.ReadAllText(linkpath.Beer_Down), Model_Beer_Down, false);
            Fill_ID(File.ReadAllText(linkpath.Jig_Up), Model_Jig_Up, false);
            Fill_ID(File.ReadAllText(linkpath.Jig_Mid), Model_Jig_Mid, false);
            Fill_ID(File.ReadAllText(linkpath.Jig_Down), Model_Jig_Down, false);
            Common.Load_View_Model(List_Models);

        }
        private void Model_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;


        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (!double.TryParse(textBox.Text, out _) & (textBox.Text != "") & (textboxName != "Model_Model") & (textboxName != "Model_RotorID") & (textboxName != "Model_TrucID"))
            {
                // Nếu là số, xóa thông báo lỗi
                MessageBox.Show("Vui Lòng nhập lại dữ liệu kiểu số");
                textBox.Text = "";
            }
            if (textboxName == "Model_Model")
            {
                Fill_Value_Mode();
                // model = Model_Model.Text;
            }
        }
        private void Fill_Value_Mode()
        {

            string json = File.ReadAllText(linkpath.Model);
            int flag = 0;
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["Model"] == Model_Model.Text)
                    {
                        Model_Model.Text = (string)obj["Model"];
                        Model_TrucID.Text = (string)obj["TrucID"];
                        Model_RotorID.Text = (string)obj["RotorID"];
                        CheckValueInComboBox((string)obj["Beer_Up"], Model_Beer_Up);
                        CheckValueInComboBox((string)obj["Beer_Down"], Model_Beer_Down);
                        CheckValueInComboBox((string)obj["Jig_Up"], Model_Jig_Up);
                        CheckValueInComboBox((string)obj["Jig_Mid"], Model_Jig_Mid);
                        CheckValueInComboBox((string)obj["Jig_Down"], Model_Jig_Down);
                        // Model_Beer_Up.SelectedItem = (string)obj["Beer_UP"];
                        // Model_Beer_Down.SelectedItem = (string)obj["Beer_Down"];
                        // Model_Jig_Up.SelectedItem = (string)obj["Jig_Up"];
                        // Model_Jig_Mid.SelectedItem = (string)obj["Jig_Mid"];
                        // Model_Jig_Down.SelectedItem = (string)obj["Jig_Down"];
                        Model_Hstand.Text = (string)obj["HStand"];
                        Model_force.Text = (string)obj["Force"];
                        flag = 1;
                        if (!AreTextBoxesFilled())
                        {
                            MessageBox.Show("Vui lòng kiểm tra lại các thông số đang còn thiếu và tiến hành cài đặt bổ sung các thông số còn thiếu (Cài Đặt--> Thông Số--> Dữ liệu đầu vào)");
                        }
                        break;
                    }


                }
                if (flag == 0)
                {
                    Model_TrucID.Text = "";
                    Model_RotorID.Text = "";
                    Model_Beer_Up.SelectedItem = null;
                    Model_Beer_Down.SelectedItem = null;
                    Model_Jig_Up.SelectedItem = null;
                    Model_Jig_Mid.SelectedItem = null;
                    Model_Jig_Down.SelectedItem = null;
                    Model_Hstand.Text = "";
                    Model_force.Text = "";
                }


            }
        }
        private void Scan_Order()
        {

            string json = File.ReadAllText(linkpath.Model);
            if (json.Length > 0)
            {
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if (((string)obj["TrucID"] == Model_TrucID.Text & (string)obj["RotorID"] == Model_RotorID.Text))
                    {
                        Model_Model.Text = (string)obj["Model"];
                        Model_TrucID.Text = (string)obj["TrucID"];
                        Model_RotorID.Text = (string)obj["RotorID"];
                        Model_Beer_Up.SelectedItem = (string)obj["Beer_Up"];
                        Model_Beer_Down.SelectedItem = (string)obj["Beer_Down"];
                        Model_Jig_Up.SelectedItem = (string)obj["Jig_Up"];
                        Model_Jig_Mid.SelectedItem = (string)obj["Jig_Mid"];
                        Model_Jig_Down.SelectedItem = (string)obj["Jig_Down"];
                        Model_Hstand.Text = (string)obj["HStand"];
                        Model_force.Text = (string)obj["Force"];
                    }
                }
            }
        }
        private void Fill_ID(string json, ComboBox ComboBox, bool flag)
        {

            // string json = File.ReadAllText(linkpath.Model);
            if (json.Length > 0)
            {
                ComboBox.Items.Clear();
                JArray jsonArray = JArray.Parse(json);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["ID"] != "")
                    {
                        ComboBox.Items.Add((string)obj["ID"]);
                    }
                }
                if (flag == false)
                {
                    //   ComboBox.Items.Add("Thêm mới");
                }


            }
        }

        private void Save_Model()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            string ID = formattedDate.Replace("/", "") + formattedtime.Replace(":", "");
            List_Model List_Model = new List_Model();
            List_Model.Model = Model_Model.Text;
            List_Model.TrucID = Model_TrucID.Text;
            List_Model.RotorID = Model_RotorID.Text;
            List_Model.Beer_Up = Model_Beer_Up.SelectedItem.ToString();
            List_Model.Beer_Down = Model_Beer_Down.SelectedItem.ToString();
            List_Model.Jig_Up = Model_Jig_Up.SelectedItem.ToString();
            List_Model.Jig_Mid = Model_Jig_Mid.SelectedItem.ToString();
            List_Model.Jig_Down = Model_Jig_Down.SelectedItem.ToString();
            List_Model.HStand = float.Parse(Model_Hstand.Text);
            List_Model.Force = float.Parse(Model_force.Text);
            string list_Model_Json = JsonConvert.SerializeObject(List_Model);
            try
            {
                string json = File.ReadAllText(linkpath.Model);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Model_Temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.TrucID == Model_TrucID.Text || item.RotorID == Model_RotorID.Text)
                    {
                        item.Model = Model_Model.Text;
                        item.TrucID = Model_TrucID.Text;
                        item.RotorID = Model_RotorID.Text;
                        item.Beer_Up = Model_Beer_Up.SelectedItem.ToString();
                        item.Beer_Down = Model_Beer_Down.SelectedItem.ToString();
                        item.Jig_Up = Model_Jig_Up.SelectedItem.ToString();
                        item.Jig_Mid = Model_Jig_Mid.SelectedItem.ToString();
                        item.Jig_Down = Model_Jig_Down.SelectedItem.ToString();
                        item.HStand = float.Parse(Model_Hstand.Text);
                        item.Force = float.Parse(Model_force.Text);

                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(linkpath.Model, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json.Length < 50)
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + list_Model_Json + "\n]";
                        File.WriteAllText(linkpath.Model, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                    else
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + ",\n" + list_Model_Json + "\n]";
                        File.WriteAllText(linkpath.Model, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }

                }
            }
            catch
            {
                string json_;
                json_ = "[\n" + list_Model_Json + "\n]";
                File.WriteAllText(linkpath.Model, json_);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }

            Common.Load_View_Model(List_Models);

            //           }
            //            else
            //            {
            //               MessageBox.Show("Giá trị tốc độ không phù hợp, giá trị hợp lệ trong khoảng 3-20");
            //           }

        }

        private void Clear_Model()
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Model: " + Model_Model.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & Model_TrucID.Text.Length > 0 & Model_RotorID.Text.Length > 0)
            {

                string json = File.ReadAllText(linkpath.Model);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Model_Temp[]>(json, options);

                var newData = new List<List_Model_Temp>();

                foreach (var item in data)
                {
                    if (item.Model != Model_Model.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(linkpath.Model, newJsonString);
                Common.Load_View_Model(List_Models);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + Model_Model.Text + " cần xóa");
            }

        }


        private void Click_bt_Save_Model(object sender, RoutedEventArgs e)
        {
            if ((Model_TrucID.Text == ""))
            {
                // Nếu là số, xóa thông báo lỗi
                MessageBox.Show("Vui Lòng nhập đầy đủ mã Truc Rotor");
            }
            else if ((Model_Model.Text == ""))
            {
                MessageBox.Show("Vui Lòng nhập đầy đủ mã Model");
            }
            else if ((Model_RotorID.Text == ""))
            {
                MessageBox.Show("Vui Lòng nhập đầy đủ mã Roto");
            }
            else if (!AreTextBoxesFilled())
            {
                MessageBox.Show("Vui Lòng nhập đầy đủ thông số còn thiếu");
            }
            else
            {
                Save_Model();
            }

        }

        private void Click_bt_Del_Model(object sender, RoutedEventArgs e)
        {

            Clear_Model();
        }
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return !string.IsNullOrWhiteSpace(Model_Model.Text) &&
                   !string.IsNullOrWhiteSpace(Model_TrucID.Text) &&
                   !string.IsNullOrWhiteSpace(Model_RotorID.Text) &&
                   !(Model_Beer_Up.SelectedItem == null) &&
                   !(Model_Beer_Down.SelectedItem == null) &&
                   !(Model_Jig_Up.SelectedItem == null) &&
                   !(Model_Jig_Mid.SelectedItem == null) &&
                   !(Model_Jig_Down.SelectedItem == null) &&
                   !string.IsNullOrWhiteSpace(Model_Hstand.Text) &&
                   !string.IsNullOrWhiteSpace(Model_force.Text);
        }
        private void CheckValueInComboBox(string valueToCheck, ComboBox comboBox)
        {
            // Kiểm tra xem giá trị có trong ComboBox hay không
            if (comboBox.Items.Contains(valueToCheck))
            {
                //   MessageBox.Show($"Giá trị '{valueToCheck}' có trong ComboBox.");
                comboBox.SelectedItem = valueToCheck;
            }
            else
            {
                switch (comboBox.Name)
                {

                    case "Model_Beer_Down":
                        Model_check = "Vòng bi dưới";
                        break;
                    case "Model_Beer_Up":
                        Model_check = "Vòng bi trên";
                        break;
                    case "Model_Jig_Up":
                        Model_check = "Jig trên";
                        break;
                    case "Model_Jig_Mid":
                        Model_check = "Jig giữa";
                        break;
                    case "Model_Jig_Down":
                        Model_check = "Jig dưới";
                        break;

                    default:
                        Model_check = "Invalid option";
                        break;
                }
                //     MessageBox.Show("Vui lòng nhập Model " + Model_check + $" '{valueToCheck}' vào cài đặt.");
                comboBox.SelectedItem = null;
            }
        }
        private void Click_bt_Import_model(object sender, RoutedEventArgs e)
        {
            excel.Import_Model_Filepath();
            Common.Load_View_Model(List_Models);
        }

        private void Click_bt_Export_model(object sender, RoutedEventArgs e)
        {
            excel.Export_Model_File("Template_Model", linkpath.Model);
        }

        private void Click_bt_Template_model(object sender, RoutedEventArgs e)
        {
            var Result = Excel.Coppy_File("Template_Model");
            MessageBox.Show("Đã tạo Template thành công.");
        }




    }
}
