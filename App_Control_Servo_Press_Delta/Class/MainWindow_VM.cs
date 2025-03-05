using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Oxyplot_test;

namespace ViewModel
{
    public class MainWindow_VM : INotifyPropertyChanged
    {
        public ICommand GridClickCommand { get; }

        public MainWindow_VM()
        {

            GridClickCommand = new RelayCommand(CloseDialogs);

        }
        private void CloseDialogs()
        {
            // Lấy tất cả các cửa sổ đang mở và đóng chúng
            var openWindows = Application.Current.Windows.Cast<Window>().Where(w => w.IsVisible && w != Application.Current.MainWindow);
            foreach (var window in openWindows)
            {
                window.Close();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
