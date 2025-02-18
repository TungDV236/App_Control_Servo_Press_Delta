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
using System.Text.RegularExpressions;

namespace App_Control_Servo_Press_Delta
{
    /// <summary>
    /// Interaction logic for Model.xaml
    /// </summary>

    public partial class Model : UserControl
    {

        Link_Path path = new Link_Path();
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

        private void Model_Loaded(object sender, RoutedEventArgs e)
        {
            Fill_ID(File.ReadAllText(path.Jig_Up), cbb_JigU);
            Fill_ID(File.ReadAllText(path.Jig_Mid), cbb_JigM);
            Fill_ID(File.ReadAllText(path.Jig_Down), cbb_JigD);
            Common.Load_View_Model(List_Models);
            foreach (var dataGrid in Common.FindVisualChildren<DataGrid>(this))
            {
                dataGrid.SelectionChanged += Datagrid_SelectionChanged;
            }
            foreach (var Combobox in Common.FindVisualChildren<ComboBox>(this))
            {
                Combobox.SelectionChanged += Combobox_Changed;
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus;
            }
            var Pressing_Condition = new List<DataView_PressingCondition>
            {
                new DataView_PressingCondition { No = 1,PressingCondition = "---" },
                new DataView_PressingCondition { No = 2,PressingCondition = "---" }
            };
            // Gán dữ liệu cho DataGrid
            List_Pressing_condition.ItemsSource = Pressing_Condition;

            Init_data();
        }
        private void Model_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string comboboxname = comboBox.Name;
            var selectedcondition = (ComboBoxItem)cbb_Pressing_condition.SelectedItem;
            int selectedValue;
            if (cbb_step.SelectedItem!=null& cbb_Pressing_condition.SelectedItem!=null)
            {
                var selectedItem = (ComboBoxItem)cbb_step.SelectedItem;
                if (int.TryParse(selectedItem.Content.ToString(), out selectedValue))
                {
                    Edit_Condition(selectedValue, selectedcondition.Content.ToString());
                }    
            }    

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            string input = textBox.Text;
            if (textboxName == "tb_Model")
            {
                Fill_Value_Mode();
            }    

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;


        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int selectedValue;
            if (cbb_step.SelectedItem != null & cbb_Pressing_condition.SelectedItem != null)
            {
                var selectedItem = (ComboBoxItem)cbb_step.SelectedItem;
                if (int.TryParse(selectedItem.Content.ToString(), out selectedValue))
                {
                    Save_Parameter(selectedValue);
                }
            }


        }
        private void Datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            string datagridname = datagrid.Name;

            if (datagridname == "cbb_Pressing_condition")
            {
                var selectedRow = List_Pressing_condition.SelectedItem as DataView_PressingCondition;
                if (selectedRow != null)
                {
                    // Lấy dữ liệu từ hàng được chọn
                    cbb_step.SelectedItem = selectedRow.No.ToString();
                    cbb_Pressing_condition.SelectedIndex = Fill_CBB_Press_Codition(selectedRow.PressingCondition.ToString(), cbb_Pressing_condition);
                }
            }

