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
            }
        }


        private void Model_Loaded(object sender, RoutedEventArgs e)
        {


        }
        private void Model_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;


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
          
        }

        private void Clear_Model()
        {
           

        }




        private void Click_bt_Del_Model(object sender, RoutedEventArgs e)
        {

            Clear_Model();
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
