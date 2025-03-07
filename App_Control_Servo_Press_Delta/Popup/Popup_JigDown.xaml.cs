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
using System.Windows.Threading;
using App_Control_Servo_Press_Delta.Class;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ViewModel;

namespace App_Control_Servo_Press_Delta.Popup
{
    /// <summary>
    /// Interaction logic for Popup_JigDown.xaml
    /// </summary>
    public partial class Popup_JigDown : Window
    {
        Common Common = new Common();
        Link_Path path = new Link_Path();

        public static string pathstring;
        public Popup_JigDown()
        {
            InitializeComponent();
            Loaded += Popup_Loaded;
            Unloaded += Popup_Unloaded;
            DataContext = new MainWindow_VM();
            pathstring = path.Jig_Down;
        }
        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {


            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click += Button_Click;
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus += TextBox_LostFocus;
            }
            foreach (var dataGrid in Common.FindVisualChildren<DataGrid>(this))
            {
                dataGrid.SelectionChanged += Model_SelectionChanged;
            }
            Common.Load_View(List_Model, pathstring);

        }
        private void Popup_Unloaded(object sender, RoutedEventArgs e)
        {

            foreach (var button in Common.FindVisualChildren<Button>(this))
            {
                button.Click -= Button_Click;
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
            }
            foreach (var dataGrid in Common.FindVisualChildren<DataGrid>(this))
            {
                dataGrid.SelectionChanged -= Model_SelectionChanged;
            }
        }

        private void Update_Screen()
        {

        }
        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            string datagridname = datagrid.Name;


            // Lấy dữ liệu từ hàng được chọn
            if (datagridname == "List_Model")
            {
                var selectedRow = List_Model.SelectedItem as DataView_Jig;
                if (selectedRow != null)
                {
                    var data_ID = selectedRow.ID;
                    tb_ID.Text = data_ID.ToString();
                }
            }


        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;

            if (textboxName == "tb_ID")
            {
                Fill_Value_Mode(pathstring);
            }


        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Fill_Value_Mode(string path)
        {
            try
            {
                string jsons = File.ReadAllText(path); ;
                if (jsons.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(jsons);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["ID"] == tb_ID.Text)
                        {
                            tb_ID.Text = (string)obj["ID"];
                            tb_Thichness.Text = (string)obj["Thickness"];
                            break;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Common.Log_err(e.ToString());
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string buttonName = ((Button)sender).Name;
            string PopupName = buttonName.Substring(4);

        }
        private void Save_JigD()
        {
            //            if (float.Parse(txb_Speed_Step2.Text) >= 3 && float.Parse(txb_Speed_Step2.Text) <= 20)
            //          {

            System.DateTime dateTime = System.DateTime.Now;
            List_Data List = new List_Data();
            List.ID = tb_ID.Text;
            List.Thickness = float.Parse(tb_Thichness.Text);
            string list_Json = JsonConvert.SerializeObject(List);
            try
            {
                string json = File.ReadAllText(pathstring);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.ID == tb_ID.Text)
                    {
                        // item.ID = tb_ID_JigU.Text;
                          item.Thickness = float.Parse(tb_Thichness.Text);

                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(pathstring, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        Common.Log_Operation("Edit Model Jig :   " + tb_ID.Text, path.Log_EN);
                        Common.Log_Operation("Sửa Model Jig :   " + tb_ID.Text, path.Log_VN);
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json.Length >0)
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + "," + list_Json + "]";
                        File.WriteAllText(pathstring, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                        Common.Log_Operation("Create Model Jig :   " + tb_ID.Text, path.Log_EN);
                        Common.Log_Operation("Tạo Model Jig :   " + tb_ID.Text, path.Log_VN);
                    }
                }

            }
            catch (Exception e)
            {
                Common.Log_err(e.ToString());
            string jsons;
                jsons = "[" + list_Json + "]";
                File.WriteAllText(pathstring, jsons);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            Common.Load_View(List_Model, pathstring);
        }
        private void Clear_JigD()
        {
            try
            {

                string jsons = File.ReadAllText(pathstring);

                if (tb_ID.Text.Length > 0)
                {

                    var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                    var data = System.Text.Json.JsonSerializer.Deserialize<List_Temp[]>(jsons, options);

                    var newData = new List<List_Temp>();

                    foreach (var item in data)
                    {
                        if (item.ID != tb_ID.Text)
                        {
                            newData.Add(item);
                        }
                    }
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                    // Write back to file
                    File.WriteAllText(pathstring, newJsonString);
                    Common.Log_Operation("Delete Model Jig :   " + tb_ID.Text, path.Log_EN);
                    Common.Log_Operation("Xóa Model Jig :   " + tb_ID.Text, path.Log_VN);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy mã Model: " + tb_ID.Text + " cần xóa");
                }
            }
            catch (Exception e)
            {
                Common.Log_err(e.ToString());
            }

            Common.Load_View(List_Model, pathstring);
        }

        private void btn_Del_Click(object sender, RoutedEventArgs e)
        {

            if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service" || Is_String(MainWindow.UserName, "Admin"))
            {
                MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Jig : " + tb_ID.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Clear_JigD();
                }
                else
                {
                    MessageBox.Show("Xóa model không thành công!");
                }
            }
            else
            {

                MessageBox.Show("Vui Lòng đăng nhập quyền cao nhất!");
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (tb_ID.Text != "" & tb_Thichness.Text != "")
            {
                if (MainWindow.UserName == "STI-Technical" || MainWindow.UserName == "STI-Service" || Is_String(MainWindow.UserName, "Admin"))
                {
                    MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn Lưu mã Model: " + tb_ID.Text + " ?", "Xác nhận Lưu", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Save_JigD();
                    }
                    else
                    {
                        MessageBox.Show("Lưu model không thành công!");
                    }
                }
                else
                {

                    MessageBox.Show("Vui Lòng đăng nhập quyền cao nhất!");
                }

            }
            else
            {
                MessageBox.Show("Vui Lòng nhập đầy đủ thông số");
            }
        }

        private static bool Is_String(string input, string Compari_1)
        {
            return input.Contains(Compari_1);
        }
    }
}