            if (datagridname == "List_Models")
            {
                var selectedRow = List_Models.SelectedItem as DataView_Model;
                if (selectedRow != null)
                {
                    var data_Model = selectedRow.Model;
                    tb_Model.Text = data_Model.ToString();
                }
            }
        }
        private void Init_data()
        {
            Global.Function1 = new List<DataFunC>
           {
                new DataFunC
                {
                    Mode = 0,
                    Press_Condition = "",
                    Press_Pos = 0,
                    Press_Force = 0,
                    Press_Vel = 0,
                    Press_Time = 0,
                    End_Max_Force_Limit = 0,
                    End_Min_Force_Limit = 0,
                    End_Max_Pos_Limit = 0,
                    End_Min_Pos_Limit = 0
                }
             };
            Global.Function2 = new List<DataFunC>
           {
                new DataFunC
                {
                    Mode = 0,
                    Press_Condition = "",
                    Press_Pos = 0,
                    Press_Force = 0,
                    Press_Vel = 0,
                    Press_Time = 0,
                    End_Max_Force_Limit = 0,
                    End_Min_Force_Limit = 0,
                    End_Max_Pos_Limit = 0,
                    End_Min_Pos_Limit = 0
                }
             };
        }
        private void Clear_Model()
        {
            Global.Function1[0].Mode = 0;
            Global.Function1[0].Press_Condition = "";
            Global.Function1[0].Press_Pos = 0;
            Global.Function1[0].Press_Force = 0;
            Global.Function1[0].Press_Vel = 0;
            Global.Function1[0].Press_Time = 0;
            Global.Function1[0].End_Max_Force_Limit = 0;
            Global.Function1[0].End_Min_Force_Limit = 0;
            Global.Function1[0].End_Max_Pos_Limit = 0;
            Global.Function1[0].End_Min_Pos_Limit = 0;
            Global.Function2[0].Mode = 0;
            Global.Function2[0].Press_Condition = "";
            Global.Function2[0].Press_Pos = 0;
            Global.Function2[0].Press_Force = 0;
            Global.Function2[0].Press_Vel = 0;
            Global.Function2[0].Press_Time = 0;
            Global.Function2[0].End_Max_Force_Limit = 0;
            Global.Function2[0].End_Min_Force_Limit = 0;
            Global.Function2[0].End_Max_Pos_Limit = 0;
            Global.Function2[0].End_Min_Pos_Limit = 0;
            //
            tb_Origin_PST.Text = "";
            tb_Origin_PST.Text = "";
            tb_PST_Standby.Text = "";
            tb_Standby_Velocity.Text = "";
            tb_Standby_Time.Text = "";
            tb_Force_Min.Text = "";
            tb_Force_Max.Text = "";
            tb_Position_Min.Text = "";
            tb_Position_Max.Text = "";
            tb_Press_PositionDistance.Text = "";
            tb_Press_Force.Text = "";
            tb_Press_Velocity.Text = "";
            tb_Press_Time.Text = "";
            cbb_Pressing_condition.SelectedIndex = -1;
            cbb_step.SelectedIndex = -1;
            //
            tb_Rotor.Text = "";
            tb_Shaft.Text = "";
            tb_BearingD.Text = "";
            tb_BearingU.Text = "";
            cbb_JigU.SelectedIndex = -1;
            cbb_JigM.SelectedIndex = -1;
            cbb_JigD.SelectedIndex = -1;

            // Tạo dữ liệu mẫu
            var Pressing_Condition = new List<DataView_PressingCondition>
            {
                new DataView_PressingCondition { No = 1,PressingCondition = "---" },
                new DataView_PressingCondition { No = 2,PressingCondition = "---" }
            };

            // Gán dữ liệu cho DataGrid
            List_Pressing_condition.ItemsSource = Pressing_Condition;
        }
        private void Del_Pressing_condition(int step_Del)
        {
            // Tạo dữ liệu mẫu
            if (List_Pressing_condition.ItemsSource is List<DataView_PressingCondition> Pressing_Condition_old && Pressing_Condition_old.Count > 1)
            {
                if (step_Del == 1)
                {
                    DataView_PressingCondition _Pressing_Condition = Pressing_Condition_old[1];
                    var Pressing_Condition = new List<DataView_PressingCondition>
                        {
                            new DataView_PressingCondition { No = 1,PressingCondition = "---" },
                            new DataView_PressingCondition { No = 2,PressingCondition = _Pressing_Condition.PressingCondition }
                        };
                    List_Pressing_condition.ItemsSource = Pressing_Condition;
                    tb_Force_Min.Text = "";
                    tb_Force_Max.Text = "";
                    tb_Position_Min.Text = "";
                    tb_Position_Max.Text = "";
                    tb_Press_PositionDistance.Text = "";
                    tb_Press_Force.Text = "";
                    tb_Press_Velocity.Text = "";
                    tb_Press_Time.Text = "";
                    cbb_Pressing_condition.SelectedIndex = -1;
                    cbb_step.SelectedIndex = -1;
                    Global.Function2[0].Mode = 0;
                    Global.Function2[0].Press_Condition = "";
                    Global.Function2[0].Press_Pos = 0;
                    Global.Function2[0].Press_Force = 0;
                    Global.Function2[0].Press_Vel = 0;
                    Global.Function2[0].Press_Time = 0;
                    Global.Function2[0].End_Max_Force_Limit = 0;
                    Global.Function2[0].End_Min_Force_Limit = 0;
                    Global.Function2[0].End_Max_Pos_Limit = 0;
                    Global.Function2[0].End_Min_Pos_Limit = 0;
                }
                if (step_Del == 2)
                {
                    DataView_PressingCondition _Pressing_Condition = Pressing_Condition_old[0];
                    var Pressing_Condition = new List<DataView_PressingCondition>
                        {
                            new DataView_PressingCondition { No = 1,PressingCondition = _Pressing_Condition.PressingCondition },
                            new DataView_PressingCondition { No = 2,PressingCondition = "---" }
                        };
                    List_Pressing_condition.ItemsSource = Pressing_Condition;

                    tb_Force_Min.Text = "";
                    tb_Force_Max.Text = "";
                    tb_Position_Min.Text = "";
                    tb_Position_Max.Text = "";
                    tb_Press_PositionDistance.Text = "";
                    tb_Press_Force.Text = "";
                    tb_Press_Velocity.Text = "";
                    tb_Press_Time.Text = "";
                    cbb_Pressing_condition.SelectedIndex = -1;
                    cbb_step.SelectedIndex = -1;
                    Global.Function1[0].Mode = 0;
                    Global.Function1[0].Press_Condition = "";
                    Global.Function1[0].Press_Pos = 0;
                    Global.Function1[0].Press_Force = 0;
                    Global.Function1[0].Press_Vel = 0;
                    Global.Function1[0].Press_Time = 0;
                    Global.Function1[0].End_Max_Force_Limit = 0;
                    Global.Function1[0].End_Min_Force_Limit = 0;
                    Global.Function1[0].End_Max_Pos_Limit = 0;
                    Global.Function1[0].End_Min_Pos_Limit = 0;
                }
            }
        }
        private void Edit_Condition(int step_Edit, string Condition)
        {
            // Tạo dữ liệu mẫu
            if (List_Pressing_condition.ItemsSource is List<DataView_PressingCondition> Pressing_Condition_old && Pressing_Condition_old.Count > 1)
            {
                if (step_Edit == 1)
                {
                    DataView_PressingCondition _Pressing_Condition = Pressing_Condition_old[1];
                    var Pressing_Condition = new List<DataView_PressingCondition>
                        {
                            new DataView_PressingCondition { No = 1,PressingCondition = Condition },
                            new DataView_PressingCondition { No = 2,PressingCondition = _Pressing_Condition.PressingCondition }
                        };
                    List_Pressing_condition.ItemsSource = Pressing_Condition;
                    tb_Force_Min.Text = "";
                    tb_Force_Max.Text = "";
                    tb_Position_Min.Text = "";
                    tb_Position_Max.Text = "";
                    tb_Press_PositionDistance.Text = "";
                    tb_Press_Force.Text = "";
                    tb_Press_Velocity.Text = "";
                    tb_Press_Time.Text = "";
                    //cbb_Pressing_condition.SelectedIndex = -1;
                    //cbb_step.SelectedIndex = -1;
                    if (Global.Function1[0].Press_Condition != null)
                    {
                        Global.Function1[0].Mode = 0;
                        Global.Function1[0].Press_Condition = "";
                        Global.Function1[0].Press_Pos = 0;
                        Global.Function1[0].Press_Force = 0;
                        Global.Function1[0].Press_Vel = 0;
                        Global.Function1[0].Press_Time = 0;
                        Global.Function1[0].End_Max_Force_Limit = 0;
                        Global.Function1[0].End_Min_Force_Limit = 0;
                        Global.Function1[0].End_Max_Pos_Limit = 0;
                        Global.Function1[0].End_Min_Pos_Limit = 0;
                    }
                }
                if (step_Edit == 2)
                {
                    DataView_PressingCondition _Pressing_Condition = Pressing_Condition_old[0];
                    var Pressing_Condition = new List<DataView_PressingCondition>
                        {
                            new DataView_PressingCondition { No = 1,PressingCondition = _Pressing_Condition.PressingCondition },
                            new DataView_PressingCondition { No = 2,PressingCondition = Condition }
                        };
                    List_Pressing_condition.ItemsSource = Pressing_Condition;

                    tb_Force_Min.Text = "";
                    tb_Force_Max.Text = "";
                    tb_Position_Min.Text = "";
                    tb_Position_Max.Text = "";
                    tb_Press_PositionDistance.Text = "";
                    tb_Press_Force.Text = "";
                    tb_Press_Velocity.Text = "";
                    tb_Press_Time.Text = "";
                    //cbb_Pressing_condition.SelectedIndex = -1;
                    //cbb_step.SelectedIndex = -1;
                    if (Global.Function2[0].Press_Condition != null)
                    {
                        Global.Function1[0].Mode = 0;
                        Global.Function1[0].Press_Condition = "";
                        Global.Function1[0].Press_Pos = 0;
                        Global.Function1[0].Press_Force = 0;
                        Global.Function1[0].Press_Vel = 0;
                        Global.Function1[0].Press_Time = 0;
                        Global.Function1[0].End_Max_Force_Limit = 0;
                        Global.Function1[0].End_Min_Force_Limit = 0;
                        Global.Function1[0].End_Max_Pos_Limit = 0;
                        Global.Function1[0].End_Min_Pos_Limit = 0;
                    }
                }
            }
        }
        private void Fill_ID(string json, ComboBox ComboBox)
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



            }
        }
        private static float Fill_Jig(string path, string id)
        {
            string jsons = File.ReadAllText(path);
            int flag = 0;
            if (jsons.Length > 0)
            {
                JArray jsonArray = JArray.Parse(jsons);
                foreach (JObject obj in jsonArray)
                {
                    if ((string)obj["ID"] == id)
                    {
                        return (float)obj["Thickness"];
                        flag = 1;
                        break;
                    }

                }
                if (flag == 0)
                {
                    MessageBox.Show("Jig chưa được chọn, Vui lòng chọn mã Jig");
                }

            }

            return -1;
        }
        private static int Fill_CBB_Press_Codition(string valueToCheck, ComboBox comboBox)
        {
            // Kiểm tra xem giá trị có trong ComboBox hay không
            int cbb_Select_intdex = -1;
            if (comboBox.Items.Contains(valueToCheck))
            {
                //   MessageBox.Show($"Giá trị '{valueToCheck}' có trong ComboBox.");
                comboBox.SelectedItem = valueToCheck;
                switch (valueToCheck)
                {

                    case "Positon":
                        cbb_Select_intdex = 0;
                        break;
                    case "Force":
                        cbb_Select_intdex = 1;
                        break;
                    case "Distance":
                        cbb_Select_intdex = 2;
                        break;
                    case "Force Position":
                        cbb_Select_intdex = 3;
                        break;
                    case "Force Distance":
                        cbb_Select_intdex = 4;
                        break;

                    default:
                        cbb_Select_intdex = -1;
                        break;
                }
            }
            return cbb_Select_intdex;
        }
        private static float CheckMode(ComboBox comboBox)
        {
            if (comboBox.SelectedItem != null)
            {
                switch (comboBox.SelectedItem.ToString())
                {

                    case "Positon":
                        return 1;
                        break;
                    case "Force":
                        return 2;
                        break;
                    case "Distance":
                        return 3;
                        break;
                    case "Force Position":
                        return 4;
                        break;
                    case "Force Distance":
                        return 5;
                        break;

                }
            }

            return -1;
        }
        private void Save_Parameter(float step)
        {
            try
            {
                if (step == 1)
                {
                    Global.Function1[0].Mode = CheckMode(cbb_Pressing_condition);
                    if (cbb_Pressing_condition.SelectedItem.ToString() != null & cbb_Pressing_condition.SelectedItem.ToString() != "")
                    {
                        Global.Function1[0].Press_Condition = cbb_Pressing_condition.SelectedValue.ToString();
                    }
                    else
                    {
                        Global.Function1[0].Press_Condition = "";
                    }
                    Global.Function1[0].Press_Pos = float.Parse(tb_Press_PositionDistance.Text);
                    Global.Function1[0].Press_Force = float.Parse(tb_Press_Force.Text);
                    Global.Function1[0].Press_Vel = float.Parse(tb_Press_Velocity.Text);
                    Global.Function1[0].Press_Time = float.Parse(tb_Press_Time.Text);
                    Global.Function1[0].End_Max_Force_Limit = float.Parse(tb_Force_Max.Text);
                    Global.Function1[0].End_Min_Force_Limit = float.Parse(tb_Force_Min.Text);
                    Global.Function1[0].End_Max_Pos_Limit = float.Parse(tb_Position_Max.Text);
                    Global.Function1[0].End_Min_Pos_Limit = float.Parse(tb_Position_Min.Text);
                }
                if (step == 2)
                {
                    Global.Function2[0].Mode = CheckMode(cbb_Pressing_condition);
                    if (cbb_Pressing_condition.SelectedItem.ToString() != null & cbb_Pressing_condition.SelectedItem.ToString() != "")
                    {
                        Global.Function2[0].Press_Condition = cbb_Pressing_condition.SelectedValue.ToString();
                    }
                    else
                    {
                        Global.Function2[0].Press_Condition = "";
                    }
                    Global.Function2[0].Press_Pos = float.Parse(tb_Press_PositionDistance.Text);
                    Global.Function2[0].Press_Force = float.Parse(tb_Press_Force.Text);
                    Global.Function2[0].Press_Vel = float.Parse(tb_Press_Velocity.Text);
                    Global.Function2[0].Press_Time = float.Parse(tb_Press_Time.Text);
                    Global.Function2[0].End_Max_Force_Limit = float.Parse(tb_Force_Max.Text);
                    Global.Function2[0].End_Min_Force_Limit = float.Parse(tb_Force_Min.Text);
                    Global.Function2[0].End_Max_Pos_Limit = float.Parse(tb_Position_Max.Text);
                    Global.Function2[0].End_Min_Pos_Limit = float.Parse(tb_Position_Min.Text);
                }
            }
            catch { }
          
        }

        private void Save_Model()
        {
            System.DateTime dateTime = System.DateTime.Now;
            string formattedDate = dateTime.ToString("dd/MM/yy");
            string formattedtime = dateTime.ToString("HH:mm:ss");
            string ID = formattedDate.Replace("/", "") + formattedtime.Replace(":", "");
            List_Model List_Model = new List_Model();
            DataFunC Data_Func1 = new DataFunC();
            DataFunC Data_Func2 = new DataFunC();
            List_Model.Model = tb_Model.Text;
            List_Model.ID_Rotor = tb_Rotor.Text;
            List_Model.ID_Shaft = tb_Shaft.Text;
            List_Model.ID_Bearings_Up = tb_BearingU.Text;
            List_Model.ID_Bearings_Down = tb_Shaft.Text;
            List_Model.Jig_Up = cbb_JigU.SelectedItem.ToString();
            List_Model.Jig_Mid = cbb_JigM.SelectedItem.ToString();
            List_Model.Jig_Down = cbb_JigD.SelectedItem.ToString();
            List_Model.Thickness_Jig_Up = Fill_Jig(path.Jig_Up, cbb_JigU.SelectedValue.ToString());
            List_Model.Thickness_Jig_Down = Fill_Jig(path.Jig_Down, cbb_JigD.SelectedValue.ToString());
            List_Model.Origin_Position= float.Parse(tb_Origin_PST.Text);
            List_Model.Origin_Velocity = float.Parse(tb_Origin_Velo.Text);
            List_Model.Standby_Position = float.Parse(tb_PST_Standby.Text);
            List_Model.Standby_Time = float.Parse(tb_Standby_Time.Text);
            List_Model.Standby_Velocity = float.Parse(tb_Standby_Velocity.Text);
            List_Model.Data_Func1 = Global.Function1;
            List_Model.Data_Func2 = Global.Function2;

            string list_Model_Json = JsonConvert.SerializeObject(List_Model);
            try
            {
                string json = File.ReadAllText(path.Model);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Model_Temp[]>(json, options);
                float flag = 0;
                foreach (var item in data)
                {

                    if (item.Model == tb_Model.Text)
                    {

                        item.Model = tb_Model.Text;
                        item.ID_Rotor = tb_Rotor.Text;
                        item.ID_Shaft = tb_Shaft.Text;
                        item.ID_Bearings_Up = tb_BearingU.Text;
                        item.ID_Bearings_Down = tb_BearingD.Text;
                        item.Jig_Up = cbb_JigU.SelectedValue.ToString();
                        item.Jig_Mid = cbb_JigM.SelectedValue.ToString();
                        item.Jig_Down = cbb_JigD.SelectedValue.ToString();
                        item.Thickness_Jig_Up = Fill_Jig(path.Jig_Up, cbb_JigU.SelectedValue.ToString());
                        item.Thickness_Jig_Down = Fill_Jig(path.Jig_Down, cbb_JigD.SelectedValue.ToString());
                        item.Height_Stand = float.Parse(tb_Stand_Height.Text);

                        item.Data_Func1 = Global.Function1;
                        item.Data_Func2 = Global.Function2;
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(path.Model, newJsonString);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (json.Length < 10)
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + list_Model_Json + "\n]";
                        File.WriteAllText(path.Model, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }
                    else
                    {
                        json = json.Remove(json.Length - 1);
                        json = json + ",\n" + list_Model_Json + "\n]";
                        File.WriteAllText(path.Model, json);
                        MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                    }

                }
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine("Log", formattedDate.Replace("/", "_") + "_Model_Log.txt"), true)) // true để thêm
                {
                    writer.WriteLine(list_Model_Json);
                }
            }
            catch (Exception ex)

            {
                string json_;
                json_ = "[" + list_Model_Json + "\n]";
                File.WriteAllText(path.Model, json_);
                MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
                //  MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");
            }
            Common.Load_View_Model(List_Models);
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
                comboBox.SelectedIndex = -1;
            }
        }
        private void Fill_Value_Mode()
        {
            try
            {
                bool flag = false;
                string json = File.ReadAllText(path.Model);
                if (json.Length > 0)
                {
                    List<List_Model> jsonArray = JsonConvert.DeserializeObject<List<List_Model>>(json);
                    //JArray jsonArray = JArray.Parse(json);
                    foreach (var obj in jsonArray)
                    {
                        if ((string)obj.Model == tb_Model.Text)
                        {
                            tb_Model.Text = obj.Model;
                            tb_Rotor.Text = obj.ID_Rotor;
                            tb_Shaft.Text = obj.ID_Rotor;
                            tb_BearingU.Text = obj.ID_Rotor;
                            tb_BearingD.Text = obj.ID_Rotor;
                            CheckValueInComboBox(obj.Jig_Up.ToString(), cbb_JigU);
                            CheckValueInComboBox(obj.Jig_Mid.ToString(), cbb_JigM);
                            CheckValueInComboBox(obj.Jig_Down.ToString(), cbb_JigD);
                            tb_Stand_Height.Text = string.Format("{0:F2}", obj.Height_Stand);
                            Global.Function1.AddRange(obj.Data_Func1);
                            Global.Function2.AddRange(obj.Data_Func2);
                            tb_Origin_PST.Text= string.Format("{0:F2}", obj.Origin_Position);
                            tb_Origin_Velo.Text = string.Format("{0:F2}", obj.Origin_Velocity);
                            tb_PST_Standby.Text = string.Format("{0:F2}", obj.Standby_Position);
                            tb_Standby_Velocity.Text = string.Format("{0:F2}", obj.Standby_Velocity);
                            tb_Standby_Time.Text = string.Format("{0:F2}", obj.Standby_Time);

                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        Clear_Model();
                    }
                    flag = false;
                }
            }
            catch (Exception ex)

            {

            }
        }

        private void Del_Model()
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã Model: " + tb_Model.Text + " ?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes & tb_Rotor.Text.Length > 0 & tb_Shaft.Text.Length > 0)
            {

                string json = File.ReadAllText(path.Model);
                var options = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
                var data = System.Text.Json.JsonSerializer.Deserialize<List_Model_Temp[]>(json, options);

                var newData = new List<List_Model_Temp>();

                foreach (var item in data)
                {
                    if (item.Model != tb_Model.Text)
                    {
                        newData.Add(item);
                    }
                }
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string newJsonString = System.Text.Json.JsonSerializer.Serialize(newData, jsonOptions);
                // Write back to file
                File.WriteAllText(path.Model, newJsonString);
                Common.Load_View_Model(List_Models);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + tb_Model.Text + " cần xóa");
            }

        }



        private void Click_bt_Del_Model(object sender, RoutedEventArgs e)
        {


        }

        private void Click_bt_Import_model(object sender, RoutedEventArgs e)
        {
            excel.Import_Model_Filepath();
            Common.Load_View_Model(List_Models);
        }

        private void Click_bt_Export_model(object sender, RoutedEventArgs e)
        {
            excel.Export_Model_File("Template_Model", path.Model);
        }

        private void Click_bt_Template_model(object sender, RoutedEventArgs e)
        {
            var Result = Excel.Coppy_File("Template_Model");
            MessageBox.Show("Đã tạo Template thành công.");
        }

        private void Click_bt_Save_model(object sender, RoutedEventArgs e)
        {
            Save_Model();
        }
    }
}
