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
using System.Windows.Threading;
using System.Data;
using System.Diagnostics;

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
        public  bool IsForcus ;

        public int Select_datagrid=0;
        private DispatcherTimer timer;
        private static bool flag2;
        private bool keyboardIsOpen = false;
        public Model()
        {
            InitializeComponent();
            Loaded += Model_Loaded;  // Thêm sự kiện Loaded
            Unloaded += Model_Unloaded;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void Model_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Start();
            Fill_ID(File.ReadAllText(path.Bearings_Up), cbb_BearingsU);
            Fill_ID(File.ReadAllText(path.Bearings_Down), cbb_BearingsD);
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
                textBox.KeyDown += TextBox_KeyDown;
            }
            var Pressing_Condition = new List<DataView_PressingCondition>
            {
                new DataView_PressingCondition { No = 1,PressingCondition = "---" },
                new DataView_PressingCondition { No = 2,PressingCondition = "---" }
            };
            // Gán dữ liệu cho DataGrid
            List_Pressing_condition.ItemsSource = Pressing_Condition;

            Init_data();
            Clear_Model();
            Global.Clear_Auto = true;
        }
        private void Model_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var dataGrid in Common.FindVisualChildren<DataGrid>(this))
            {
                dataGrid.SelectionChanged -= Datagrid_SelectionChanged;
            }
            foreach (var Combobox in Common.FindVisualChildren<ComboBox>(this))
            {
                Combobox.SelectionChanged -= Combobox_Changed;
            }
            foreach (var textBox in Common.FindVisualChildren<TextBox>(this))
            {
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.LostFocus -= TextBox_LostFocus;
                textBox.KeyDown -= TextBox_KeyDown;
            }
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Update_Data();
                });

                CheckKeyboardStatus();

            }
            catch (Exception ex)
            {
                Common.Log_err(ex.ToString());
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            string input = textBox.Text;
            if(Is_String(textboxName, "tb_num"))
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "0";
                    textBox.SelectAll(); // Select all text to allow easy replacement
                }
                if (!double.TryParse(textBox.Text, out _) & (textBox.Text != ""))
                {
                    MessageBox.Show("Vui Lòng nhập lại dữ liệu kiểu số");
                    textBox.Text = "";
                }
                if (input.Contains(","))
                {
                    input = input.Replace(",", ".");
                    // Cập nhật lại giá trị trong TextBox
                    textBox.Text = input;
                    // Đặt con trỏ về cuối
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }    


            if (textboxName == "tb_Model")
            {
                Fill_Value_Mode();
                var selectedRow = List_Pressing_condition.SelectedItem as DataView_PressingCondition;
                if (selectedRow != null)
                {
                    // Lấy dữ liệu từ hàng được chọn
                    //  cbb_step.SelectedItem = selectedRow.No.ToString();
                    Fill_CBB_Press_Codition(selectedRow.PressingCondition.ToString(), selectedRow.No.ToString());
                }
            }    

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            IsForcus = true;
            if (!flag2 & !Global.NumPad_Visiable & Is_String(textboxName, "tb_num") & textBox.IsReadOnly== false)
            {
                NumPad numberPad = new NumPad(textBox);

                Point position = this.PointToScreen(new Point(0, 0));

                numberPad.Left = position.X + 600;
                numberPad.Top = position.Y + 360;
                numberPad.Show(); // Hiển thị cửa sổ như hộp thoại
                                  // MessageBox.Show("TextBox_GotFocus2");
                flag2 = true;
                // Common.Open_KeyBoard(); 
            }

        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            if (e.Key == Key.Enter)
            {
                if(textboxName == "tb_Model")
                {
                    tb_Rotor.Focusable = true;
                    tb_Rotor.Focus();
                }    
                else if (textboxName == "tb_Rotor")
                {
                    tb_Shaft.Focusable = true;
                    tb_Shaft.Focus();
                }
                else
                {
                    Keyboard.ClearFocus();
                    FocusBorder.Focusable = true;
                    FocusBorder.Focus();
                }
                // Mất focus khi nhấn Enter


            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string textboxName = textBox.Name;
            int selectedValue;
            if (cbb_step.SelectedItem != null & cbb_Pressing_condition.SelectedItem != null)
            {

                var selectedcondition = (ComboBoxItem)cbb_Pressing_condition.SelectedItem;
                var selectedItem = (ComboBoxItem)cbb_step.SelectedItem;
                if (int.TryParse(selectedItem.Content.ToString(), out selectedValue))
                {
                    Save_Parameter(selectedValue, selectedcondition.Content.ToString());
                }
            }
            if (Is_String(textboxName, "tb_num"))
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = "0";
                }
            }
            Global.Model_Press_Pos1 = Caculate_Position_Distance(Global.Function1[0].Mode, Global.Model_Thickness_Bearings_D.ToString(), tb_num_Distance_Bearings_After.Text,
    Global.Model_Thickness_Bearings_U.ToString(), tb_num_Distance_Bearings_Before.Text, tb_num_Ofset.Text, tb_num_PST_Standby.Text);
            Global.Model_Press_Pos2 = Caculate_Position_Distance(Global.Function2[0].Mode, Global.Model_Thickness_Bearings_D.ToString(), tb_num_Distance_Bearings_After.Text,
    Global.Model_Thickness_Bearings_U.ToString(), tb_num_Distance_Bearings_Before.Text, tb_num_Ofset.Text, tb_num_PST_Standby.Text);
            
            flag2 = false;
            IsForcus = false;

        }
        private void Datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = (DataGrid)sender;
            string datagridname = datagrid.Name;

            if (datagridname == "List_Pressing_condition")
            {
                var selectedRow = List_Pressing_condition.SelectedItem as DataView_PressingCondition;
                if (selectedRow != null)
                {
                    // Lấy dữ liệu từ hàng được chọn
                    //  cbb_step.SelectedItem = selectedRow.No.ToString();

                    Select_datagrid = 1;
                    Fill_CBB_Press_Codition(selectedRow.PressingCondition.ToString(), selectedRow.No.ToString());
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
        private void Combobox_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string comboboxname = comboBox.Name;
            var selectedcondition = (ComboBoxItem)cbb_Pressing_condition.SelectedItem;
            int selectedValue;
            var datagrid_selectedRow = List_Pressing_condition.SelectedItem as DataView_PressingCondition;
            if ((comboboxname == "cbb_step" || comboboxname == "cbb_Pressing_condition") & Select_datagrid != 0)
            {
                Select_datagrid++;
            }

            if (cbb_step.SelectedItem != null & cbb_Pressing_condition.SelectedItem != null)
            {
                var selectedItem = (ComboBoxItem)cbb_step.SelectedItem;
                if (int.TryParse(selectedItem.Content.ToString(), out selectedValue))
                {

                        if (Select_datagrid == 0)
                        {
                            if ((selectedValue == 1 & selectedcondition.Content.ToString() != Global.Function1[0].Press_Condition) || (selectedValue == 2 & selectedcondition.Content.ToString() != Global.Function2[0].Press_Condition))
                            {
                                Edit_Condition(selectedValue, selectedcondition.Content.ToString());
                            }

                        }
                        else if (datagrid_selectedRow!= null)
                        {
                        if (datagrid_selectedRow.PressingCondition.ToString() == "---")
                        {
                            if ((selectedValue == 1 & selectedcondition.Content.ToString() != Global.Function1[0].Press_Condition) || (selectedValue == 2 & selectedcondition.Content.ToString() != Global.Function2[0].Press_Condition))
                            {
                                Edit_Condition(selectedValue, selectedcondition.Content.ToString());
                            }
                        }    


                        }


                    if (Select_datagrid == 3)
                    {
                        Select_datagrid = 0;
                    }


                }
            }
            if (comboboxname == "cbb_JigU" & comboBox.SelectedItem != null)
            {
                Global.Model_Thickness_Jig_Up = Fill_Jig(path.Jig_Up, cbb_JigU.SelectedValue.ToString());

            }
            if (comboboxname == "cbb_JigD" & comboBox.SelectedItem != null)
            {
                Global.Model_Thickness_Jig_Down = Fill_Jig(path.Jig_Down, cbb_JigD.SelectedValue.ToString());
            }
            if (comboboxname == "cbb_BearingsU" & comboBox.SelectedItem != null)
            {
                
                Global.Model_Thickness_Bearings_U = Fill_Jig(path.Bearings_Up, cbb_BearingsU.SelectedValue.ToString());
            }
            if (comboboxname == "cbb_BearingsD" & comboBox.SelectedItem != null)
            {

                Global.Model_Thickness_Bearings_D = Fill_Jig(path.Bearings_Down, cbb_BearingsD.SelectedValue.ToString());
            }
            Global.Model_Press_Pos1 = Caculate_Position_Distance(Global.Function1[0].Mode, Global.Model_Thickness_Bearings_D.ToString(), tb_num_Distance_Bearings_After.Text,
 Global.Model_Thickness_Bearings_U.ToString(), tb_num_Distance_Bearings_Before.Text, tb_num_Ofset.Text, tb_num_PST_Standby.Text);
            Global.Model_Press_Pos2 = Caculate_Position_Distance(Global.Function2[0].Mode, Global.Model_Thickness_Bearings_D.ToString(), tb_num_Distance_Bearings_After.Text,
    Global.Model_Thickness_Bearings_U.ToString(), tb_num_Distance_Bearings_Before.Text, tb_num_Ofset.Text, tb_num_PST_Standby.Text);

        }
        private void Update_Data()
        {
            if (!IsForcus)
            {
                int selectedValue;

                if (cbb_step.SelectedItem != null & cbb_Pressing_condition.SelectedItem != null)
                {
                    var selectedItem = (ComboBoxItem)cbb_step.SelectedItem;
                    if (int.TryParse(selectedItem.Content.ToString(), out selectedValue))
                    {
                        Update_Condition_Press(selectedValue);
                    }
                }
            }
                    }
        private void Update_Condition_Press( int step)
        {
            if (step == 1)
            {

                tb_num_Force_Max.Text = string.Format("{0:F2}", Global.Function1[0].End_Max_Force_Limit);
                tb_num_Force_Min.Text = string.Format("{0:F2}", Global.Function1[0].End_Min_Force_Limit);
                tb_num_Position_Max.Text = string.Format("{0:F2}", Global.Function1[0].End_Max_Pos_Limit);
                tb_num_Position_Min.Text = string.Format("{0:F2}", Global.Function1[0].End_Min_Pos_Limit);
                tb_num_Press_PositionDistance.Text = string.Format("{0:F2}", Global.Model_Press_Pos1);
                tb_num_Press_Force.Text = string.Format("{0:F2}", Global.Function1[0].Press_Force);
                tb_num_Press_Velocity.Text = string.Format("{0:F2}", Global.Function1[0].Press_Vel);
                tb_num_Press_Time.Text = string.Format("{0:F2}", Global.Function1[0].Press_Time);
            }
            if (step == 2)
            {

                tb_num_Force_Max.Text = string.Format("{0:F2}", Global.Function2[0].End_Max_Force_Limit);
                tb_num_Force_Min.Text = string.Format("{0:F2}", Global.Function2[0].End_Min_Force_Limit);
                tb_num_Position_Max.Text = string.Format("{0:F2}", Global.Function2[0].End_Max_Pos_Limit);
                tb_num_Position_Min.Text = string.Format("{0:F2}", Global.Function2[0].End_Min_Pos_Limit);
                tb_num_Press_PositionDistance.Text = string.Format("{0:F2}", Global.Model_Press_Pos2);
                tb_num_Press_Force.Text = string.Format("{0:F2}", Global.Function2[0].Press_Force);
                tb_num_Press_Velocity.Text = string.Format("{0:F2}", Global.Function2[0].Press_Vel);
                tb_num_Press_Time.Text = string.Format("{0:F2}", Global.Function2[0].Press_Time);
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
            Global.Function1[0].Press_Condition = "---";
            Global.Function1[0].Press_Force = 0;
            Global.Function1[0].Press_Vel = 0;
            Global.Function1[0].Press_Time = 0;
            Global.Function1[0].End_Max_Force_Limit = 0;
            Global.Function1[0].End_Min_Force_Limit = 0;
            Global.Function1[0].End_Max_Pos_Limit = 0;
            Global.Function1[0].End_Min_Pos_Limit = 0;
            Global.Function2[0].Mode = 0;
            Global.Function2[0].Press_Condition = "---";
            Global.Function2[0].Press_Force = 0;
            Global.Function2[0].Press_Vel = 0;
            Global.Function2[0].Press_Time = 0;
            Global.Function2[0].End_Max_Force_Limit = 0;
            Global.Function2[0].End_Min_Force_Limit = 0;
            Global.Function2[0].End_Max_Pos_Limit = 0;
            Global.Function2[0].End_Min_Pos_Limit = 0;
            //
            tb_num_Origin_PST.Text = "0";
            tb_num_Origin_Velo.Text = "0";
            tb_num_PST_Standby.Text = "0";
            tb_num_Standby_Velocity.Text = "0";
            tb_num_Standby_Time.Text = "0";
            tb_num_Force_Min.Text = "";
            tb_num_Force_Max.Text = "";
            tb_num_Position_Min.Text = "";
            tb_num_Position_Max.Text = "";
            tb_num_Press_PositionDistance.Text = "";
            tb_num_Press_Force.Text = "";
            tb_num_Press_Velocity.Text = "";
            tb_num_Press_Time.Text = "";
            cbb_Pressing_condition.SelectedIndex = -1;
            cbb_step.SelectedIndex = -1;
            //
            tb_Rotor.Text = "";
            tb_Shaft.Text = "";
            cbb_BearingsU.SelectedIndex = -1;
            cbb_BearingsD.SelectedIndex = -1;
            cbb_JigU.SelectedIndex = -1;
            cbb_JigM.SelectedIndex = -1;
            cbb_JigD.SelectedIndex = -1;
            tb_num_Stand_Height.Text = "0";
            tb_num_Distance_Bearings_After.Text = "0";
            tb_num_Distance_Bearings_Before.Text = "0";
            tb_num_Ofset.Text = "0";
            Global.Model_Thickness_Bearings_D = 0;
            Global.Model_Thickness_Bearings_U = 0;
            Global.Model_Thickness_Jig_Up = 0;
            Global.Model_Thickness_Jig_Down = 0;
            Global.Model_Press_Pos1 = 0;
            Global.Model_Press_Pos2 = 0;
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
                    tb_num_Force_Min.Text = "";
                    tb_num_Force_Max.Text = "";
                    tb_num_Position_Min.Text = "";
                    tb_num_Position_Max.Text = "";
                    tb_num_Press_PositionDistance.Text = "";
                    tb_num_Press_Force.Text = "";
                    tb_num_Press_Velocity.Text = "";
                    tb_num_Press_Time.Text = "";
                    cbb_Pressing_condition.SelectedIndex = -1;
                    cbb_step.SelectedIndex = -1;
                    Global.Function1[0].Mode = 0;
                    Global.Function1[0].Press_Condition = "";
                    Global.Function1[0].Press_Force = 0;
                    Global.Function1[0].Press_Vel = 0;
                    Global.Function1[0].Press_Time = 0;
                    Global.Function1[0].End_Max_Force_Limit = 0;
                    Global.Function1[0].End_Min_Force_Limit = 0;
                    Global.Function1[0].End_Max_Pos_Limit = 0;
                    Global.Function1[0].End_Min_Pos_Limit = 0;
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

                    tb_num_Force_Min.Text = "";
                    tb_num_Force_Max.Text = "";
                    tb_num_Position_Min.Text = "";
                    tb_num_Position_Max.Text = "";
                    tb_num_Press_PositionDistance.Text = "";
                    tb_num_Press_Force.Text = "";
                    tb_num_Press_Velocity.Text = "";
                    tb_num_Press_Time.Text = "";
                    cbb_Pressing_condition.SelectedIndex = -1;
                    cbb_step.SelectedIndex = -1;
                    Global.Function2[0].Mode = 0;
                    Global.Function2[0].Press_Condition = "";
                    Global.Function2[0].Press_Force = 0;
                    Global.Function2[0].Press_Vel = 0;
                    Global.Function2[0].Press_Time = 0;
                    Global.Function2[0].End_Max_Force_Limit = 0;
                    Global.Function2[0].End_Min_Force_Limit = 0;
                    Global.Function2[0].End_Max_Pos_Limit = 0;
                    Global.Function2[0].End_Min_Pos_Limit = 0;
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
                    tb_num_Force_Min.Text = "";
                    tb_num_Force_Max.Text = "";
                    tb_num_Position_Min.Text = "";
                    tb_num_Position_Max.Text = "";
                    tb_num_Press_PositionDistance.Text = "";
                    tb_num_Press_Force.Text = "";
                    tb_num_Press_Velocity.Text = "";
                    tb_num_Press_Time.Text = "";
                    //cbb_Pressing_condition.SelectedIndex = -1;
                    //cbb_step.SelectedIndex = -1;
                    if (Global.Function1[0].Press_Condition != null)
                    {
                        Global.Function1[0].Mode = 0;
                        Global.Function1[0].Press_Condition = "---";
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

                    tb_num_Force_Min.Text = "";
                    tb_num_Force_Max.Text = "";
                    tb_num_Position_Min.Text = "";
                    tb_num_Position_Max.Text = "";
                    tb_num_Press_PositionDistance.Text = "";
                    tb_num_Press_Force.Text = "";
                    tb_num_Press_Velocity.Text = "";
                    tb_num_Press_Time.Text = "";
                    //cbb_Pressing_condition.SelectedIndex = -1;
                    //cbb_step.SelectedIndex = -1;
                    if (Global.Function2[0].Press_Condition != null)
                    {
                        Global.Function2[0].Mode = 0;
                        Global.Function2[0].Press_Condition = "---";
                        Global.Function2[0].Press_Force = 0;
                        Global.Function2[0].Press_Vel = 0;
                        Global.Function2[0].Press_Time = 0;
                        Global.Function2[0].End_Max_Force_Limit = 0;
                        Global.Function2[0].End_Min_Force_Limit = 0;
                        Global.Function2[0].End_Max_Pos_Limit = 0;
                        Global.Function2[0].End_Min_Pos_Limit = 0;
                    }
                }
            }
            CheckMode();
        }
        private void Load_CBB()
        {
            if (List_Pressing_condition.ItemsSource is List<DataView_PressingCondition> Pressing_Condition_old && Pressing_Condition_old.Count > 1)
            {
                DataView_PressingCondition _Pressing_Condition1 = Pressing_Condition_old[0];
                DataView_PressingCondition _Pressing_Condition2 = Pressing_Condition_old[1];
                if (_Pressing_Condition1.PressingCondition != null & _Pressing_Condition1.PressingCondition != "" & _Pressing_Condition1.PressingCondition != "---")
                {
                    Fill_CBB_Press_Codition(_Pressing_Condition1.PressingCondition, "1");
                }
                else if (_Pressing_Condition2.PressingCondition != null & _Pressing_Condition2.PressingCondition != "" & _Pressing_Condition2.PressingCondition != "---")
                {
                    Fill_CBB_Press_Codition(_Pressing_Condition2.PressingCondition, "2");
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
            try
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

                            flag = 1;
                            return (float)obj["Thickness"];
                        }

                    }
                    if (flag == 0)
                    {
                        MessageBox.Show("Jig chưa được chọn, Vui lòng chọn mã Jig");
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return -1;
        }
        private void Fill_CBB_Press_Codition(string condition, string step )
        {
            // Kiểm tra xem giá trị có trong ComboBox hay không
            //  int cbb_Select_intdex = -1;
            foreach (ComboBoxItem item in cbb_Pressing_condition.Items)
            {
                if (item.Content.ToString() == condition)
                {
                    cbb_Pressing_condition.SelectedItem = item;
                    break;
                }
            }
            foreach (ComboBoxItem item in cbb_step.Items)
            {
                if (item.Content.ToString() == step)
                {
                    cbb_step.SelectedItem = item;
                    break;
                }
            }
            CheckMode();
            //    comboBox.SelectedIndex = -1;
            // return cbb_Select_intdex;
        }
        private static float CheckMode(ComboBox comboBox)
        {
            if (comboBox.SelectedItem != null)
            {
                var selectedItem = (ComboBoxItem)comboBox.SelectedItem;
                switch (selectedItem.Content.ToString())
                {

                    case "Position":
                        return 1;
                    case "Force":
                        return 2;
                    case "Distance":
                        return 3;
                    case "Force Position":
                        return 4;
                    case "Force Distance":
                        return 5;

                }
            }

            return -1;
        }
        private void CheckMode()
        {
            if (cbb_Pressing_condition.SelectedItem != null)
            {
                var selectedItem = (ComboBoxItem)cbb_Pressing_condition.SelectedItem;
                tb_num_Press_PositionDistance.IsReadOnly = true;
                tb_num_Press_Force.IsReadOnly = true;
                tb_num_Press_Time.IsReadOnly = true;
                tb_num_Press_Velocity.IsReadOnly = true;
                tb_num_Position_Max.IsReadOnly = true;
                tb_num_Position_Min.IsReadOnly = true;
                tb_num_Force_Max.IsReadOnly = true;
                tb_num_Force_Min.IsReadOnly = true;
                switch (selectedItem.Content.ToString())
                {
                   
                    case "Position":
                        tb_num_Press_Time.IsReadOnly = false;
                        tb_num_Press_Velocity.IsReadOnly = false;
                        tb_num_Force_Max.IsReadOnly = false;
                        tb_num_Force_Min.IsReadOnly = false;
                        break;
                    case "Force":
                        tb_num_Press_Force.IsReadOnly = false;
                        tb_num_Press_Time.IsReadOnly = false;
                        tb_num_Press_Velocity.IsReadOnly = false;
                        tb_num_Position_Max.IsReadOnly = false;
                        tb_num_Position_Min.IsReadOnly = false;
                        break;
                    case "Distance":
                        tb_num_Press_Time.IsReadOnly = false;
                        tb_num_Press_Velocity.IsReadOnly = false;
                        tb_num_Position_Max.IsReadOnly = false;
                        tb_num_Position_Min.IsReadOnly = false;
                        tb_num_Force_Max.IsReadOnly = false;
                        tb_num_Force_Min.IsReadOnly = false;
                        break;
                    case "Force Position":
                        tb_num_Press_Force.IsReadOnly = false;
                        tb_num_Press_Time.IsReadOnly = false;
                        tb_num_Press_Velocity.IsReadOnly = false;
                        break;
                    case "Force Distance":
                        tb_num_Press_Force.IsReadOnly = false;
                        tb_num_Press_Time.IsReadOnly = false;
                        tb_num_Press_Velocity.IsReadOnly = false;
                        tb_num_Position_Max.IsReadOnly = false;
                        tb_num_Position_Min.IsReadOnly = false;
                        break;

                }
            }
        }
        private void Save_Parameter(float step, string PressCodition)
        {
            try
            {
                if (step == 1)
                {
                    Global.Function1[0].Mode = CheckMode(cbb_Pressing_condition);
                    if (cbb_Pressing_condition.SelectedItem.ToString() != null & cbb_Pressing_condition.SelectedItem.ToString() != "")
                    {
                        Global.Function1[0].Press_Condition = PressCodition;
                    }
                    else
                    {
                        Global.Function1[0].Press_Condition = "";
                    }

                    Global.Function1[0].Press_Force = float.Parse(tb_num_Press_Force.Text);
                    Global.Function1[0].Press_Vel = float.Parse(tb_num_Press_Velocity.Text);
                    Global.Function1[0].Press_Time = float.Parse(tb_num_Press_Time.Text);
                    Global.Function1[0].End_Max_Force_Limit = float.Parse(tb_num_Force_Max.Text);
                    Global.Function1[0].End_Min_Force_Limit = float.Parse(tb_num_Force_Min.Text);
                    Global.Function1[0].End_Max_Pos_Limit = float.Parse(tb_num_Position_Max.Text);
                    Global.Function1[0].End_Min_Pos_Limit = float.Parse(tb_num_Position_Min.Text);
                }
                if (step == 2)
                {
                    Global.Function2[0].Mode = CheckMode(cbb_Pressing_condition);
                    if (cbb_Pressing_condition.SelectedItem.ToString() != null & cbb_Pressing_condition.SelectedItem.ToString() != "")
                    {
                        Global.Function2[0].Press_Condition = PressCodition;
                    }
                    else
                    {
                        Global.Function2[0].Press_Condition = "";
                    }

                    Global.Function2[0].Press_Force = float.Parse(tb_num_Press_Force.Text);
                    Global.Function2[0].Press_Vel = float.Parse(tb_num_Press_Velocity.Text);
                    Global.Function2[0].Press_Time = float.Parse(tb_num_Press_Time.Text);
                    Global.Function2[0].End_Max_Force_Limit = float.Parse(tb_num_Force_Max.Text);
                    Global.Function2[0].End_Min_Force_Limit = float.Parse(tb_num_Force_Min.Text);
                    Global.Function2[0].End_Max_Pos_Limit = float.Parse(tb_num_Position_Max.Text);
                    Global.Function2[0].End_Min_Pos_Limit = float.Parse(tb_num_Position_Min.Text);
                }
            }
            catch { }
          
        }
        private static float Caculate_Position_Distance(float mode, string Thickness_BearingsD, string Distance_Bearings_After , string Thickness_BearingsU , string Distance_Bearings_Before, string ofset_Model , string standby_position)
        {

            float Position = Global.Height_Shaft_Press
                + (float)Data.ofset_Machine 
                - (float)Data.Height_Jig_Base 
                - Global.Model_Thickness_Jig_Down 
                - float.Parse(Thickness_BearingsD) 
                - float.Parse(Distance_Bearings_After) 
                - float.Parse(Thickness_BearingsU) 
                - Global.Model_Thickness_Jig_Up
                + float.Parse(ofset_Model);
            float Distance = Global.Height_Shaft_Press
                + (float)Data.ofset_Machine
                - (float)Data.Height_Jig_Base
                - Global.Model_Thickness_Jig_Down
                - float.Parse(Thickness_BearingsD)
                - float.Parse(Distance_Bearings_After)
                - float.Parse(Thickness_BearingsU)
                - Global.Model_Thickness_Jig_Up
                + float.Parse(ofset_Model)
                - float.Parse(standby_position);
            Global.Standby_Position = Global.Height_Shaft_Press
                + (float)Data.ofset_Machine
                - (float)Data.Height_Jig_Base
                - Global.Model_Thickness_Jig_Down
                - float.Parse(Thickness_BearingsD)
                - float.Parse(Distance_Bearings_Before)
                - float.Parse(Thickness_BearingsU)
                - Global.Model_Thickness_Jig_Up
                + float.Parse(ofset_Model);

            switch (mode)
            {

                case 1:
                    return Position;
                case 2:
                    return 0;
                case 3:
                    return Distance;
                case 4:
                    return Position;
                case 5:
                    return Position;

            }
            return 0;
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
            List_Model.ID_Bearings_Up = cbb_BearingsU.SelectedItem.ToString();
            List_Model.ID_Bearings_Down = cbb_BearingsD.SelectedItem.ToString();
            List_Model.Jig_Up = cbb_JigU.SelectedItem.ToString();
            List_Model.Jig_Mid = cbb_JigM.SelectedItem.ToString();
            List_Model.Jig_Down = cbb_JigD.SelectedItem.ToString();
            List_Model.Pre_press_Bearings_distance = float.Parse(tb_num_Distance_Bearings_Before.Text);
            List_Model.After_press_bearings_distance = float.Parse(tb_num_Distance_Bearings_After.Text);
            List_Model.Ofset_position1 = float.Parse(tb_num_Ofset.Text);
            List_Model.Ofset_position2 = 0;
            List_Model.Origin_Position= float.Parse(tb_num_Origin_PST.Text);
            List_Model.Origin_Velocity = float.Parse(tb_num_Origin_Velo.Text);
            List_Model.Standby_Position = float.Parse(tb_num_PST_Standby.Text);
            List_Model.Standby_Time = float.Parse(tb_num_Standby_Time.Text);
            List_Model.Standby_Velocity = float.Parse(tb_num_Standby_Velocity.Text);
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
                        item.ID_Bearings_Up = cbb_BearingsU.SelectedValue.ToString();
                        item.ID_Bearings_Down = cbb_BearingsD.SelectedValue.ToString();
                        item.Jig_Up = cbb_JigU.SelectedValue.ToString();
                        item.Jig_Mid = cbb_JigM.SelectedValue.ToString();
                        item.Jig_Down = cbb_JigD.SelectedValue.ToString();
                        item.Height_Stand = float.Parse(tb_num_Stand_Height.Text);
                        item.Pre_press_Bearings_distance = float.Parse(tb_num_Distance_Bearings_Before.Text);
                        item.After_press_bearings_distance = float.Parse(tb_num_Distance_Bearings_After.Text);
                        item.Ofset_position1 = float.Parse(tb_num_Ofset.Text);
                        item.Ofset_position2 = 0;
                        item.Origin_Position = float.Parse(tb_num_Origin_PST.Text);
                        item.Origin_Velocity = float.Parse(tb_num_Origin_Velo.Text);
                        item.Standby_Position = float.Parse(tb_num_PST_Standby.Text);
                        item.Standby_Time = float.Parse(tb_num_Standby_Time.Text);
                        item.Standby_Velocity = float.Parse(tb_num_Standby_Velocity.Text);
                        item.Data_Func1 = Global.Function1;
                        item.Data_Func2 = Global.Function2;
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        string newJsonString = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                        File.WriteAllText(path.Model, newJsonString);

                        Common.Log_Operation("Edit Model:  " + tb_Model.Text, path.Log_EN);
                        Common.Log_Operation("Sửa Model:  " + tb_Model.Text, path.Log_VN);
                        MessageBox.Show("Đã Lưu Thành Công");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {

                    json = json.Remove(json.Length - 1);
                    json = json + ",\n" + list_Model_Json + "\n]";
                    File.WriteAllText(path.Model, json);
                    MessageBox.Show("Đã Lưu Và Tạo Model Mới Thành Công");

                    Common.Log_Operation("Create Model:  " + tb_Model.Text, path.Log_EN);
                    Common.Log_Operation("Tạo Model:  " + tb_Model.Text, path.Log_VN);
                }

                List<Data_Log> data_Logs = new List<Data_Log>
                {
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Model:" + tb_Model.Text , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Stand_Height:" + tb_num_Stand_Height.Text , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Origin_Position:" + tb_num_Origin_PST.Text, Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Origin_Velocity:" + tb_num_Origin_Velo.Text , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Standby_Position:" + tb_num_PST_Standby.Text , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Standby_Velocity:" + tb_num_Standby_Velocity.Text , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Standby_Time:" + tb_num_Standby_Time.Text , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Mode1:" + Global.Function1[0].Mode , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Press_Force1:" + Global.Function1[0].Press_Force , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Press_Velocity1:" + Global.Function1[0].Press_Vel , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Press_Time1:" + Global.Function1[0].Press_Time, Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Max_Force1:" + Global.Function1[0].End_Max_Force_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Min_Force1:" + Global.Function1[0].End_Min_Force_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Max_Position1:" + Global.Function1[0].End_Max_Pos_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Min_Position1:" + Global.Function1[0].End_Min_Pos_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Mode2:" + Global.Function2[0].Mode , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Press_Force2:" + Global.Function2[0].Press_Force , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Press_Velocity2:" + Global.Function2[0].Press_Vel , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Press_Time2:" + Global.Function2[0].Press_Time, Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Max_Force2:" + Global.Function2[0].End_Max_Force_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Min_Force2:" + Global.Function2[0].End_Min_Force_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Max_Position2:" + Global.Function2[0].End_Max_Pos_Limit , Time = formattedDate +" "+formattedtime},
                    new Data_Log { No = 0, User = MainWindow.UserName, Log =  "Save Min_Position2:" + Global.Function2[0].End_Min_Pos_Limit , Time = formattedDate +" "+formattedtime}
                };

                // Chuyển danh sách sang định dạng JSON
                string json_Log = JsonConvert.SerializeObject(data_Logs, Formatting.Indented);
                Common.Log_Operation_Json(json_Log, path.Log);

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
                    
                    case "cbb_BearingsD":
                        Model_check = "Vòng bi dưới";
                        break;
                    case "cbb_BearingsU":
                        Model_check = "Vòng bi trên";
                        break;
                    case "cbb_JigU":
                        Model_check = "Jig trên";
                        break;
                    case "cbb_JigM":
                        Model_check = "Jig giữa";
                        break;
                    case "cbb_JigD":
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
                            tb_Shaft.Text = obj.ID_Shaft;
                            CheckValueInComboBox(obj.ID_Bearings_Up.ToString(), cbb_BearingsU);
                            CheckValueInComboBox(obj.ID_Bearings_Down.ToString(), cbb_BearingsD);
                            CheckValueInComboBox(obj.Jig_Up.ToString(), cbb_JigU);
                            CheckValueInComboBox(obj.Jig_Mid.ToString(), cbb_JigM);
                            CheckValueInComboBox(obj.Jig_Down.ToString(), cbb_JigD);
                            tb_num_Stand_Height.Text = string.Format("{0:F2}", obj.Height_Stand);
                           Global.Model_Thickness_Bearings_U = Fill_Bearings_JigUD (path.Bearings_Up, obj.ID_Bearings_Up.ToString());
                            Global.Model_Thickness_Bearings_D = Fill_Bearings_JigUD(path.Bearings_Down, obj.ID_Bearings_Down.ToString());
                            Global.Model_Thickness_Jig_Up = Fill_Bearings_JigUD(path.Jig_Up, obj.Jig_Up.ToString());
                            Global.Model_Thickness_Jig_Down = Fill_Bearings_JigUD(path.Jig_Down, obj.Jig_Down.ToString());
                            tb_num_Distance_Bearings_Before.Text = string.Format("{0:F2}", obj.Pre_press_Bearings_distance);
                            tb_num_Distance_Bearings_After.Text = string.Format("{0:F2}", obj.After_press_bearings_distance);
                            tb_num_Ofset.Text = string.Format("{0:F2}", obj.Ofset_position1);
                            Global.Function1.Clear();   
                            Global.Function2.Clear();
                            Global.Function1.AddRange(obj.Data_Func1);
                            Global.Function2.AddRange(obj.Data_Func2);
                            tb_num_Origin_PST.Text= string.Format("{0:F2}", obj.Origin_Position);
                            tb_num_Origin_Velo.Text = string.Format("{0:F2}", obj.Origin_Velocity);
                            tb_num_PST_Standby.Text = string.Format("{0:F2}", obj.Standby_Position);
                            tb_num_Standby_Velocity.Text = string.Format("{0:F2}", obj.Standby_Velocity);
                            tb_num_Standby_Time.Text = string.Format("{0:F2}", obj.Standby_Time);
                            cbb_Pressing_condition.SelectedIndex = -1;
                            cbb_step.SelectedIndex = -1;
                            Load_View_Codition();
                            tb_num_Force_Max.Text = "0";
                            tb_num_Force_Min.Text = "0";
                            tb_num_Position_Max.Text = "0";
                            tb_num_Position_Min.Text = "0";
                            tb_num_Press_PositionDistance.Text = "0";
                            tb_num_Press_Force.Text = "0";
                            tb_num_Press_Velocity.Text = "0";
                            tb_num_Press_Time.Text = "0";
                            // Load_CBB();
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
                Common.Log_Operation("Delete Model:  " + tb_Model.Text, path.Log_EN);
                Common.Log_Operation("Xóa Model:  " + tb_Model.Text, path.Log_VN);
                // Write back to file
                File.WriteAllText(path.Model, newJsonString);
                Common.Load_View_Model(List_Models);
            }
            else
            {
                MessageBox.Show("Không tìm thấy mã Model: " + tb_Model.Text + " cần xóa");
            }

        }
        public void Load_View_Codition()
        {
            try
            {
                // string List_Show = File.ReadAllText(path.Model);

                var Pressing_Condition = new List<DataView_PressingCondition>
                {
                    new DataView_PressingCondition { No = 1,PressingCondition = Global.Function1[0].Press_Condition },
                    new DataView_PressingCondition { No = 2,PressingCondition = Global.Function2[0].Press_Condition }
                };
                List_Pressing_condition.ItemsSource = Pressing_Condition;

            }
            catch
            {

            }
        }

        private void CheckKeyboardStatus()
        {
            var arrProcs = Process.GetProcessesByName("osk");
            var focusedElement = Keyboard.FocusedElement;


            if ((arrProcs.Length == 0) & keyboardIsOpen)
            {

                Console.WriteLine("Bàn phím ảo đang đóng.");
                if (focusedElement is TextBox textBox)
                {

                    Keyboard.ClearFocus();
                    TextBox_LostFocus(textBox, null);
                    Global.clear_forcus = false;
                    Console.WriteLine("Đã lostforcus");
                    keyboardIsOpen = false;
                    //  }

                }//
            }
            else if (!(arrProcs.Length == 0) & !keyboardIsOpen)
            {
                keyboardIsOpen = true;
            }
            if (Global.clear_forcus)
            {
                if (focusedElement is TextBox textBox)
                {
                    //  textBox.Text = Global.Textbox_string ;
                    TextBox_LostFocus(textBox, null);
                    Keyboard.ClearFocus();
                    Global.clear_forcus = false;
                    Console.WriteLine("Đã lostforcus");
                    Global.Textbox_string = "";
                    keyboardIsOpen = false;
                    //  }

                }//
            }

        }  //
        private static float Fill_Bearings_JigUD(string path, string id)
        {
            try
            {
                string jsons = File.ReadAllText(path); ;
                if (jsons.Length > 0)
                {
                    JArray jsonArray = JArray.Parse(jsons);
                    foreach (JObject obj in jsonArray)
                    {
                        if ((string)obj["ID"] == id)
                        {
                            return (float)obj["Thickness"];
                        }
                    }
                }
            }
            catch { }

            return -1;
        }
        private bool AreTextBoxesFilled()
        {
            // Kiểm tra từng TextBox
            return !string.IsNullOrWhiteSpace(tb_Model.Text) &&
                   !string.IsNullOrWhiteSpace(cbb_BearingsU.SelectedItem.ToString()) &&
                   !string.IsNullOrWhiteSpace(cbb_BearingsD.SelectedItem.ToString()) &&
                   !string.IsNullOrWhiteSpace(tb_Rotor.Text) &&
                   !string.IsNullOrWhiteSpace(tb_Shaft.Text) &&
                   !string.IsNullOrWhiteSpace(tb_num_Stand_Height.Text) &&
                   !string.IsNullOrWhiteSpace(tb_num_Distance_Bearings_After.Text) &&
                   !string.IsNullOrWhiteSpace(tb_num_Distance_Bearings_Before.Text) &&
                   !string.IsNullOrWhiteSpace(cbb_JigU.SelectedItem.ToString()) &&
                   !string.IsNullOrWhiteSpace(cbb_JigM.SelectedItem.ToString()) &&
                   !string.IsNullOrWhiteSpace(cbb_JigD.SelectedItem.ToString());
        }
        private void Click_bt_Del_Model(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName != "")
            {
                Del_Model();
            }

            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }
        }

        private void Click_bt_Import_model(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName != "")
            {
                excel.Export_Model_File("Template_Model", path.Model, false, "Chọn thư mục lưu file Backup", "Backup_Model");
                excel.Import_Model_Filepath();

                Common.Log_Operation("Import Model  " , path.Log_EN);
                Common.Log_Operation("Nhập Model  " , path.Log_VN);
                Common.Load_View_Model(List_Models);
            }

            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }
        }

        private void Click_bt_Export_model(object sender, RoutedEventArgs e)
        {
            excel.Export_Model_File("Template_Model", path.Model, true, "Chọn thư mục lưu file", "Model");
        }


        private void Click_bt_Save_model(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserName != "")
            {
                if (!AreTextBoxesFilled())
                {
                    MessageBox.Show("Vui Lòng nhập đẩy đủ thông tin model");
                }
                else if (tb_num_Stand_Height.Text == "0")
                {
                    MessageBox.Show("Vui Lòng nhập giá trị độ cao tiêu chuẩn");
                }
                else if (Global.Model_Thickness_Bearings_U <= 0)
                {
                    MessageBox.Show("Vui Lòng nhập giá trị độ dày tại setting vòng bi trên");
                }
                else if (Global.Model_Thickness_Bearings_D <= 0)
                {
                    MessageBox.Show("Vui Lòng nhập giá trị độ dày tại setting vòng bi dưới");
                }
                else if (tb_num_Distance_Bearings_Before.Text == "0")
                {
                    MessageBox.Show("Vui Lòng nhập giá trị khoảng cách 2 vòng bi trước khi ép");
                }
                else if (tb_num_Distance_Bearings_After.Text == "0")
                {
                    MessageBox.Show("Vui Lòng nhập giá trị khoảng cách 2 vòng bi sau khi ép");
                }
                else if (Global.Standby_Position < float.Parse(tb_num_PST_Standby.Text) & tb_num_PST_Standby.Text != "" & tb_num_PST_Standby.Text != null)
                {
                    MessageBox.Show(" Vị trí chờ làm việc tối đa của Model là :" + Global.Standby_Position.ToString() + " , vui lòng kiểm tra lại!");
                }
                else if ((Global.Function1[0].Mode == 2 || Global.Function1[0].Mode == 3 || Global.Function1[0].Mode == 5) & (Global.Function1[0].End_Max_Pos_Limit < float.Parse(tb_num_PST_Standby.Text)|| Global.Function1[0].End_Min_Pos_Limit < float.Parse(tb_num_PST_Standby.Text)))
                {
                    MessageBox.Show("Giá trị giới hạn của điều kiện ép1 nhỏ hơn vị trí chờ làm việc, vui lòng kiểm tra lại !");
                }
                else if ((Global.Function2[0].Mode == 2 || Global.Function2[0].Mode == 3 || Global.Function2[0].Mode == 5) & (Global.Function2[0].End_Max_Pos_Limit < float.Parse(tb_num_PST_Standby.Text) || Global.Function2[0].End_Min_Pos_Limit < float.Parse(tb_num_PST_Standby.Text)))
                {
                    MessageBox.Show("Giá trị giới hạn của điều kiện ép1 nhỏ hơn vị trí chờ làm việc, vui lòng kiểm tra lại !");
                }
                else if ((Global.Function1[0].End_Max_Force_Limit > 3300) || Global.Function1[0].End_Min_Force_Limit > 3300)
                {
                    MessageBox.Show("Giá trị giới hạn lực ép của điều kiện ép 1 lớn hơn lực ép lớn nhất (3300N), vui lòng kiểm tra lại !");
                }
                else if ((Global.Function2[0].End_Max_Force_Limit > 3300) || Global.Function2[0].End_Min_Force_Limit > 3300)
                {
                    MessageBox.Show("Giá trị giới hạn lực ép của điều kiện ép 1 lớn hơn lực ép lớn nhất (3300N), vui lòng kiểm tra lại !");
                }
                else
                {
                    Save_Model();
                }
            }

            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }
        }

        private void Click_bt_del_Condition(object sender, RoutedEventArgs e)
        {
            int selectedValue;
            if (MainWindow.UserName != "")
            {
                if (cbb_step.SelectedItem != null & cbb_Pressing_condition.SelectedItem != null)
            {
                var selectedItem = (ComboBoxItem)cbb_step.SelectedItem;
                if (int.TryParse(selectedItem.Content.ToString(), out selectedValue))
                {
                    Del_Pressing_condition(selectedValue);
                }
            }
            }

            else
            {
                MessageBox.Show("Vui Lòng Đăng Nhập");
            }

        }
        private static bool Is_String(string input, string Compari_1)
        {
            return input.Contains(Compari_1) ;
        }
    }
}
